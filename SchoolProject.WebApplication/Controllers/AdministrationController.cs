using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SchoolProject.WebApplication.ServiceManager.Interface;

namespace SchoolProject.WebApplication.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly IAdmininstrationManager _iAdminstrationManager;
        public AdministrationController(IAdmininstrationManager iAdmininstrationManager) {
            _iAdminstrationManager = iAdmininstrationManager;
        }

        public ActionResult Status() {
            var status = _iAdminstrationManager.GetActiveStatuses();
            return View(status);
        }

        public ActionResult DocumentType() {
            var result = _iAdminstrationManager.GetActiveDocumentType();
            return View(result);
        }

        public ActionResult Terms() {
            var terms = _iAdminstrationManager.GetActiveTerms();
            return View(terms);
        }

        public ActionResult JobGrade() {
            var jobGrade = _iAdminstrationManager.GetActiveJobGrades();
            return View(jobGrade);
        }

        public ActionResult PerformanceYear() {
            var performanceYears = _iAdminstrationManager.GetPerformanceYears().Where(x => x.DateDeleted == null && x.StatusId != 4).ToList();
            return View(performanceYears);
        }

        public ActionResult PerformanceYearReviews() {
            var reviewPeriods = _iAdminstrationManager.GetReviewPeriods().Where(x => x.DateDeleted == null && x.StatusId != 4).ToList();
            return View(reviewPeriods);
        }

        public ActionResult PerformanceReviewPeriodLinking() {
            var reviewLinks = _iAdminstrationManager.GetLinkedPerformanceYearReviews();
            return View(reviewLinks);
        }

        public ActionResult OrganisationStructure() {
            var orgStructure = _iAdminstrationManager.GetActiveStructureOrganisation();
            return View(orgStructure);
        }

        public ActionResult Portfolios() {
            var result = _iAdminstrationManager.GetActiveStructurePortfolio();
            return View(result);
        }

        public ActionResult Department() {
            var results = _iAdminstrationManager.GetActiveStructureDepartment();
            return View(results);
        }

        public ActionResult Team() {
            var results = _iAdminstrationManager.GetActiveStructureTeam();
            return View(results);
        }

        public ActionResult ReviewReportingStructure() {
            var results = _iAdminstrationManager.GetPMReviewReportingStructure();
            return View(results);
        }

        public ActionResult Employees() {
            var results = _iAdminstrationManager.GetEmployee();
            return View(results);
        }

        // GET: Administration
        public ActionResult Index()
        {
            return View();
        }
    }
}