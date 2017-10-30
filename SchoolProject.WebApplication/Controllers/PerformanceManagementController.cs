using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SchoolProject.WebApplication.DTO;
using SchoolProject.WebApplication.Models;
using SchoolProject.WebApplication.ServiceManager.Interface;
using SchoolProject.WebApplication.ViewModels;

namespace SchoolProject.WebApplication.Controllers
{
    [Authorize]
    [HandleError]
    public class PerformanceManagementController : _BaseController {
        private readonly IAdmininstrationManager _iAdminstrationManager;
        private ApplicationDatabaseContext _dbContext;

        public PerformanceManagementController(IAdmininstrationManager iAdmininstrationManager) {
            _iAdminstrationManager = iAdmininstrationManager;
            _dbContext = new ApplicationDatabaseContext();
        }

        [HttpGet]
        public ActionResult ChangeUserPassword(string username) {
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Account");
            var user = _dbContext.Users.FirstOrDefault(x => string.Compare(x.UserName,username, true) ==0);
            if (user != null) {
                return RedirectToAction("ChangePassword", "Account", new { userId = user.Id});
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public ActionResult ManageReview(string username) {
            if (!string.IsNullOrEmpty(username)) {
                var modelView = new ManageReviewModelView();
                modelView.EmployeeReviewPeriods = GetEmployeeReviewPeriods(username);
                modelView.UserRole = GetUserRole(username);
                modelView.DirectReportReviews = GetDirectReportingReviewPeriods(username);
                return View(modelView);
            }
            return RedirectToAction("Login", "Account");
        }

        public string GetUserRole(string username) {
            var user = _dbContext.Users.Include(x => x.Roles).FirstOrDefault(x => string.Compare(x.UserName, username) == 0);

            if (user != null)
                if (user.Roles.Count > 0) {
                    var roleId = user.Roles.FirstOrDefault().RoleId;
                    return _dbContext.Roles.FirstOrDefault(x => string.Compare(x.Id, roleId, true) == 0).Name;
                }
            return ("Employee");
        }

        [HttpGet]
        public ActionResult CreateReviewDocument() {
            var userName = User.Identity.Name;
            var employee = GetEmployee(userName);
            if (employee != null) {
                var createReviewPeriodModel = new PerformanceReviewModelView() {
                    ReportingStructureId = employee.ReviewReportingStructureId,
                    ReviewPeriods = ReviewPeriods()
                };
                return View(createReviewPeriodModel);
            }
            else {
                var createReviewPeriodModel = new PerformanceReviewModelView() {
                    ProcessingStatus = false,
                    ProcessingStatusMessage = "Your Line Manager is not yet added, please ask System Administrator to add your Line Manager.",
                    ReviewPeriods = ReviewPeriods(),
                    ReportingStructureId = -1
                };
                return View(createReviewPeriodModel);
            }
        }

        [HttpPost]
        public ActionResult CreateReviewDocument(PerformanceReviewModelView createReviewPeriodModel) {
            if (ModelState.IsValid) {
                if (!PerformanceReviewPeriodExist(createReviewPeriodModel)) {
                    var pmReviewPeriod = TransformPMReview(createReviewPeriodModel);
                    _dbContext.PMReview.Add(pmReviewPeriod);
                    _dbContext.SaveChanges();
                    //Set PMReviewProgressStatus Content Creation
                    var setProgressStatus = CreatePMReviewProgressStatus(1, pmReviewPeriod.PMReviewId);

                    var reviewName = createReviewPeriodModel.ReviewPeriods.FirstOrDefault(x => string.Compare(x.ValueText, 
                                                  createReviewPeriodModel.ReviewPeriodId.ToString()) == 0).DisplayText;
                    createReviewPeriodModel.ReviewPeriodId = 0;
                    createReviewPeriodModel.ProcessingStatus = true;
                    createReviewPeriodModel.ProcessingStatusMessage = string.Format("Successfully created Performance Review Period {0}", reviewName);
                }
                else {
                    createReviewPeriodModel.ProcessingStatus = false;
                    createReviewPeriodModel.ProcessingStatusMessage = "Performance Review already exist, please complete review or create " +
                                                                  "a different Peformance Review Period.";
                }
                
            }
            
            return View(createReviewPeriodModel);
        }

        private bool PerformanceReviewPeriodExist(PerformanceReviewModelView createReviewPeriodModel) {
            var result = _dbContext.PMReview.FirstOrDefault(x => x.DateDeleted == null && 
                                                   x.PMReviewPeriodId == createReviewPeriodModel.ReviewPeriodId && 
                                                   x.ReviewReportingStructureId == createReviewPeriodModel.ReportingStructureId);
            return result == null ? false : true;

        }

        private bool CreatePMReviewProgressStatus(int processStageId, int pmReviewId) {
            var employee = GetEmployee(User.Identity.Name);
            var progressStage = new PMReviewProgressStatus() {
                ProcessStageId = processStageId,
                PMReviewId = pmReviewId,
                DateCreated = DateTime.Now,
                CreatedBy = string.Format("{0} {1}", employee.Owner.Name, employee.Owner.Surname),
                StatusId = 1,
                DateModified = DateTime.Now,
                ModifiedBy = string.Format("{0} {1}", employee.Owner.Name, employee.Owner.Surname),
                StartDate = DateTime.Now,
                EndDate = DateTime.Now
            };
            _dbContext.PMReviewProgressStatus.Add(progressStage);
            _dbContext.SaveChanges();
            return (true);
        }

        private PMReview TransformPMReview(PerformanceReviewModelView createReviewPeriodModel) {
            var employee = GetEmployee(User.Identity.Name);
            return (new PMReview() {
                ReviewReportingStructureId = createReviewPeriodModel.ReportingStructureId,
                PMReviewPeriodId = createReviewPeriodModel.ReviewPeriodId,
                DateCreated = DateTime.Now,
                CreatedBy = string.Format("{0} {1}", employee.Owner.Name, employee.Owner.Surname),
                StatusId = 1,
                DateModified = DateTime.Now,
                ModifiedBy = string.Format("{0} {1}", employee.Owner.Name, employee.Owner.Surname)
            });
        }

        public PMReviewReportingStructure GetEmployee(string userName) {
            var employee = _dbContext.PMReviewReportingStructure.Include(x => x.Owner).
                           FirstOrDefault(x => string.Compare(x.Owner.NetworkUsername, userName, true) == 0 && x.DateDeleted == null);
            return (employee);
        }

        public List<SelectionOptions> ReviewPeriods() {
            var results = new List<SelectionOptions>();
            var reviewPeriods = _dbContext.PMReviewPeriod.Where(x => x.DateDeleted == null).Include(x => x.ReviewPeriod).Include(x => x.PerformanceYear).ToList();
            foreach(var review in reviewPeriods) {
                results.Add(new SelectionOptions() {
                    ValueText = review.PMReviewPeriodId.ToString(),
                    DisplayText = string.Format("{0} - {1}", review.PerformanceYear.PerformanceYearName,review.ReviewPeriod.ReviewPeriodName)
                });
            }
            return (results);
        }

        public List<ManagePerformanceReview> GetEmployeeReviewPeriods(string usernmame) {
            var performanceReviews = new List<ManagePerformanceReview>();
            var results = _dbContext.PMReviewProgressStatus.Where(x => x.DateDeleted == null).Include(x => x.ProcessStage).
                              Include(x => x.PMReview).Include(x => x.PMReview.ReportingStructure).Include(x => x.PMReview.ReportingStructure.Owner).
                              Include(x => x.PMReview.ReportingStructure.Manager).Include(x => x.PMReview.ReportingStructure.DocumentType).
                              Include(x => x.PMReview.PMReviewPeriod.PerformanceYear).
                              Include(x=> x.PMReview.PMReviewPeriod.ReviewPeriod).
                              Where(x => x.PMReview.ReportingStructure.Owner.NetworkUsername == usernmame).ToList();

            var reviews = results.Select(x => x.PMReviewId).Distinct();
            foreach(var reviewId in reviews) {
                var filterReview = results.OrderByDescending(x => x.ProcessStageId).FirstOrDefault(x => x.PMReviewId == reviewId);
                performanceReviews.Add(new ManagePerformanceReview() {
                    PerformancereviewPeriodStageId = filterReview.PMReviewProgressStatusId,
                    ProcessStageId = filterReview.ProcessStageId,
                    ReviewStageName = filterReview.ProcessStage.ProcessStageName,
                    PMReviewId = filterReview.PMReviewId,
                    ReviewPeriodName = string.Format("{0} - {1}", filterReview.PMReview.PMReviewPeriod.PerformanceYear.PerformanceYearName,
                                                                  filterReview.PMReview.PMReviewPeriod.ReviewPeriod.ReviewPeriodName),
                    DocumentOwnerId = filterReview.PMReview.ReportingStructure.MemberId,
                    LineManagerId = filterReview.PMReview.ReportingStructure.ManagerId,
                    EmployeeName = string.Format("{0} {1}", filterReview.PMReview.ReportingStructure.Owner.Name,
                                                            filterReview.PMReview.ReportingStructure.Owner.Name),
                    LineManagerName = string.Format("{0} {1}", filterReview.PMReview.ReportingStructure.Manager.Name,
                                                            filterReview.PMReview.ReportingStructure.Manager.Name),
                    NetworkUsername = usernmame,
                    DocumentType = filterReview.PMReview.ReportingStructure.DocumentType.DocumentTypeName
                });
            }

            return(performanceReviews);
        }

        public List<ManagePerformanceReview> GetDirectReportingReviewPeriods(string usernmame) {
            var performanceReviews = new List<ManagePerformanceReview>();
            var results = _dbContext.PMReviewProgressStatus.Where(x => x.DateDeleted == null).Include(x => x.ProcessStage).
                              Include(x => x.PMReview).Include(x => x.PMReview.ReportingStructure).Include(x => x.PMReview.ReportingStructure.Owner).
                              Include(x => x.PMReview.ReportingStructure.Manager).Include(x => x.PMReview.ReportingStructure.DocumentType).
                              Include(x => x.PMReview.PMReviewPeriod.PerformanceYear).
                              Include(x => x.PMReview.PMReviewPeriod.ReviewPeriod).
                              Where(x => x.PMReview.ReportingStructure.Manager.NetworkUsername == usernmame).ToList();

            var reviews = results.Select(x => x.PMReviewId).Distinct();
            foreach (var reviewId in reviews) {
                var filterReview = results.OrderByDescending(x => x.ProcessStageId).FirstOrDefault(x => x.PMReviewId == reviewId);
                performanceReviews.Add(new ManagePerformanceReview() {
                    PerformancereviewPeriodStageId = filterReview.PMReviewProgressStatusId,
                    ProcessStageId = filterReview.ProcessStageId,
                    ReviewStageName = filterReview.ProcessStage.ProcessStageName,
                    PMReviewId = filterReview.PMReviewId,
                    ReviewPeriodName = string.Format("{0} - {1}", filterReview.PMReview.PMReviewPeriod.PerformanceYear.PerformanceYearName,
                                                                  filterReview.PMReview.PMReviewPeriod.ReviewPeriod.ReviewPeriodName),
                    DocumentOwnerId = filterReview.PMReview.ReportingStructure.MemberId,
                    LineManagerId = filterReview.PMReview.ReportingStructure.ManagerId,
                    EmployeeName = string.Format("{0} {1}", filterReview.PMReview.ReportingStructure.Owner.Name,
                                                            filterReview.PMReview.ReportingStructure.Owner.Name),
                    LineManagerName = string.Format("{0} {1}", filterReview.PMReview.ReportingStructure.Manager.Name,
                                                            filterReview.PMReview.ReportingStructure.Manager.Name),
                    NetworkUsername = usernmame,
                    DocumentType = filterReview.PMReview.ReportingStructure.DocumentType.DocumentTypeName
                });
            }
            return (performanceReviews);
        }

        [HttpGet]
        public ActionResult AddPerformanceReviewContents(int? performanceReviewId, FormModeOption? formProcessingMode, long? measureId) {
            if ((performanceReviewId == null) || (formProcessingMode == null))
                return RedirectToAction("ManageReview", new { username = User.Identity.Name });
            var modelView = new CreateMeasureModelView();
            modelView.Username = User.Identity.Name;
            modelView.ManagerUsername = ManagerNetworkUsername((int)performanceReviewId);
            modelView.CurrentReviewStatus = GetCurrentReviewStatus((int)performanceReviewId);
            modelView.PerformanceReviewId = (int)performanceReviewId;
            modelView.StrategicGoals = GetReviewStrategicGoals();
            modelView.FormProcessingMode = (FormModeOption)formProcessingMode;

            if ((formProcessingMode == FormModeOption.EDIT) || (formProcessingMode == FormModeOption.DELETE)) {
                var measure = _dbContext.PMMeasure.Find((long)measureId);
                if (measure != null) {
                    modelView.MeasureId = measure.PMMeasureId;
                    modelView.MeasureName = measure.MeasureName;
                    modelView.MeasureWeight = measure.MeasureWeight;
                    modelView.StrategicGoalId = measure.PMStrategicGoalId;
                    modelView.ObjectiveName = measure.PMObjective;
                }
            }
            //REFRESH
            modelView.CreatedMeasures = GetReviewCapturedMeasures((int)performanceReviewId);
            modelView.StrategicGoals = GetReviewStrategicGoals();

            return View(modelView);
        }

        public string ManagerNetworkUsername(int performanceReviewId) {
            var result = _dbContext.PMReview.Include(x => x.ReportingStructure.Owner).Include(x => x.ReportingStructure.Manager).
                         FirstOrDefault(x => x.PMReviewPeriodId == performanceReviewId && x.ReportingStructure.Manager.DateDeleted == null);
            if (result != null)
                return result.ReportingStructure.Manager.NetworkUsername;
            return ("Unknown");
        }

        public List<SelectionOptions> GetReviewStrategicGoals() {
            var results = new List<SelectionOptions>();
            var goals = _dbContext.StrategicGoal.Where(x => x.DateDeleted == null);
            foreach(var goal in goals) {
                results.Add(new SelectionOptions() {
                    DisplayText = goal.StrategicGoalName,
                    ValueText = goal.StrategicGoalId.ToString()
                });
            }
            return (results);
        }

        public ReviewProcessStatus GetCurrentReviewStatus(int performanceReviewId) {
            var review = _dbContext.PMReviewProgressStatus.Where(x => x.PMReviewId == performanceReviewId).Include(x => x.ProcessStage).
                         OrderByDescending(x => x.PMReviewProgressStatusId).FirstOrDefault();
            if (review == null)
                return ReviewProcessStatus.Unknown;
            return CastReviewProgressStatus(review.ProcessStage.ProcessStageName);

        }

        public ReviewProcessStatus CastReviewProgressStatus(string reviewStageName) {
            switch (reviewStageName.ToUpper()) {
                case "CONTENT CREATION":
                    return ReviewProcessStatus.Content_Creation;
                case "CONTENT CREATION COMPLETED":
                    return ReviewProcessStatus.Content_Creattion_Completed;
                case "EMPLOYEE SCORING":
                    return ReviewProcessStatus.Employee_Scoring;
                case "LINE MANAGER SCORING":
                    return ReviewProcessStatus.Line_Manager_Scoring;
                case "SCORING COMPLETED":
                    return ReviewProcessStatus.Scoring_Completed;
                default:
                    return ReviewProcessStatus.Unknown;
            }
        }

        public List<PerformanceReeviewContent> GetReviewCapturedMeasures(int PerformanceReviewId) {
            var currentMesures = new List<PerformanceReeviewContent>();
            var results = _dbContext.PMMeasure.Where(X => X.DateDeleted == null).Include(x => x.StrategeicGoal).
                                              Include(x => x.StrategeicGoal.StrategicGoal);
            foreach( var measure in results) {
                currentMesures.Add(new PerformanceReeviewContent() {
                    MeasureId = measure.PMMeasureId,
                    MeasureName = measure.MeasureName,
                    ObjectiveName = measure.PMObjective,
                    StrategicGoalID = measure.PMStrategicGoalId,
                    StrategicGoalName = measure.StrategeicGoal.StrategicGoal.StrategicGoalName,
                    MeasureWeight = measure.MeasureWeight
                });
            }
            return (currentMesures);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPerformanceReviewContents(CreateMeasureModelView modelView) {
            if (ModelState.IsValid) {
                switch (modelView.FormProcessingMode) {
                    case FormModeOption.CREATE: {
                            if (AddNewMeasure(modelView)) {
                                modelView.MeasureName = string.Empty;
                                modelView.ObjectiveName = string.Empty;
                                modelView.MeasureWeight = 0;
                                modelView.StrategicGoalId = 0;
                                modelView.ProcessingStatus = true;
                                modelView.ProcessingStatusMessage = string.Format("Successfully added measure: {0}.", modelView.MeasureName);
                            }
                            else {
                                modelView.ProcessingStatus = false;
                                modelView.ProcessingStatusMessage = string.Format("An error occurred while trying to save measure: {0}. " +
                                                 "Please try again and if the error persist contact System Support.", modelView.MeasureName);
                            }
                            break;
                        }
                    case FormModeOption.EDIT:
                        
                        break;
                    case FormModeOption.DELETE:
                        break;
                }
            }
            modelView.CreatedMeasures = GetReviewCapturedMeasures(modelView.PerformanceReviewId);
            //Model contains errors
            return View(modelView);
        }

        private bool AddNewMeasure(CreateMeasureModelView modelView) {
            var pmStrategicGoalId = 0;
            if (!StrategicGoalExist(modelView.StrategicGoalId, modelView.PerformanceReviewId)) {
                var addStrategicGoal = _dbContext.PMStrategicGoal.Add(TransformStrategicGoal(modelView));
                _dbContext.SaveChanges();
                pmStrategicGoalId = addStrategicGoal.PMStrategicGoalId;
            }
            else {
                pmStrategicGoalId = GetReviewStrategicGoal(modelView);
            }

            if (pmStrategicGoalId == 0)
                return false;
            var measure = TransformMeasure(modelView, pmStrategicGoalId);
            _dbContext.PMMeasure.Add(measure);
            _dbContext.SaveChanges();
            return measure.PMMeasureId > 0 ? true : false;
        }

        private int GetReviewStrategicGoal(CreateMeasureModelView modelView) {
            return (_dbContext.PMStrategicGoal.FirstOrDefault(x => x.PMReviewId == modelView.PerformanceReviewId &&
                                                                         x.StrategicGoalId == modelView.StrategicGoalId &&
                                                                         x.DateDeleted == null).PMStrategicGoalId);
        }

        private PMeasure TransformMeasure(CreateMeasureModelView modelView, int reviewStrategicGoalId) {
            return (new PMeasure() {
                PMStrategicGoalId = reviewStrategicGoalId,
                MeasureName = modelView.MeasureName,
                MeasureWeight = modelView.MeasureWeight,
                PMObjective =modelView.ObjectiveName,
                SubjectMatterExpert = "Employee",
                StatusId = 1,
                CreatedBy = modelView.Username,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
                ModifiedBy = modelView.Username,
                AuditScore = 1,
                LineManagerScore = 1,
                EmployeeScore = 1,
                TermId = 1
            });
        }

        private PMStrategicGoal TransformStrategicGoal(CreateMeasureModelView modelView) {
            return (new PMStrategicGoal() {
                StrategicGoalId = modelView.StrategicGoalId,
                PMReviewId = modelView.PerformanceReviewId,
                StrategicGoalWeight = 0.00M,
                StatusId = 1,
                CreatedBy = modelView.Username,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
                ModifiedBy = modelView.Username
            });
        }

        private bool StrategicGoalExist(int strategicGoalId, int performanceReviewId) {
            var result = _dbContext.PMStrategicGoal.FirstOrDefault(x => x.PMReviewId == performanceReviewId && 
                                                                         x.StrategicGoalId == strategicGoalId && 
                                                                         x.DateDeleted == null);
            return result == null ? false : true;
        }
    }
}