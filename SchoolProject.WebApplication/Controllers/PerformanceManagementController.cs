using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SchoolProject.WebApplication.Models;
using SchoolProject.WebApplication.ServiceManager.Interface;
using SchoolProject.WebApplication.ViewModels;

namespace SchoolProject.WebApplication.Controllers
{
    [Authorize]
    [HandleError]
    public class PerformanceManagementController : Controller
    {
        private readonly IAdmininstrationManager _iAdminstrationManager;
        private ApplicationDatabaseContext _dbContext;


        public PerformanceManagementController(IAdmininstrationManager iAdmininstrationManager) {
            _iAdminstrationManager = iAdmininstrationManager;
            _dbContext = new ApplicationDatabaseContext();
        }

        [HttpGet]
        public ActionResult ManageReview(string username) {
            return View();
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
                  ProcessingStatusMessage = "Your Line Manager is not yet added, please ask System Administrator to add your Line Manager."
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
    }
}