using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SchoolProject.WebApplication.Models;
using SchoolProject.WebApplication.ServiceManager.Interface;
using SchoolProject.WebApplication.ViewModels;

namespace SchoolProject.WebApplication.Controllers
{
    public class AdministrationController : _BaseController
    {
        private readonly IAdmininstrationManager _iAdminstrationManager;
        private ApplicationDatabaseContext dbContext;
        public AdministrationController(IAdmininstrationManager iAdmininstrationManager) {
            _iAdminstrationManager = iAdmininstrationManager;
            dbContext = new ApplicationDatabaseContext();
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

        [HttpGet]
        public ActionResult CreateEmployee() {
            if (TempData["viewModelEmployee"] != null) {
                return View((RegisterEmployee)TempData["viewModelEmployee"]);
            }
            else {
                var employee = new RegisterEmployee();
                employee.Team = GetJobTeams();
                employee.JobGrade = GetJobGrades();
                employee.FormMode = FormModeOption.CREATE;
                return View(employee);
            }
        }

        [HttpGet]
        public ActionResult UpdateEmployee(int? employeeRecordId) {
            if (employeeRecordId == null)
                return RedirectToAction("CreateEmployee");
            var dbEmployee = _iAdminstrationManager.FindEmployee((int)employeeRecordId);
            var viewModelEmployee = new RegisterEmployee() {
                EmployeeRecordId = dbEmployee.EmployeeRecordId,
                EmployeeCode = dbEmployee.EmployeeCode,
                NetworkUsername = dbEmployee.NetworkUsername,
                EmployeeName = dbEmployee.Name,
                EmployeeSurname = dbEmployee.Surname,
                JobGradeId = dbEmployee.JobGradeId,
                TeamId = dbEmployee.TeamId,
                JobGrade = GetJobGrades(),
                Team = GetJobTeams(),
                ProcessingStatusMessage = string.Empty,
                FormMode = FormModeOption.EDIT
            };
            TempData["viewModelEmployee"] = viewModelEmployee;
            return RedirectToAction("CreateEmployee");
        }

        [HttpGet]
        public ActionResult RemoveEmployee(int? employeeRecordId) {
            if (employeeRecordId == null)
                return RedirectToAction("CreateEmployee");

            var dbEmployee = _iAdminstrationManager.FindEmployee((int)employeeRecordId);
            var viewModelEmployee = new RegisterEmployee() {
                EmployeeRecordId = dbEmployee.EmployeeRecordId,
                EmployeeCode = dbEmployee.EmployeeCode,
                NetworkUsername = dbEmployee.NetworkUsername,
                EmployeeName = dbEmployee.Name,
                EmployeeSurname = dbEmployee.Surname,
                JobGradeId = dbEmployee.JobGradeId,
                TeamId = dbEmployee.TeamId,
                JobGrade = GetJobGrades(),
                Team = GetJobTeams(),
                ProcessingStatusMessage = string.Empty,
                FormMode = FormModeOption.DELETE
            };
            TempData["viewModelEmployee"] = viewModelEmployee;
            return RedirectToAction("CreateEmployee");
        }

        [HttpPost]
        public ActionResult CreateEmployee(RegisterEmployee registerEmployee) {

            if (ModelState.IsValid) {
                switch (registerEmployee.FormMode) {
                    case FormModeOption.CREATE: {
                            if (!EmployeeExist(registerEmployee.EmployeeCode)) {
                                var employee = TransformEmployee(registerEmployee);
                                dbContext.StructureEmployee.Add(employee);
                                dbContext.SaveChanges();

                                var createNew = new RegisterEmployee();
                                createNew.Team = GetJobTeams();
                                createNew.JobGrade = GetJobGrades();
                                createNew.FormMode = FormModeOption.CREATE;
                                createNew.ProcessingStatus = true;
                                createNew.ProcessingStatusMessage = string.Format("Successfully added employee {0} - {1} {2}",
                                                             registerEmployee.EmployeeCode, registerEmployee.EmployeeName,
                                                             registerEmployee.EmployeeSurname);
                                TempData["viewModelEmployee"] = createNew;
                                return RedirectToAction("CreateEmployee");
                            }
                            else {
                                ModelState.AddModelError(string.Empty, "Employee aready exist...");
                                break;
                            }
                        }
                    case FormModeOption.EDIT: {
                            var employee = _iAdminstrationManager.FindEmployee(registerEmployee.EmployeeRecordId);
                            employee.JobGradeId = registerEmployee.JobGradeId;
                            employee.TeamId = registerEmployee.TeamId;
                            employee.Name = registerEmployee.EmployeeName;
                            employee.Surname = registerEmployee.EmployeeSurname;
                            employee.NetworkUsername = registerEmployee.NetworkUsername;
                            employee.DateModified = DateTime.Now;
                            employee.ModifiedBy = "System";
                            dbContext.SaveChanges();
                            //DbEntityEntry entry = dbContext.Entry(employee);
                            //if (entry.State == EntityState.Detached) {
                            //    entry.State = EntityState.Modified;
                            //    dbContext.SaveChanges();
                            //}
                            
                            var createNew = new RegisterEmployee();
                            createNew.Team = GetJobTeams();
                            createNew.JobGrade = GetJobGrades();
                            createNew.FormMode = FormModeOption.CREATE;
                            createNew.ProcessingStatus = true;
                            createNew.ProcessingStatusMessage = string.Format("Successfully updated employee: {0} - {1} {2}",
                                                             registerEmployee.EmployeeCode, registerEmployee.EmployeeName,
                                                             registerEmployee.EmployeeSurname);
                            TempData["viewModelEmployee"] = createNew;
                            return RedirectToAction("CreateEmployee");
                        }
                    case FormModeOption.DELETE: {
                            var employee = _iAdminstrationManager.FindEmployee(registerEmployee.EmployeeRecordId);
                            employee.DeletedBy = "System";
                            employee.DateDeleted = DateTime.Now;
                            employee.StatusId = 2; //Deleted Status Id
                            dbContext.SaveChanges();
                            //DbEntityEntry entry = dbContext.Entry(employee);
                            //if (entry.State == EntityState.Detached) {
                            //    entry.State = EntityState.Modified;
                            //    dbContext.SaveChanges();
                            //}

                            var createNew = new RegisterEmployee();
                            createNew.Team = GetJobTeams();
                            createNew.JobGrade = GetJobGrades();
                            createNew.FormMode = FormModeOption.CREATE;
                            createNew.ProcessingStatus = true;
                            createNew.ProcessingStatusMessage = string.Format("Successfully remove employee: {0} - {1} {2}",
                                                             registerEmployee.EmployeeCode, registerEmployee.EmployeeSurname,
                                                             registerEmployee.EmployeeName);
                            TempData["viewModelEmployee"] = createNew;
                            return RedirectToAction("CreateEmployee");
                        }
                    default: {
                            registerEmployee.ProcessingStatus = false;
                            registerEmployee.ProcessingStatusMessage = "Unknown form processing Mode. the following are supported ModesS: Create, Edit, Delete and Details";
                            break;
                        }
                }
            }
            return View(registerEmployee);
        }

        public StructureEmployee TransformEmployee(RegisterEmployee registeremployee) {
            var employee = new StructureEmployee() {
                EmployeeCode = registeremployee.EmployeeCode,
                TeamId = registeremployee.TeamId,
                JobGradeId = registeremployee.JobGradeId,
                NetworkUsername = registeremployee.NetworkUsername,
                Name = registeremployee.EmployeeName,
                Surname = registeremployee.EmployeeSurname,
                StatusId = 1,
                DateCreated = DateTime.Now,
                CreatedBy = "System"
            };
            return (employee);
        }

        private bool EmployeeExist(string employeeCode) {
            var getEmployee = dbContext.StructureEmployee.FirstOrDefault(x => string.Compare(x.EmployeeCode, employeeCode, true) == 0
                                                                         && x.StatusId == 1);
            if (getEmployee == null)
                return false;
            return true;
        }

        public List<SelectionOptions> GetJobGrades() {
            var results = new List<SelectionOptions>();
            _iAdminstrationManager.GetActiveJobGrades().ForEach(x => {
                results.Add(new SelectionOptions {
                    DisplayText = x.JobGradeName,
                    ValueText = x.JodGradeId.ToString()
                });
            });
            return (results);
        }

        [HttpGet]
        public ActionResult LinkEmployeeToManager() {
            if (TempData["viewModelLinkEmployeeManager"] != null) {
                return View((LinkEmployeeManager)TempData["viewModelLinkEmployeeManager"]);
            }
            else {
                var linkEmployeeManager = new LinkEmployeeManager();
                linkEmployeeManager.Employees = GetEmployees();
                linkEmployeeManager.Managers = GetManagers();
                linkEmployeeManager.DocumentTypes = GetDocumentTypes();
                linkEmployeeManager.FormMode = FormModeOption.CREATE;
                return View(linkEmployeeManager);
            }
        }

        [HttpGet]
        public ActionResult UpdateReportingStructure(int? employeeReportingRecordId, FormModeOption? formMode) {
            if (employeeReportingRecordId == null)
                return RedirectToAction("LinkEmployeeToManager");

            var mode = FormModeOption.EDIT;
            if (formMode == FormModeOption.DELETE)
                mode = FormModeOption.DELETE;

            var dbEmployeeLink = dbContext.PMReviewReportingStructure.Find((int)employeeReportingRecordId);
            var viewModelLinkEmployee = new LinkEmployeeManager() {
                EmployeeReportingRecordId = dbEmployeeLink.ReviewReportingStructureId,
                EmployeeRecordId = dbEmployeeLink.MemberId,
                ManagerRecordId = dbEmployeeLink.ManagerId,
                DocumentTypeId = dbEmployeeLink.DocumentTypeId,
                ProcessingStatusMessage = string.Empty,
                FormMode = mode,
                Employees = GetEmployees(),
                Managers = GetManagers(),
                DocumentTypes = GetDocumentTypes()
            };
            TempData["viewModelLinkEmployeeManager"] = viewModelLinkEmployee;
            return RedirectToAction("LinkEmployeeToManager");
        }

        public ActionResult ManageEmployeeManager(LinkEmployeeManager linkEmployeeManager) {
            if (ModelState.IsValid) {
                switch (linkEmployeeManager.FormMode) {
                    case FormModeOption.CREATE: {
                            if (!EmployeeManagerLinkExist(linkEmployeeManager.EmployeeRecordId, linkEmployeeManager.ManagerRecordId)) {
                                var link = TransformEmployeeManagerLink(linkEmployeeManager);
                                dbContext.PMReviewReportingStructure.Add(link);
                                dbContext.SaveChanges();
                                var employeeName = linkEmployeeManager.Employees.FirstOrDefault(x => x.ValueText == linkEmployeeManager.EmployeeRecordId.ToString()).DisplayText;
                                var managerName = linkEmployeeManager.Managers.FirstOrDefault(x => x.ValueText == linkEmployeeManager.ManagerRecordId.ToString()).DisplayText;

                                var createNew = new LinkEmployeeManager();
                                createNew.Employees = GetEmployees();
                                createNew.Managers = GetManagers();
                                createNew.DocumentTypes = GetDocumentTypes();
                                createNew.ProcessingStatus = true;
                                createNew.ProcessingStatusMessage = string.Format("Successfully linked employee {0} to manager {1} ",
                                                             employeeName, managerName);
                                TempData["viewModelLinkEmployeeManager"] = createNew;
                                return RedirectToAction("LinkEmployeeToManager");
                            }
                            else {                               
                               linkEmployeeManager.ProcessingStatusMessage = "Employee Manager linking aready exist...";
                                linkEmployeeManager.ProcessingStatus = false;
                                break;
                            }
                        }
                    case FormModeOption.EDIT: {
                            var link = _iAdminstrationManager.FindReviewReportingStructure(linkEmployeeManager.EmployeeReportingRecordId);
                            link.ManagerId = linkEmployeeManager.ManagerRecordId;
                            link.MemberId = linkEmployeeManager.EmployeeRecordId;
                            link.DocumentTypeId = linkEmployeeManager.DocumentTypeId;
                            link.DateModified = DateTime.Now;
                            link.ModifiedBy = "System";
                            dbContext.SaveChanges();
                            //DbEntityEntry entry = dbContext.Entry(link);
                            //if (entry.State == EntityState.Detached) {
                            //    entry.State = EntityState.Modified;
                            //    dbContext.SaveChanges();
                            //}

                            var employeeName = linkEmployeeManager.Employees.FirstOrDefault(x => x.ValueText == linkEmployeeManager.EmployeeRecordId.ToString()).DisplayText;
                            var managerName = linkEmployeeManager.Managers.FirstOrDefault(x => x.ValueText == linkEmployeeManager.ManagerRecordId.ToString()).DisplayText;

                            var createNew = new LinkEmployeeManager();
                            createNew.Employees = GetEmployees();
                            createNew.Managers = GetManagers();
                            createNew.DocumentTypes = GetDocumentTypes();
                            createNew.FormMode = FormModeOption.CREATE;
                            createNew.ProcessingStatus = true;
                            createNew.ProcessingStatusMessage = string.Format("Successfully updated employee {0} link to manager {1} ",
                                                         employeeName, managerName);
                            TempData["viewModelLinkEmployeeManager"] = createNew;
                            return RedirectToAction("LinkEmployeeToManager");
                        }
                    case FormModeOption.DELETE: {
                            var link = _iAdminstrationManager.FindReviewReportingStructure(linkEmployeeManager.EmployeeReportingRecordId);
                            link.DeletedBy = "System";
                            link.DateDeleted = DateTime.Now;
                            link.StatusId = 2; //Deleted Status Id
                            dbContext.SaveChanges();
                            //DbEntityEntry entry = dbContext.Entry(link);
                            //if (entry.State == EntityState.Detached) {
                            //    entry.State = EntityState.Modified;
                            //    dbContext.SaveChanges();
                            //}

                            var employeeName = linkEmployeeManager.Employees.FirstOrDefault(x => x.ValueText == linkEmployeeManager.EmployeeRecordId.ToString()).DisplayText;
                            var managerName = linkEmployeeManager.Managers.FirstOrDefault(x => x.ValueText == linkEmployeeManager.ManagerRecordId.ToString()).DisplayText;

                            var createNew = new LinkEmployeeManager();
                            createNew.Employees = GetEmployees();
                            createNew.Managers = GetManagers();
                            createNew.DocumentTypes = GetDocumentTypes();
                            createNew.FormMode = FormModeOption.CREATE;
                            createNew.ProcessingStatus = true;
                            createNew.ProcessingStatusMessage = string.Format("Successfully removed employee {0} link to manager {1} ",
                                                         employeeName, managerName);
                            TempData["viewModelLinkEmployeeManager"] = createNew;
                            return RedirectToAction("LinkEmployeeToManager");
                        }
                    default: {
                            linkEmployeeManager.ProcessingStatus = false;
                            linkEmployeeManager.ProcessingStatusMessage = "Unknown form processing Mode. the following are supported Modes: Create, Edit, Delete and Details";
                            break;
                        }
                }
            }
            TempData["viewModelLinkEmployeeManager"] = linkEmployeeManager;
            return RedirectToAction("LinkEmployeeToManager");
        }

        public PMReviewReportingStructure TransformEmployeeManagerLink(LinkEmployeeManager linkEmployeeManager) {
            var link = new PMReviewReportingStructure() {
                MemberId = linkEmployeeManager.EmployeeRecordId,
                ManagerId = linkEmployeeManager.ManagerRecordId,
                DocumentTypeId = linkEmployeeManager.DocumentTypeId,
                StatusId = 1,
                DateCreated = DateTime.Now,
                CreatedBy = "System"
            };
            return (link);
        }

        private bool EmployeeManagerLinkExist(int employeeRecordId, int managerRecordId) {
            var getEmployee = dbContext.PMReviewReportingStructure.FirstOrDefault(x => x.MemberId == employeeRecordId &&
                                                                       x.ManagerId == managerRecordId   && x.StatusId == 1);
            if (getEmployee == null)
                return false;
            return true;
        }

        public List<SelectionOptions> GetEmployees() {
            var resulst = new List<SelectionOptions>();
            //var employeesList = new List<string>() {
            //    "F1".ToUpper(),
            //    "E1".ToUpper(),
            //    "E3".ToUpper()
            //};

            var employees = dbContext.StructureEmployee.Include(x => x.JobGrade).Where(x => x.StatusId == 1); // && 
                                                              //employeesList.Contains(x.JobGrade.JobGradeCode.ToUpper()));

            employees.ToList().ForEach(x => {
                resulst.Add(new SelectionOptions() {
                    DisplayText = string.Format("{0} - {1} {2} ({3})", x.EmployeeCode, x.Name, x.Surname, x.JobGrade.JobGrade),
                    ValueText = x.EmployeeRecordId.ToString()
                });
            });

            return (resulst);
        }

        public List<SelectionOptions> GetManagers() {
            var resulst = new List<SelectionOptions>();
            var employeesList = new List<string>() {
                "F1".ToUpper(),
                "E1".ToUpper(),
                "E3".ToUpper()
            };

            var managers = dbContext.StructureEmployee.Include(x => x.JobGrade).Where(x => x.StatusId == 1 &&
                                                              !employeesList.Contains(x.JobGrade.JobGradeCode.ToUpper()));

            managers.ToList().ForEach(x => {
                resulst.Add(new SelectionOptions() {
                    DisplayText = string.Format("{0} - {1} {2} ({3})", x.EmployeeCode, x.Name, x.Surname, x.JobGrade.JobGrade),
                    ValueText = x.EmployeeRecordId.ToString()
                });
            });

            return (resulst);
        }

        public List<SelectionOptions> GetDocumentTypes() {
            var results = new List<SelectionOptions>();
            dbContext.PMDocumentType.Where(x => x.StatusId == 1).ToList().ForEach(x => {
                results.Add(new SelectionOptions() {
                    DisplayText = x.DocumentTypeName,
                    ValueText = x.DocumentTypeId.ToString()
                });
            });
            return (results);
        }

        public List<SelectionOptions> GetJobTeams() {
            var results = new List<SelectionOptions>();
            _iAdminstrationManager.GetActiveStructureTeam().ForEach(x => {
                results.Add(new SelectionOptions {
                    DisplayText = x.TeamName,
                    ValueText = x.TeamId.ToString()
                });
            });
            return (results);
        }

        public ActionResult StrategicGoals() {
            var results = dbContext.StrategicGoal.Include(x => x.Status).Where(x => x.StatusId == 1 && x.DateDeleted == null).ToList();
            return View(results);
        }

        [HttpGet]
        public ActionResult CreateStrategicGoal() {
            if (TempData["viewModelStrategicGoal"] != null) {
                return View("ManageStrategicgoal",(RegisterStrategicGoal)TempData["viewModelStrategicGoal"]);
            }
            else {
                var goal = new RegisterStrategicGoal();
                goal.FormMode = FormModeOption.CREATE;
                return View("ManageStrategicgoal",goal);
            }
        }

        [HttpGet]
        public ActionResult ManageStrategicGoal(int? goalId, FormModeOption? formMode) {
            if (goalId == null)
                return RedirectToAction("CreateStrategicGoal");

            var mode = FormModeOption.EDIT;
            if (formMode == FormModeOption.DELETE)
                mode = FormModeOption.DELETE;

            var dbGoal = dbContext.StrategicGoal.Find((int)goalId);
            var goal = new RegisterStrategicGoal() {
                StrategicGoalId = dbGoal.StrategicGoalId,
                StrategicGoalCode = dbGoal.StrategicGoalCode,
                StrategicGoalName = dbGoal.StrategicGoalName,
                ProcessingStatusMessage = string.Empty,
                FormMode = mode,
            };
            TempData["viewModelStrategicGoal"] = goal;
            return RedirectToAction("CreateStrategicGoal");
        }

        [HttpPost]
        public ActionResult ManageStrategicGoal(RegisterStrategicGoal goal) {
            if (ModelState.IsValid) {
                switch (goal.FormMode) {
                    case FormModeOption.CREATE: {
                            if (!StrategicGoalCodeExist(goal.StrategicGoalCode)) {
                                if (!StrategicGoalNameExist(goal.StrategicGoalName)) {
                                    var transformedGoal = TransformStrategicGoal(goal);
                                    dbContext.StrategicGoal.Add(transformedGoal);
                                    dbContext.SaveChanges();
                                    
                                    var createNew = new RegisterStrategicGoal();
                                    createNew.FormMode = FormModeOption.CREATE;
                                    createNew.ProcessingStatus = true;
                                    createNew.ProcessingStatusMessage = string.Format("Successfully added Strategic Goal:{0} - {1} ",
                                                                 goal.StrategicGoalCode, goal.StrategicGoalName);
                                    TempData["viewModelStrategicGoal"] = createNew;
                                    return RedirectToAction("CreateStrategicGoal");
                                }
                                else {
                                    ModelState.AddModelError(string.Empty, "The Strategic Goal Name is alreay exist...");
                                    break;
                                }
                            }
                            else {
                                ModelState.AddModelError(string.Empty, "The Strategic Goal Code is alreay used...");
                                break;
                            }
                        }
                    case FormModeOption.EDIT: {
                            var dbGoal = _iAdminstrationManager.FindStrategicGoal(goal.StrategicGoalId);

                            dbGoal.StrategicGoalName = goal.StrategicGoalName;
                            dbGoal.DateModified = DateTime.Now;
                            dbGoal.ModifiedBy = "System";
                            dbContext.SaveChanges();
                            //DbEntityEntry entry = dbContext.Entry(dbGoal);
                            //if (entry.State == EntityState.Detached) {
                            //    entry.State = EntityState.Modified;
                            //    dbContext.SaveChanges();
                            //}

                            var createNew = new RegisterStrategicGoal();
                            createNew.FormMode = FormModeOption.CREATE;
                            createNew.ProcessingStatus = true;
                            createNew.ProcessingStatusMessage = string.Format("Successfully updated Strategic Goal: {0} - {1} ",
                                                         goal.StrategicGoalCode, goal.StrategicGoalName);
                            TempData["viewModelStrategicGoal"] = createNew;
                            return RedirectToAction("CreateStrategicGoal");
                        }
                    case FormModeOption.DELETE: {
                            var dbGoal = _iAdminstrationManager.FindStrategicGoal(goal.StrategicGoalId);
                            dbGoal.DeletedBy = "System";
                            dbGoal.DateDeleted = DateTime.Now;
                            dbGoal.StatusId = 4; //Deleted Status Id
                            dbContext.SaveChanges();
                            //DbEntityEntry entry = dbContext.Entry(dbGoal);
                            //if (entry.State == EntityState.Detached) {
                            //    entry.State = EntityState.Modified;
                            //    dbContext.SaveChanges();
                            //}

                            var createNew = new RegisterStrategicGoal();
                            createNew.FormMode = FormModeOption.CREATE;
                            createNew.ProcessingStatus = true;
                            createNew.ProcessingStatusMessage = string.Format("Successfully removed Strategic Goal: {0} - {1} ",
                                                         goal.StrategicGoalCode, goal.StrategicGoalName);
                            TempData["viewModelStrategicGoal"] = createNew;
                            return RedirectToAction("CreateStrategicGoal");
                        }
                    default: {
                            goal.ProcessingStatus = false;
                            goal.ProcessingStatusMessage = "Unknown form processing Mode. the following are supported Modes: Create, Edit, Delete and Details";
                            break;
                        }
                }
            }
            return View(goal);
        }

        private AdminStrategicGoal TransformStrategicGoal(RegisterStrategicGoal goal) {
            var newGoal = new AdminStrategicGoal() {
               StrategicGoalCode = goal.StrategicGoalCode.Replace(" ","").Trim().TrimEnd().TrimStart(),
               StrategicGoalName = goal.StrategicGoalName,
               DefaultOverallWeight = 0.00M,
                StatusId = 1,
                DateCreated = DateTime.Now,
                CreatedBy = "System"
            };
            return (newGoal);
        }

        private bool StrategicGoalCodeExist(string goalCode) {
            var goal = dbContext.StrategicGoal.FirstOrDefault(x => string.Compare(x.StrategicGoalCode,goalCode,true) == 0 &&
                                                                     x.StatusId == 1);
            if (goal == null)
                return false;
            return true;
        }

        private bool StrategicGoalNameExist(string goalName) {
            var name = dbContext.StrategicGoal.FirstOrDefault(x => string.Compare(x.StrategicGoalName, goalName, true) == 0
                                                                     && x.StatusId == 1);
            if (name == null)
                return false;
            return true;
        }

        [HttpGet]
        public ActionResult CreatePerformaceYear() {
            if (TempData["viewModelPerformaceYear"] != null) {
                return View("ManagePerformanceYear", (RegisterPerformanceYear)TempData["viewModelPerformaceYear"]);
            }
            else {
                var performanceYear = new RegisterPerformanceYear();
                performanceYear.FormMode = FormModeOption.CREATE;
                return View("ManagePerformanceYear", performanceYear);
            }
        }

        [HttpGet]
        public ActionResult ManagePerformanceYear(int? performanceYearId, FormModeOption? formMode) {
            if (performanceYearId == null)
                return RedirectToAction("CreatePerformaceYear");

            var mode = FormModeOption.EDIT;
            if (formMode == FormModeOption.DELETE)
                mode = FormModeOption.DELETE;

            var dbperfromanceYear = dbContext.PerformanceYear.Find((int)performanceYearId);
            var performanceYear = new RegisterPerformanceYear() {
                PerformanceYearId = dbperfromanceYear.PerformanceYearId,
                PerformanceYearName = dbperfromanceYear.PerformanceYearName,
                StartDate = dbperfromanceYear.StartDate.ToString("MMMM dd yyyy"),
                EndDate = dbperfromanceYear.EndDate.ToString("MMMM dd yyyy"),
                ProcessingStatusMessage = string.Empty,
                FormMode = mode,
            };
            TempData["viewModelPerformaceYear"] = performanceYear;
            return RedirectToAction("CreatePerformaceYear");
        }

        [HttpPost]
        public ActionResult ManagePerformanceYear(RegisterPerformanceYear performanceYear) {
            if (ModelState.IsValid) {
                switch (performanceYear.FormMode) {
                    case FormModeOption.CREATE: {
                            if (!PerformanceYearExist(performanceYear)) {
                                    var transformedYear = TransformPerformanceYear(performanceYear);
                                    dbContext.PerformanceYear.Add(transformedYear);
                                    dbContext.SaveChanges();

                                    var createNew = new RegisterPerformanceYear();
                                    createNew.FormMode = FormModeOption.CREATE;
                                    createNew.ProcessingStatus = true;
                                    createNew.ProcessingStatusMessage = string.Format("Successfully added Performanve Year:{0} ( {1} - {2})",
                                                                 performanceYear.PerformanceYearName, 
                                                                 DateTime.Parse(performanceYear.StartDate).ToString("MMMM dd yyyy"),
                                                                 DateTime.Parse(performanceYear.EndDate).ToString("MMMM dd yyyy"));
                                    TempData["viewModelPerformaceYear"] = createNew;
                                    return RedirectToAction("CreatePerformaceYear");
                            }
                            else {
                                ModelState.AddModelError(string.Empty, "Performance Year Exist...");
                                break;
                            }
                        }
                    case FormModeOption.EDIT: {
                            var dbYear = _iAdminstrationManager.FindPerformanceYear(performanceYear.PerformanceYearId);

                            dbYear.PerformanceYearName = performanceYear.PerformanceYearName;
                            dbYear.StartDate = DateTime.Parse(performanceYear.StartDate).Date;
                            dbYear.EndDate = DateTime.Parse(performanceYear.EndDate).Date;
                            dbYear.DateModified = DateTime.Now;
                            dbYear.ModifiedBy = "System";
                            dbContext.SaveChanges();
                            //DbEntityEntry entry = dbContext.Entry(dbYear);
                            //if (entry.State == EntityState.Detached) {
                            //    entry.State = EntityState.Modified;
                            //    dbContext.SaveChanges();
                            //}

                            var createNew = new RegisterPerformanceYear();
                            createNew.FormMode = FormModeOption.CREATE;
                            createNew.ProcessingStatus = true;
                            createNew.ProcessingStatusMessage = string.Format("Successfully updated Performanve Year:{0} ( {1} - {2})",
                                                                 performanceYear.PerformanceYearName,
                                                                 DateTime.Parse(performanceYear.StartDate).ToString("MMMM dd yyyy"),
                                                                 DateTime.Parse(performanceYear.EndDate).ToString("MMMM dd yyyy"));
                            TempData["viewModelPerformaceYear"] = createNew;
                            return RedirectToAction("CreatePerformaceYear");
                        }
                    case FormModeOption.DELETE: {
                            var dbYear = _iAdminstrationManager.FindPerformanceYear(performanceYear.PerformanceYearId);
                            dbYear.DeletedBy = "Administrator";
                            dbYear.DateDeleted = DateTime.Now;
                            dbYear.StatusId = 4; //Deleted Status Id
                            dbContext.SaveChanges();
                            //DbEntityEntry entry = dbContext.Entry(dbYear);
                            //if (entry.State == EntityState.Detached) {
                            //    entry.State = EntityState.Modified;
                            //    dbContext.SaveChanges();
                            //}

                            var createNew = new RegisterPerformanceYear();
                            createNew.FormMode = FormModeOption.CREATE;
                            createNew.ProcessingStatus = true;
                            createNew.ProcessingStatusMessage = string.Format("Successfully removed Performanve Year:{0} ( {1} - {2})",
                                                                 performanceYear.PerformanceYearName,
                                                                 DateTime.Parse(performanceYear.StartDate).ToString("MMMM dd yyyy"),
                                                                 DateTime.Parse(performanceYear.EndDate).ToString("MMMM dd yyyy"));
                            TempData["viewModelPerformaceYear"] = createNew;
                            return RedirectToAction("CreatePerformaceYear");
                        }
                    default: {
                            performanceYear.ProcessingStatus = false;
                            performanceYear.ProcessingStatusMessage = "Unknown form processing Mode. the following are supported Modes: Create, Edit, Delete and Details";
                            break;
                        }
                }
            }
            return View(performanceYear);
        }

        private AdminPerformanceYear TransformPerformanceYear(RegisterPerformanceYear performanceYear) {
            return (new AdminPerformanceYear() {
                PerformanceYearName = performanceYear.PerformanceYearName,
                StartDate = DateTime.Parse(performanceYear.StartDate).Date,
                EndDate = DateTime.Parse(performanceYear.EndDate).Date,
                StatusId = 1,
                DateCreated = DateTime.Now,
                CreatedBy = "Administrator",
                DateModified = DateTime.Now,
                ModifiedBy = "Administrator"
            });
        }

        private bool PerformanceYearExist(RegisterPerformanceYear performanceYear) {
            var startDate = DateTime.Parse(performanceYear.StartDate);
            var endDate = DateTime.Parse(performanceYear.EndDate);
            var newYear = _iAdminstrationManager.GetPerformanceYears().FirstOrDefault(x => x.StatusId == 1 &&
                                string.Compare(x.PerformanceYearName,performanceYear.PerformanceYearName,true) == 0 &&
                                x.StartDate.Date == startDate && 
                                x.EndDate.Date == endDate);
            return (newYear != null) ? true : false;
        }

        public List<SelectionOptions> GetReviewPeriod() {
            var results = new List<SelectionOptions>();
            _iAdminstrationManager.GetReviewPeriods().Where(x => x.StatusId == 1).ToList().ForEach(x => {
                results.Add(new SelectionOptions {
                    DisplayText = x.ReviewPeriodName,
                    ValueText = x.ReviewPeriodId.ToString()
                });
            });
            return (results);
        }

        public List<SelectionOptions> GetPerformanceYearsSelection() {
            var results = new List<SelectionOptions>();
            _iAdminstrationManager.GetPerformanceYears().Where(x => x.StatusId == 1).ToList().ForEach(x => {
                results.Add(new SelectionOptions {
                    DisplayText = string.Format("{0} - ({1} - {2})", x.PerformanceYearName, 
                                                                     x.StartDate.Date.ToString("MMMM dd yyyy"),
                                                                     x.EndDate.Date.ToString("MMMM dd yyyy")),
                    ValueText = x.PerformanceYearId.ToString()
                });
            });
            return (results);
        }

        [HttpGet]
        public ActionResult CreatePerformanceYearReviewPeriodLink() {
            if (TempData["viewPerformanceYearReviewPeriodLink"] != null) {
                return View("ManagePerformanceYearReviewPeriodLink", (RegisterPerformanceYearReviewPeriodLink)TempData["viewPerformanceYearReviewPeriodLink"]);
            }
            else {
                var reviewYearLink = new RegisterPerformanceYearReviewPeriodLink();
                reviewYearLink.FormMode = FormModeOption.CREATE;
                reviewYearLink.PerformanceYears = GetPerformanceYearsSelection();
                reviewYearLink.ReviewPeriods = GetReviewPeriod();
                return View("ManagePerformanceYearReviewPeriodLink", reviewYearLink);
            }
        }

        [HttpGet]
        public ActionResult ManagePerformanceYearReviewPeriodLink(int? pmReviewPeriodId, FormModeOption? formMode) {
            if (pmReviewPeriodId == null)
                return RedirectToAction("CreatePerformanceYearReviewPeriodLink");

            var mode = FormModeOption.EDIT;
            if (formMode == FormModeOption.DELETE)
                mode = FormModeOption.DELETE;

            var dbperfromanceYearReviewPeriodLink = dbContext.PMReviewPeriod.Find((int)pmReviewPeriodId);
            var newLink = new RegisterPerformanceYearReviewPeriodLink() {
                PerformanceYearId = dbperfromanceYearReviewPeriodLink.PerformanceYearId,
                ReviewPeriodId = dbperfromanceYearReviewPeriodLink.ReviewPeriodId,
                PMReviewPeriodId = dbperfromanceYearReviewPeriodLink.PMReviewPeriodId,
                PerformanceYears = GetPerformanceYearsSelection(),
                ReviewPeriods = GetReviewPeriod(),
                ProcessingStatusMessage = string.Empty,
                FormMode = mode,
            };
            TempData["viewPerformanceYearReviewPeriodLink"] = newLink;
            return RedirectToAction("CreatePerformanceYearReviewPeriodLink");
        }

        [HttpPost]
        public ActionResult ManagePerformanceYearReviewPeriodLink(RegisterPerformanceYearReviewPeriodLink performanceYearReviewLink) {
            if (ModelState.IsValid) {
                switch (performanceYearReviewLink.FormMode) {
                    case FormModeOption.CREATE: {
                            if (!PerformanceYearReviewPeriodExist(performanceYearReviewLink)) {
                                var transformedYear = TransformPerformanceYearReviewPeriodLink(performanceYearReviewLink);
                                dbContext.PMReviewPeriod.Add(transformedYear);
                                dbContext.SaveChanges();

                                var createNew = new RegisterPerformanceYearReviewPeriodLink();
                                createNew.PerformanceYears = GetPerformanceYearsSelection();
                                createNew.ReviewPeriods = GetReviewPeriod();
                                createNew.FormMode = FormModeOption.CREATE;
                                createNew.ProcessingStatus = true;
                                createNew.ProcessingStatusMessage = string.Format("Successfully added Performanve Year Review Link :{0} - {1}",
                                                             GetPerformanceYearsSelection().FirstOrDefault(x => int.Parse(x.ValueText) == performanceYearReviewLink.PerformanceYearId).DisplayText,
                                                             GetReviewPeriod().FirstOrDefault(x => int.Parse(x.ValueText) == performanceYearReviewLink.ReviewPeriodId).DisplayText);
                                TempData["viewPerformanceYearReviewPeriodLink"] = createNew;
                                return RedirectToAction("CreatePerformanceYearReviewPeriodLink");
                            }
                            else {
                                ModelState.AddModelError(string.Empty, "Performance Year Review Period Link Exist...");
                                break;
                            }
                        }
                    case FormModeOption.EDIT: {
                            var dbYearLink = _iAdminstrationManager.FindPMReview(performanceYearReviewLink.PMReviewPeriodId); 
                            dbYearLink.ReviewPeriodId = performanceYearReviewLink.ReviewPeriodId;
                            dbYearLink.PerformanceYearId = performanceYearReviewLink.PerformanceYearId;
                            dbYearLink.DateModified = DateTime.Now;
                            dbYearLink.ModifiedBy = "System";
                            dbContext.SaveChanges();
                            //var update = _iAdminstrationManager.UpdatePMReview(dbYearLink);

                            var createNew = new RegisterPerformanceYearReviewPeriodLink();
                            createNew.PerformanceYears = GetPerformanceYearsSelection();
                            createNew.ReviewPeriods = GetReviewPeriod();
                            createNew.FormMode = FormModeOption.CREATE;
                            createNew.ProcessingStatus = true;
                            createNew.ProcessingStatusMessage = string.Format("Successfully updated Performanve Year Review Link :{0} - {1}",
                                                             GetPerformanceYearsSelection().FirstOrDefault(x => int.Parse(x.ValueText) == performanceYearReviewLink.PerformanceYearId).DisplayText,
                                                             GetReviewPeriod().FirstOrDefault(x => int.Parse(x.ValueText) == performanceYearReviewLink.ReviewPeriodId).DisplayText);
                            TempData["viewPerformanceYearReviewPeriodLink"] = createNew;
                            return RedirectToAction("CreatePerformanceYearReviewPeriodLink");
                        }
                    case FormModeOption.DELETE: {
                            var dbYearLink = _iAdminstrationManager.FindPMReview(performanceYearReviewLink.PMReviewPeriodId);
                            dbYearLink.DeletedBy = "Administrator";
                            dbYearLink.DateDeleted = DateTime.Now;
                            dbYearLink.StatusId = 4; //Deleted Status Id
                            dbContext.SaveChanges();
                            //var updatePMReviewPeriod = _iAdminstrationManager.DeletePMReview(dbYearLink);

                            var createNew = new RegisterPerformanceYearReviewPeriodLink();
                            createNew.PerformanceYears = GetPerformanceYearsSelection();
                            createNew.ReviewPeriods = GetReviewPeriod();
                            createNew.FormMode = FormModeOption.CREATE;
                            createNew.ProcessingStatus = true;
                            createNew.ProcessingStatusMessage = string.Format("Successfully removed Performanve Year Review Link :{0} - {1}",
                                                             GetPerformanceYearsSelection().FirstOrDefault(x => int.Parse(x.ValueText) == performanceYearReviewLink.PerformanceYearId).DisplayText,
                                                             GetReviewPeriod().FirstOrDefault(x => int.Parse(x.ValueText) == performanceYearReviewLink.ReviewPeriodId).DisplayText);
                            TempData["viewPerformanceYearReviewPeriodLink"] = createNew;
                            return RedirectToAction("CreatePerformanceYearReviewPeriodLink");
                        }
                    default: {
                            performanceYearReviewLink.ProcessingStatus = false;
                            performanceYearReviewLink.ProcessingStatusMessage = "Unknown form processing Mode. the following are supported Modes: Create, Edit, Delete and Details";
                            break;
                        }
                }
            }
            return View(performanceYearReviewLink);
        }

        private PMReviewPeriod TransformPerformanceYearReviewPeriodLink(RegisterPerformanceYearReviewPeriodLink performanceYearReviewLink) {
            return (new PMReviewPeriod() {
                ReviewPeriodId = performanceYearReviewLink.ReviewPeriodId,
                PerformanceYearId = performanceYearReviewLink.PerformanceYearId,
                StatusId = 1,
                DateCreated = DateTime.Now,
                CreatedBy = "Admin",
                ModifiedBy = "Admin",
                DateModified = DateTime.Now
            });
        }

        private bool PerformanceYearReviewPeriodExist(RegisterPerformanceYearReviewPeriodLink performanceYearReviewLink) {
            var checkLink = _iAdminstrationManager.GetLinkedPerformanceYearReviews().FirstOrDefault(x => x.StatusId == 1 &&
                                x.PerformanceYearId == performanceYearReviewLink.PerformanceYearId &&
                                x.ReviewPeriodId == performanceYearReviewLink.ReviewPeriodId);
            return (checkLink != null) ? true : false;
        }


        // GET: Administration
        public ActionResult Index()
        {
            return View();
        }
    }
}