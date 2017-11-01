using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SchoolProject.WebApplication.DTO;
using SchoolProject.WebApplication.Models;
using SchoolProject.WebApplication.ServiceManager.Interface;
using SchoolProject.WebApplication.ViewModels;

namespace SchoolProject.WebApplication.Controllers {
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
            var user = _dbContext.Users.FirstOrDefault(x => string.Compare(x.UserName, username, true) == 0);
            if (user != null) {
                return RedirectToAction("ChangePassword", "Account", new { userId = user.Id });
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
            foreach (var review in reviewPeriods) {
                results.Add(new SelectionOptions() {
                    ValueText = review.PMReviewPeriodId.ToString(),
                    DisplayText = string.Format("{0} - {1}", review.PerformanceYear.PerformanceYearName, review.ReviewPeriod.ReviewPeriodName)
                });
            }
            return (results);
        }

        public List<ManagePerformanceReview> GetEmployeeReviewPeriods(string usernmame) {
            var performanceReviews = new List<ManagePerformanceReview>();

            var filterReviews = _dbContext.PMReview.Where(x => x.DateDeleted == null && x.ReportingStructure.Owner.NetworkUsername == usernmame).
                              Include(x => x.ReportingStructure).Include(x => x.PMReviewPeriod).
                              Include(x => x.PMReviewPeriod.PerformanceYear).Include(x => x.PMReviewPeriod.ReviewPeriod).
                              Include(x => x.ReportingStructure.Manager).Include(x => x.ReportingStructure.Owner).
                              Include(x => x.ReportingStructure.DocumentType).Include(x => x.PMReviewPeriod.PerformanceYear).
                              Include(x => x.PMReviewPeriod.ReviewPeriod);
            var reviewIds = filterReviews.Select(x => x.PMReviewId).ToList();
            var reviewStages = _dbContext.PMReviewProgressStatus.Where(x => reviewIds.Contains(x.PMReviewId)).Include(x => x.ProcessStage).ToList();
            foreach (var review in filterReviews) {
                var stage = reviewStages.OrderByDescending(x => x.ProcessStageId).FirstOrDefault(x => x.PMReviewId == review.PMReviewId);
                performanceReviews.Add(new ManagePerformanceReview() {
                    PerformancereviewPeriodStageId = stage.PMReviewProgressStatusId,
                    ProcessStageId = stage.ProcessStageId,
                    ReviewStageName = stage.ProcessStage.ProcessStageName,
                    PMReviewId = stage.PMReviewId,
                    ReviewPeriodName = string.Format("{0} - {1}", review.PMReviewPeriod.PerformanceYear.PerformanceYearName,
                                                                  review.PMReviewPeriod.ReviewPeriod.ReviewPeriodName),
                    DocumentOwnerId = review.ReportingStructure.MemberId,
                    LineManagerId = review.ReportingStructure.ManagerId,
                    EmployeeName = string.Format("{0} {1}", review.ReportingStructure.Owner.Name,
                                                            review.ReportingStructure.Owner.Name),
                    LineManagerName = string.Format("{0} {1}", review.ReportingStructure.Manager.Name,
                                                              review.ReportingStructure.Manager.Name),
                    NetworkUsername = usernmame,
                    ManagerUsername = review.ReportingStructure.Manager.NetworkUsername,
                    DocumentType = review.ReportingStructure.DocumentType.DocumentTypeName
                });
            }
            return (performanceReviews);
        }

        public List<ManagePerformanceReview> GetDirectReportingReviewPeriods(string usernmame) {
            var performanceReviews = new List<ManagePerformanceReview>();

            var filterReviews = _dbContext.PMReview.Where(x => x.DateDeleted == null && x.ReportingStructure.Manager.NetworkUsername == usernmame).
                              Include(x => x.ReportingStructure).Include(x => x.PMReviewPeriod).
                              Include(x => x.PMReviewPeriod.PerformanceYear).Include(x => x.PMReviewPeriod.ReviewPeriod).
                              Include(x => x.ReportingStructure.Manager).Include(x => x.ReportingStructure.Owner).
                              Include(x => x.ReportingStructure.DocumentType).Include(x => x.PMReviewPeriod.PerformanceYear).
                              Include(x => x.PMReviewPeriod.ReviewPeriod);
            var reviewIds = filterReviews.Select(x => x.PMReviewId).ToList();
            var reviewStages = _dbContext.PMReviewProgressStatus.Where(x => reviewIds.Contains(x.PMReviewId)).Include(x => x.ProcessStage).ToList();
            foreach (var review in filterReviews) {
                var stage = reviewStages.OrderByDescending(x => x.ProcessStageId).FirstOrDefault(x => x.PMReviewId == review.PMReviewId);
                performanceReviews.Add(new ManagePerformanceReview() {
                    PerformancereviewPeriodStageId = stage.PMReviewProgressStatusId,
                    ProcessStageId = stage.ProcessStageId,
                    ReviewStageName = stage.ProcessStage.ProcessStageName,
                    PMReviewId = stage.PMReviewId,
                    ReviewPeriodName = string.Format("{0} - {1}", review.PMReviewPeriod.PerformanceYear.PerformanceYearName,
                                                                  review.PMReviewPeriod.ReviewPeriod.ReviewPeriodName),
                    DocumentOwnerId = review.ReportingStructure.MemberId,
                    LineManagerId = review.ReportingStructure.ManagerId,
                    EmployeeName = string.Format("{0} {1}", review.ReportingStructure.Owner.Name,
                                                            review.ReportingStructure.Owner.Name),
                    LineManagerName = string.Format("{0} {1}", review.ReportingStructure.Manager.Name,
                                                              review.ReportingStructure.Manager.Name),
                    NetworkUsername = review.ReportingStructure.Owner.NetworkUsername,
                    ManagerUsername = review.ReportingStructure.Manager.NetworkUsername,
                    DocumentType = review.ReportingStructure.DocumentType.DocumentTypeName
                });
            }
            return (performanceReviews);
        }

        [HttpGet]
        public ActionResult AddPerformanceReviewContents(int? performanceReviewId, FormModeOption? formProcessingMode, long?
                                                              measureId, bool? processingStatus, string message) {
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

            if (!string.IsNullOrEmpty(message)) {
                modelView.ProcessingStatusMessage = message;
                modelView.ProcessingStatus = (bool)processingStatus;
            }
            //REFRESH
            modelView.CreatedMeasures = GetReviewCapturedMeasures((int)performanceReviewId);
            modelView.StrategicGoals = GetReviewStrategicGoals();

            return View(modelView);
        }

        public string ManagerNetworkUsername(int performanceReviewId) {
            var result = _dbContext.PMReview.Include(x => x.ReportingStructure.Owner).Include(x => x.ReportingStructure.Manager).
                         FirstOrDefault(x => x.PMReviewId == performanceReviewId && x.ReportingStructure.Manager.DateDeleted == null);
            if (result != null)
                return result.ReportingStructure.Manager.NetworkUsername;
            return ("Unknown");
        }

        public List<SelectionOptions> GetReviewStrategicGoals() {
            var results = new List<SelectionOptions>();
            var goals = _dbContext.StrategicGoal.Where(x => x.DateDeleted == null);
            foreach (var goal in goals) {
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
                    return ReviewProcessStatus.Content_Creation_Completed;
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
            var results = _dbContext.PMMeasure.Where(X => X.DateDeleted == null && X.StrategeicGoal.PMReviewId == PerformanceReviewId).
                              Include(x => x.StrategeicGoal).Include(x => x.StrategeicGoal.StrategicGoal);
            foreach (var measure in results) {
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
                                var successOne = string.Format("Successfully added measure: {0}.", modelView.MeasureName);
                                return RedirectToAction("AddPerformanceReviewContents",
                                     new {
                                         performanceReviewId = modelView.PerformanceReviewId, formProcessingMode = FormModeOption.CREATE,
                                         processingStatus = true, message = successOne
                                     });
                            }
                            else {
                                modelView.ProcessingStatus = false;
                                modelView.ProcessingStatusMessage = string.Format("An error occurred while trying to save measure: {0}. " +
                                                 "Please try again and if the error persist contact System Support.", modelView.MeasureName);
                            }
                            break;
                        }
                    case FormModeOption.EDIT:
                        var measure = _dbContext.PMMeasure.Find(modelView.MeasureId);
                        measure.MeasureName = modelView.MeasureName;
                        measure.MeasureWeight = modelView.MeasureWeight;
                        measure.PMStrategicGoalId = GetReviewStrategicGoal(modelView);
                        measure.PMObjective = modelView.ObjectiveName;
                        measure.ModifiedBy = modelView.Username;
                        measure.DateModified = DateTime.Now;
                        _dbContext.SaveChanges();
                        var success = string.Format("Successfully updated measure: {0}.", measure.MeasureName);
                        return RedirectToAction("AddPerformanceReviewContents",
                                    new {
                                        performanceReviewId = modelView.PerformanceReviewId, formProcessingMode = FormModeOption.CREATE,
                                        processingStatus = true, message = success
                                    });
                    case FormModeOption.DELETE:
                        var deleteMeasure = _dbContext.PMMeasure.Find(modelView.MeasureId);
                        deleteMeasure.DeletedBy = modelView.Username;
                        deleteMeasure.DateDeleted = DateTime.Now;
                        deleteMeasure.StatusId = 4; //Deleted
                        deleteMeasure.ModifiedBy = modelView.Username;
                        deleteMeasure.DateModified = DateTime.Now;
                        _dbContext.SaveChanges();
                        var successTwo = string.Format("Successfully Deleted measure: {0}.", deleteMeasure.MeasureName);
                        return RedirectToAction("AddPerformanceReviewContents",
                                    new {
                                        performanceReviewId = modelView.PerformanceReviewId, formProcessingMode = FormModeOption.CREATE,
                                        processingStatus = true, message = successTwo
                                    });
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
            var goal = (_dbContext.PMStrategicGoal.FirstOrDefault(x => x.PMReviewId == modelView.PerformanceReviewId &&
                                                                        x.StrategicGoalId == modelView.StrategicGoalId &&
                                                                        x.DateDeleted == null));
            if (goal != null)
                return goal.PMStrategicGoalId;
            var addStrategicGoal = _dbContext.PMStrategicGoal.Add(TransformStrategicGoal(modelView));
            _dbContext.SaveChanges();
            return (addStrategicGoal.PMStrategicGoalId);
        }

        private PMeasure TransformMeasure(CreateMeasureModelView modelView, int reviewStrategicGoalId) {
            return (new PMeasure() {
                PMStrategicGoalId = reviewStrategicGoalId,
                MeasureName = modelView.MeasureName,
                MeasureWeight = modelView.MeasureWeight,
                PMObjective = modelView.ObjectiveName,
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

        [HttpGet]
        public ActionResult ConfirmReviewPeriodContentCreation(int? performanceReviewId, string managerUsername) {
            if (performanceReviewId == null)
                return RedirectToAction("ManageReview", new { username = User.Identity.Name });
            var status = false;
            var processingMessage = string.Empty;
            var totalMeasures = _dbContext.PMMeasure.Where(x => x.StrategeicGoal.PMReviewId == (int)performanceReviewId &&
                                     x.DateDeleted == null).Include(x => x.StrategeicGoal).ToList();
            if (totalMeasures.Count == 0) {
                return RedirectToAction("AddPerformanceReviewContents",
                            new {
                                performanceReviewId = performanceReviewId, formProcessingMode = FormModeOption.CREATE,
                                processingStatus = false, message = "Please capture Performance Review Contents!!"
                            });
            }

            if (totalMeasures.Sum(x => x.MeasureWeight) == 100) {
                if (CreatePMReviewProgressStatus(2, (int)performanceReviewId, managerUsername)) {
                    status = true;
                    processingMessage = "Successfully completed Performance Review Content Creation";
                }
                else {
                    status = false;
                    processingMessage = "An error has occurred while trying to move the Performance Review to " +
                                        "Content Creation!! Please try again";
                }
            }
            else {
                status = false;
                processingMessage = string.Format("The Total Overall Performace Review Weight should be 100.00 %!. " +
                                           "Current Total Weight: {0} %", totalMeasures.Sum(x => x.MeasureWeight));

            }
            return RedirectToAction("AddPerformanceReviewContents",
                            new {
                                performanceReviewId = performanceReviewId, formProcessingMode = FormModeOption.CREATE,
                                processingStatus = status, message = processingMessage
                            });
        }

        private bool CreatePMReviewProgressStatus(int processStageId, int pmReviewId, string username) {
            var progressStage = new PMReviewProgressStatus() {
                ProcessStageId = processStageId,
                PMReviewId = pmReviewId,
                DateCreated = DateTime.Now,
                CreatedBy = username,
                StatusId = 1,
                DateModified = DateTime.Now,
                ModifiedBy = username,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now
            };
            _dbContext.PMReviewProgressStatus.Add(progressStage);
            _dbContext.SaveChanges();
            return (true);
        }

        [HttpGet]
        public ActionResult ScoreReview(int? performanceReviewId, long? measureId, bool? processingStatus, string message) {
            if (performanceReviewId == null)
                return RedirectToAction("ManageReview", new { username = User.Identity.Name });
            if (measureId == null)
                measureId = _dbContext.PMMeasure.FirstOrDefault(x => x.StrategeicGoal.PMReviewId == performanceReviewId
                                                                 && x.DateDeleted == null).PMMeasureId;
            var measure = _dbContext.PMMeasure.Include(x => x.StrategeicGoal).Include(x => x.StrategeicGoal.StrategicGoal)
                                              .FirstOrDefault(x => x.PMMeasureId == (long)measureId && x.DateDeleted == null);

            var modelView = new ScoreReviewViewModel() {
                Username = User.Identity.Name,
                ManagerUsername = ManagerNetworkUsername((int)performanceReviewId),
                CurrentReviewStatus = GetCurrentReviewStatus((int)performanceReviewId),
                PerformanceReviewId = (int)performanceReviewId,
                MeasureId = measure.PMMeasureId,
                MeasureName = measure.MeasureName,
                MeasureWeight = measure.MeasureWeight,
                StrategicGoal = measure.StrategeicGoal.StrategicGoal.StrategicGoalName,
                Objective = measure.PMObjective,
                IsLineManager = IsLineManager(User.Identity.Name, (int)performanceReviewId),
                MaximumScore = GetMaximumRating(),
                ReviewContents = GetReviewMeasures((int)performanceReviewId)
            };

            modelView.Score = measure.EmployeeScore;
            modelView.CaptureComments = measure.EmployeeComments;
            if (modelView.IsLineManager) {
                modelView.Score = measure.LineManagerScore;
                modelView.CaptureComments = measure.LineManagerComments;
            }

            if (!string.IsNullOrEmpty(message)) {
                modelView.ProcessingStatusMessage = message;
                modelView.ProcessingStatus = (bool)processingStatus;
            }

            return View(modelView);
        }

        [HttpPost]
        public ActionResult ScoreReview(ScoreReviewViewModel modelView) {
            if (ModelState.IsValid) {
                if (modelView.Score <= modelView.MaximumScore) {
                    if (UpdateMeasureScore(modelView)) {
                        modelView.ProcessingStatus = true;
                        modelView.ProcessingStatusMessage = string.Format("Successfully updated measure: {0}  score to {1}", modelView.MeasureName, modelView.Score);
                    }
                    else {
                        modelView.ProcessingStatus = false;
                        modelView.ProcessingStatusMessage = string.Format("Failed to update measure: {0}  score!!! Please try again", modelView.MeasureName);
                    }
                }
                else {
                    modelView.ProcessingStatus = false;
                    modelView.ProcessingStatusMessage = string.Format("The Maximum allowed score is {0}", modelView.MaximumScore);
                }
            }
            modelView.ReviewContents = GetReviewMeasures(modelView.PerformanceReviewId);
            //REFRESH
            return View(modelView);
        }

        private bool UpdateMeasureScore(ScoreReviewViewModel modelView) {
            var measure = _dbContext.PMMeasure.Find(modelView.MeasureId);

            //Manager Scoring
            if (modelView.IsLineManager) {
                measure.LineManagerScore = modelView.Score;
                measure.LineManagerComments = modelView.CaptureComments;
                measure.ModifiedBy = modelView.ManagerUsername;
                measure.DateModified = DateTime.Now;
            }
            else {
                measure.EmployeeScore = modelView.Score;
                measure.EmployeeComments = modelView.CaptureComments;
                measure.ModifiedBy = modelView.Username;
                measure.DateModified = DateTime.Now;
            }
            _dbContext.SaveChanges();
            return true;
        } 

        private bool IsLineManager(string username, int reviewId) {
            var employee = _dbContext.PMReview.Include(x => x.ReportingStructure.Manager).FirstOrDefault(x => x.PMReviewId == reviewId 
                                                        && x.DateDeleted == null);
            if (employee == null)
                return false;
            return string.Compare(employee.ReportingStructure.Manager.NetworkUsername, username, true) == 0 ? true : false;
        }

        public List<PerformanceReviewScoringContent> GetReviewMeasures(int PerformanceReviewId) {
            var currentMesures = new List<PerformanceReviewScoringContent>();
            var results = _dbContext.PMMeasure.Where(X => X.DateDeleted == null && X.StrategeicGoal.PMReviewId == PerformanceReviewId).
                              Include(x => x.StrategeicGoal).Include(x => x.StrategeicGoal.StrategicGoal);
            foreach (var measure in results) {
                currentMesures.Add(new PerformanceReviewScoringContent() {
                    MeasureId = measure.PMMeasureId,
                    MeasureName = measure.MeasureName,
                    ObjectiveName = measure.PMObjective,
                    StrategicGoalID = measure.PMStrategicGoalId,
                    StrategicGoalName = measure.StrategeicGoal.StrategicGoal.StrategicGoalName,
                    MeasureWeight = measure.MeasureWeight,
                    EmployeeComments = measure.EmployeeComments,
                    ManagerComments = measure.LineManagerComments,
                    EmployeeScore = measure.EmployeeScore,
                    ManagerScore = measure.LineManagerScore
                });
            }
            return (currentMesures);
        }

        [HttpGet]
        public ActionResult ConfirmReviewScoring(int? performanceReviewId, string managerUsername) {
            if (performanceReviewId == null)
                return RedirectToAction("ManageReview", new { username = User.Identity.Name });
            var status = false;
            var processingMessage = string.Empty;
            var totalMeasures = _dbContext.PMMeasure.Where(x => x.StrategeicGoal.PMReviewId == performanceReviewId &&
                                     x.DateDeleted == null && x.LineManagerScore <= 0).Include(x => x.StrategeicGoal).ToList();
            if (totalMeasures.Count > 0) {
                return RedirectToAction("ScoreReview",
                            new {
                                performanceReviewId = performanceReviewId,
                                processingStatus = false, message = "Please capture all review score before concluding the Performance review!!"
                            });
            }

            if (totalMeasures.Count == 0) {
                if (CreatePMReviewProgressStatus(6, (int)performanceReviewId, managerUsername)) {
                    status = true;
                    processingMessage = "Successfully completed Performance Review Scoring";
                }
                else {
                    status = false;
                    processingMessage = "An error has occurred while trying to conclude the Performance Review!!! " +
                                        "Please try again.";
                }
            }
            
            return RedirectToAction("ScoreReview",
                            new {
                                performanceReviewId = performanceReviewId,
                                processingStatus = status, message = processingMessage
                            });
        }

        private decimal GetMaximumRating() {
            return _dbContext.ScoreRating.Max(x => x.MaxScore);
        }

        [HttpGet]
        public ActionResult ViewReviewResult(int? performanceReviewId) {
            if (performanceReviewId == null)
                return RedirectToAction("ManageReview", new { username = User.Identity.Name });

            var reviewContents = GetReviewMeasures((int)performanceReviewId);
            var reviewAverageScore = GetReviewAverageScore(reviewContents);

            var modelView = new ViewReviewResultModel() {
                Username = User.Identity.Name,
                AverageReviewScore = reviewAverageScore,
                ReviewMeasures = reviewContents,
                ReviewRating = GetReviewRating(reviewAverageScore)
            };
            return View(modelView);
        }

        private string GetReviewRating(decimal reviewAverageScore) {
            var result = _dbContext.ScoreRating.Where(x => reviewAverageScore >= x.MinScore).
                         Where(x => reviewAverageScore < x.MaxScore).FirstOrDefault();
            if (result == null)
                return "UNKNOWN RATING";
            return result.Rating.ToUpper();
        }

        private decimal GetReviewAverageScore(List<PerformanceReviewScoringContent> reviewContents) {
            var score = reviewContents.Sum(x => x.ManagerScore * (x.MeasureWeight / 100));
            return score;
        }
    }
}