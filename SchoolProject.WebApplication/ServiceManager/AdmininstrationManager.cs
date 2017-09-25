using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using EntityFramework.BulkInsert.Extensions;
using Ninject;
using SchoolProject.WebApplication.DTO;
using SchoolProject.WebApplication.Models;
using SchoolProject.WebApplication.Models.Repository;
using SchoolProject.WebApplication.ServiceManager.Interface;

namespace SchoolProject.WebApplication.ServiceManager {
    public class AdmininstrationManager : IAdmininstrationManager {
        private IPerformanceManagmentRepository _pmRepository;
        private INinjectStandardModule _standardModule;
        public AdmininstrationManager(INinjectStandardModule ninjectStandardModules) {
            _standardModule = ninjectStandardModules;
            IKernel kernel = _standardModule.GetStandardModelule();

            _pmRepository = kernel.Get<IPerformanceManagmentRepository>();
        }

        public T InsertItem<T>(T item) where T : class {
            _pmRepository.Insert<T>(item, _pmRepository.GetApplicationDbContext);
            _pmRepository.Commit(_pmRepository.GetApplicationDbContext);
            return item;
        }

        public bool BulkInsertItem<T>(List<T> bulkInsertItems) {
            _pmRepository.GetApplicationDbContext.Configuration.AutoDetectChangesEnabled = false;
            _pmRepository.GetApplicationDbContext.Configuration.ValidateOnSaveEnabled = false;
            var transactionScope = _pmRepository.GetApplicationDbContext.Database.BeginTransaction();
            _pmRepository.GetApplicationDbContext.BulkInsert(bulkInsertItems);
            _pmRepository.GetApplicationDbContext.SaveChanges();
            transactionScope.Commit();
            _pmRepository.GetApplicationDbContext.Configuration.AutoDetectChangesEnabled = true;
            _pmRepository.GetApplicationDbContext.Configuration.ValidateOnSaveEnabled = true;
            return true;
        }

        public bool DeleteItem<T>(T item) where T : class {
            _pmRepository.Delete(item, _pmRepository.GetApplicationDbContext);
            _pmRepository.Commit(_pmRepository.GetApplicationDbContext);
            return true;
        }

        public T UpdateItem<T>(T item) where T : class {
            _pmRepository.Update<T>(item, _pmRepository.GetApplicationDbContext);
            _pmRepository.Commit(_pmRepository.GetApplicationDbContext);
            return item;
        }

        public T FindItem<T>(int id) where T : class {
            return _pmRepository.Find<T>(id, _pmRepository.GetApplicationDbContext);
        }

        public List<T> GetItems<T>() where T : class {
            return _pmRepository.Get<T>(_pmRepository.GetApplicationDbContext).ToList();
        }

        //Portfolios
        public List<Portfolio> GetPortfolios() {
            var portfolios = new List<Portfolio>();
            var results = _pmRepository.Get<StructurePortfolio>(_pmRepository.GetApplicationDbContext).
                          Include(x => x.Status).Include(x => x.Organisation);
            foreach (var item in results) {
                portfolios.Add(new DTO.Portfolio() {
                    PortfolioId = item.PortfolioId,
                    PortfolioCode = item.PortfolioCode,
                    PortfolioName = item.PortfolioName,
                    DeleteDate = item.DateDeleted,
                    StatusId = item.StatusId,
                    StatusName = item.Status.StatusName,
                    OrganisationStructureId = item.StructureOrganisationId,
                    OrganisationStructureName = item.Organisation.OrganisationName,
                    OrganisationStructureCode = item.Organisation.OrganisationCode
                });
            }
            return (portfolios);
        }
        public Portfolio SearchPortfolio(int id) {
            var result = _pmRepository.Find<StructurePortfolio>(id, _pmRepository.GetApplicationDbContext);
            if (result != null) {
                return (new DTO.Portfolio() {
                    PortfolioId = result.PortfolioId,
                    PortfolioCode = result.PortfolioCode,
                    PortfolioName = result.PortfolioName,
                    DeleteDate = result.DateDeleted,
                    StatusId = result.StatusId,
                    StatusName = result.Status.StatusName,
                    OrganisationStructureId = result.StructureOrganisationId,
                    OrganisationStructureName = result.Organisation.OrganisationName,
                    OrganisationStructureCode = result.Organisation.OrganisationCode
                });
            }
            return null;
        }

        //Structure Departments
        public List<Department> GetDepartment() {
            var departments = new List<Department>();
            var results = _pmRepository.Get<StructureDepartment>(_pmRepository.GetApplicationDbContext).
                          Include(x => x.Status).Include(x => x.Portfolio).Include(x => x.Portfolio.Organisation);
            foreach (var item in results) {
                departments.Add(new DTO.Department() {
                    DepartmentId = item.DepartmentId,
                    DepartmentCode = item.DepartmentCode,
                    DepartmentName = item.DepartmentName,
                    PortfolioId = item.PortfolioId,
                    PortfolioCode = item.Portfolio.PortfolioCode,
                    PortfolioName = item.Portfolio.PortfolioName,
                    DeleteDate = item.DateDeleted,
                    StatusId = item.StatusId,
                    StatusName = item.Status.StatusName,
                    OrganisationStructureId = item.Portfolio.Organisation.StructureOrganisationId,
                    OrganisationStructureName = item.Portfolio.Organisation.OrganisationName,
                    OrganisationStructureCode = item.Portfolio.Organisation.OrganisationCode
                });
            }
            return (departments);
        }
        public Department SearchDepartment(int id) {
            var item = _pmRepository.Find<StructureDepartment>(id, _pmRepository.GetApplicationDbContext);
            if (item != null) {
                return (new DTO.Department() {
                    DepartmentId = item.DepartmentId,
                    DepartmentCode = item.DepartmentCode,
                    DepartmentName = item.DepartmentName,
                    PortfolioId = item.PortfolioId,
                    PortfolioCode = item.Portfolio.PortfolioCode,
                    PortfolioName = item.Portfolio.PortfolioName,
                    DeleteDate = item.DateDeleted,
                    StatusId = item.StatusId,
                    StatusName = item.Status.StatusName,
                    OrganisationStructureId = item.Portfolio.Organisation.StructureOrganisationId,
                    OrganisationStructureName = item.Portfolio.Organisation.OrganisationName,
                    OrganisationStructureCode = item.Portfolio.Organisation.OrganisationCode
                });
            }
            return null;
        }

        //Structure Teams
        public List<Team> GetTeam() {
            var teams = new List<Team>();
            var results = _pmRepository.Get<StructureTeam>(_pmRepository.GetApplicationDbContext).
                          Include(x => x.Status).Include(x => x.Department).Include(x => x.Department.Portfolio).
                          Include(x => x.Department.Portfolio.Organisation);
            foreach (var item in results) {
                teams.Add(new DTO.Team() {
                    TeamId = item.TeamId,
                    TeamCode = item.TeamCode,
                    TeamName = item.TeamName,
                    DepartmentId = item.Department.DepartmentId,
                    DepartmentCode = item.Department.DepartmentCode,
                    DepartmentName = item.Department.DepartmentName,
                    PortfolioId = item.Department.Portfolio.PortfolioId,
                    PortfolioCode = item.Department.Portfolio.PortfolioCode,
                    PortfolioName = item.Department.Portfolio.PortfolioName,
                    DeleteDate = item.DateDeleted,
                    StatusId = item.StatusId,
                    StatusName = item.Status.StatusName,
                    OrganisationStructureId = item.Department.Portfolio.Organisation.StructureOrganisationId,
                    OrganisationStructureName = item.Department.Portfolio.Organisation.OrganisationName,
                    OrganisationStructureCode = item.Department.Portfolio.Organisation.OrganisationCode
                });
            }
            return (teams);
        }
        public Team SearchTeam(int id) {
            var item = _pmRepository.Find<StructureTeam>(id, _pmRepository.GetApplicationDbContext);
            if (item != null) {
                return (new DTO.Team() {
                    TeamId = item.TeamId,
                    TeamCode = item.TeamCode,
                    TeamName = item.TeamName,
                    DepartmentId = item.Department.DepartmentId,
                    DepartmentCode = item.Department.DepartmentCode,
                    DepartmentName = item.Department.DepartmentName,
                    PortfolioId = item.Department.Portfolio.PortfolioId,
                    PortfolioCode = item.Department.Portfolio.PortfolioCode,
                    PortfolioName = item.Department.Portfolio.PortfolioName,
                    DeleteDate = item.DateDeleted,
                    StatusId = item.StatusId,
                    StatusName = item.Status.StatusName,
                    OrganisationStructureId = item.Department.Portfolio.Organisation.StructureOrganisationId,
                    OrganisationStructureName = item.Department.Portfolio.Organisation.OrganisationName,
                    OrganisationStructureCode = item.Department.Portfolio.Organisation.OrganisationCode
                });
            }
            return null;
        }

        //Job Grades
        public List<JobGrade> GetJobGrade() {
            var jobGrade = new List<JobGrade>();
            var results = _pmRepository.Get<AdminJobGrade>(_pmRepository.GetApplicationDbContext).
                          Include(x => x.Status);
            foreach (var item in results) {
                jobGrade.Add(new DTO.JobGrade() {
                    JodGradeId = item.JobGradeId,
                    JobGradeCode = item.JobGradeCode,
                    JobGradeName = item.JobGrade,
                    DeleteDate = item.DateDeleted,
                    StatusId = item.StatusId,
                    StatusName = item.Status.StatusName
                });
            }
            return (jobGrade);
        }
        public JobGrade SearchJobGrade(int id) {
            var item = _pmRepository.Find<AdminJobGrade>(id, _pmRepository.GetApplicationDbContext);
            if (item != null) {
                return (new DTO.JobGrade() {
                    JodGradeId = item.JobGradeId,
                    JobGradeCode = item.JobGradeCode,
                    JobGradeName = item.JobGrade,
                    DeleteDate = item.DateDeleted,
                    StatusId = item.StatusId,
                    StatusName = item.Status.StatusName
                });
            }
            return null;
        }

        //Document Type
        public List<DocumentType> GetDocumentType() {
            var documentType = new List<DocumentType>();
            var results = _pmRepository.Get<PMDocumentType>(_pmRepository.GetApplicationDbContext).
                          Include(x => x.Status);
            foreach (var item in results) {
                documentType.Add(new DTO.DocumentType() {
                    DocumentTypeId = item.DocumentTypeId,
                    DocumentTypeCode = item.DocumentTypeCode,
                    DocumentTypeName = item.DocumentTypeName,
                    DeleteDate = item.DateDeleted,
                    StatusId = item.StatusId,
                    StatusName = item.Status.StatusName
                });
            }
            return (documentType);
        }
        public DocumentType SearchDocumentType(int id) {
            var item = _pmRepository.Find<PMDocumentType>(id, _pmRepository.GetApplicationDbContext);
            if (item != null) {
                return (new DTO.DocumentType() {
                    DocumentTypeId = item.DocumentTypeId,
                    DocumentTypeCode = item.DocumentTypeCode,
                    DocumentTypeName = item.DocumentTypeName,
                    DeleteDate = item.DateDeleted,
                    StatusId = item.StatusId,
                    StatusName = item.Status.StatusName
                });
            }
            return null;
        }



        //Status

        public AdminStatus AddStatus(AdminStatus Status) {
            return _pmRepository.Insert(Status, _pmRepository.GetApplicationDbContext);
        }

        public bool DeleteStatus(AdminStatus Status) {
             _pmRepository.Delete(Status, _pmRepository.GetApplicationDbContext);
            return true;
        }

        public AdminStatus UpdateStatus(AdminStatus Status) {
            return _pmRepository.Update(Status, _pmRepository.GetApplicationDbContext);
        }

        public AdminStatus FindStatus(int id) {
            return _pmRepository.Find<AdminStatus>(id, _pmRepository.GetApplicationDbContext);
        }

        public List<AdminStatus> GetStatuses() {
            return _pmRepository.Get<AdminStatus>(_pmRepository.GetApplicationDbContext).ToList();
        }

        public List<AdminStatus> GetActiveStatuses() {
            return GetStatuses().Where(x => x.DateDeleted == null).ToList();
        }

        //Job Grade
        public AdminJobGrade AddJobGrade(AdminJobGrade jobGrade) {
            return _pmRepository.Insert(jobGrade, _pmRepository.GetApplicationDbContext);
        }

        public bool DeleteJobGrade(AdminJobGrade jobGrade) {
             _pmRepository.Delete(jobGrade, _pmRepository.GetApplicationDbContext);
            return true;
        }

        public AdminJobGrade UpdateJobGrade(AdminJobGrade jobGrade) {
            return _pmRepository.Update(jobGrade,_pmRepository.GetApplicationDbContext);
        }

        public JobGrade FindJobGrade(int id) {
            return SearchJobGrade(id);
        }

        public List<JobGrade> GetJobGrades() {
            return GetJobGrade();
        }

        public List<JobGrade> GetActiveJobGrades() {
            return GetJobGrades().Where(x => x.DeleteDate == null).ToList();
        }

        public AdminMeasure AddMeasure(AdminMeasure measure) {
            return _pmRepository.Insert(measure, _pmRepository.GetApplicationDbContext);
        }

        public bool DeleteMeasure(AdminMeasure measure) {
            _pmRepository.Delete(measure, _pmRepository.GetApplicationDbContext);
            return true;
        }

        public AdminMeasure UpdateMeasure(AdminMeasure measure) {
            return _pmRepository.Update(measure, _pmRepository.GetApplicationDbContext);
        }

        public AdminMeasure FindMeasure(int id) {
            return _pmRepository.Find<AdminMeasure>(id, _pmRepository.GetApplicationDbContext);
        }

        public List<AdminMeasure> GetMeasures() {
            return _pmRepository.Get<AdminMeasure>(_pmRepository.GetApplicationDbContext).ToList();
        }

        public List<AdminMeasure> GetActiveMeasures() {
            return GetMeasures().Where(x => x.DateDeleted == null).ToList();
        }

        public AdminObjective AddObjective(AdminObjective objective) {
            return _pmRepository.Insert(objective, _pmRepository.GetApplicationDbContext);
        }

        public bool DeleteObjective(AdminObjective objective) {
            _pmRepository.Delete(objective, _pmRepository.GetApplicationDbContext);
            return true;
        }

        public AdminObjective UpdateObjective(AdminObjective objective) {
            return _pmRepository.Update(objective, _pmRepository.GetApplicationDbContext);
        }

        public AdminObjective FindObjective(int id) {
            return _pmRepository.Find<AdminObjective>(id, _pmRepository.GetApplicationDbContext);
        }

        public List<AdminObjective> GetObjectives() {
            return _pmRepository.Get<AdminObjective>(_pmRepository.GetApplicationDbContext).ToList();
        }

        public List<AdminObjective> GetActiveObjectives() {
            return GetObjectives().Where(x => x.DateDeleted == null).ToList();
        }

        public AdminPerformanceYear AddPerformanceYear(AdminPerformanceYear performanceYear) {
            return _pmRepository.Insert(performanceYear, _pmRepository.GetApplicationDbContext);
        }

        public bool DeletePerformanceYear(AdminPerformanceYear performanceYear) {
            _pmRepository.Delete(performanceYear, _pmRepository.GetApplicationDbContext);
            return true;
        }

        public AdminPerformanceYear UpdatePerformanceYear(AdminPerformanceYear performanceYear) {
            return _pmRepository.Update(performanceYear, _pmRepository.GetApplicationDbContext);
        }

        public AdminPerformanceYear FindPerformanceYear(int id) {
            return _pmRepository.Find<AdminPerformanceYear>(id, _pmRepository.GetApplicationDbContext);
        }

        public List<AdminPerformanceYear> GetPerformanceYears() {
            return _pmRepository.Get<AdminPerformanceYear>(_pmRepository.GetApplicationDbContext).ToList();
        }

        public List<AdminPerformanceYear> GetActivePerformanceYears() {
            return _pmRepository.Get<AdminPerformanceYear>(_pmRepository.GetApplicationDbContext).Where(x => x.DateDeleted == null).ToList();
        }

        public AdminReviewPeriod AddReviewPeriod(AdminReviewPeriod reviewPeriod) {
            return _pmRepository.Insert(reviewPeriod, _pmRepository.GetApplicationDbContext);
        }

        public bool DeleteReviewPeriod(AdminReviewPeriod reviewPeriod) {
            _pmRepository.Delete(reviewPeriod, _pmRepository.GetApplicationDbContext);
            return true;
        }

        public AdminReviewPeriod UpdateReviewPeriod(AdminReviewPeriod reviewPeriod) {
            return _pmRepository.Update(reviewPeriod, _pmRepository.GetApplicationDbContext);
        }

        public AdminReviewPeriod FindReviewPeriod(int id) {
            return _pmRepository.Find<AdminReviewPeriod>(id, _pmRepository.GetApplicationDbContext);
        }

        public List<AdminReviewPeriod> GetReviewPeriods() {
            return _pmRepository.Get<AdminReviewPeriod>(_pmRepository.GetApplicationDbContext).ToList();
        }

        public List<AdminReviewPeriod> GetActiveReviewPeriods() {
            return _pmRepository.Get<AdminReviewPeriod>(_pmRepository.GetApplicationDbContext).Where(x => x.DateDeleted == null).ToList();
        }

        public AdminScoreRating AddScoreRating(AdminScoreRating scoreRating) {
            return _pmRepository.Insert(scoreRating, _pmRepository.GetApplicationDbContext);
        }

        public bool DeleteScoreRating(AdminScoreRating scoreRating) {
            _pmRepository.Delete(scoreRating, _pmRepository.GetApplicationDbContext);
            return true;
        }

        public AdminScoreRating UpdateScoreRating(AdminScoreRating scoreRating) {
            return _pmRepository.Update(scoreRating, _pmRepository.GetApplicationDbContext);
        }

        public AdminScoreRating FindScoreRating(int id) {
            return _pmRepository.Find<AdminScoreRating>(id, _pmRepository.GetApplicationDbContext);
        }

        public List<AdminScoreRating> GetScoreRatings() {
            return _pmRepository.Get<AdminScoreRating>(_pmRepository.GetApplicationDbContext).ToList();
        }

        public List<AdminScoreRating> GetActiveScoreRatings() {
            return _pmRepository.Get<AdminScoreRating>(_pmRepository.GetApplicationDbContext).Where(x => x.DateDeleted == null).ToList();
        }

        public AdminStrategicGoal AddStrategicGoal(AdminStrategicGoal strategicGoal) {
            return _pmRepository.Insert(strategicGoal, _pmRepository.GetApplicationDbContext);
        }

        public bool DeleteStrategicGoal(AdminStrategicGoal strategicGoal) {
            _pmRepository.Delete(strategicGoal, _pmRepository.GetApplicationDbContext);
            return true;
        }

        public AdminStrategicGoal UpdateStrategicGoal(AdminStrategicGoal strategicGoal) {
            return _pmRepository.Update(strategicGoal, _pmRepository.GetApplicationDbContext);
        }

        public AdminStrategicGoal FindStrategicGoal(int id) {
            return _pmRepository.Find<AdminStrategicGoal>(id,_pmRepository.GetApplicationDbContext);
        }

        public List<AdminStrategicGoal> GetStrategicGoals() {
            return _pmRepository.Get<AdminStrategicGoal>(_pmRepository.GetApplicationDbContext).ToList();
        }

        public List<AdminStrategicGoal> GetActiveStrategicGoals() {
            return _pmRepository.Get<AdminStrategicGoal>(_pmRepository.GetApplicationDbContext).Where(x => x.DateDeleted == null).ToList();
        }

        public AdminTerm AddTerm(AdminTerm term) {
            return _pmRepository.Insert(term, _pmRepository.GetApplicationDbContext);
        }

        public bool DeleteTerm(AdminTerm term) {
             _pmRepository.Delete(term, _pmRepository.GetApplicationDbContext);
            return true;
        }

        public AdminTerm UpdateTerm(AdminTerm term) {
            return _pmRepository.Update(term, _pmRepository.GetApplicationDbContext);
        }

        public AdminTerm FindTerm(int id) {
            return _pmRepository.Find<AdminTerm>(id, _pmRepository.GetApplicationDbContext);
        }

        public List<AdminTerm> GetTerms() {
            return _pmRepository.Get<AdminTerm>(_pmRepository.GetApplicationDbContext).ToList();
        }

        public List<AdminTerm> GetActiveTerms() {
            return _pmRepository.Get<AdminTerm>(_pmRepository.GetApplicationDbContext).Where(x => x.DateDeleted == null && x.StatusId == 1).ToList();
        }

        public List<LinkPerformanceYearReviewPeriod> GetLinkedPerformanceYearReviews() {
            var results = new List<LinkPerformanceYearReviewPeriod>();
            var linkedPerformanceYears = _pmRepository.Get<PMReviewPeriod>(_pmRepository.GetApplicationDbContext).
                                         Where(x => x.DateDeleted == null && x.StatusId != 4).
                                         Include(x => x.Status).Include(x => x.PerformanceYear).Include(x => x.ReviewPeriod);
            foreach (var item in linkedPerformanceYears) {
                var review = new LinkPerformanceYearReviewPeriod() {
                    PMReviewPeriodId = item.PMReviewPeriodId,
                    ReviewPeriodId = item.ReviewPeriodId,
                    PerformanceYearId = item.PerformanceYearId,
                    PerformanceYear = item.PerformanceYear.PerformanceYearName,
                    PerformanceYearStartEndDate = string.Format("{0} - {1}",
                                                                       item.PerformanceYear.StartDate.ToString("MMMM dd yyyy"),
                                                                       item.PerformanceYear.EndDate.ToString("MMMM dd yyyy")),
                    ReviewPeriod = item.ReviewPeriod.ReviewPeriodName,
                    StatusId = item.StatusId,
                    StatusDescription = item.Status.StatusName
                };
                results.Add(review);
            }
            return (results);
        }

        //Structure Organisation

        public StructureOrganisation AddStructureOrganisation(StructureOrganisation structureOrganisation) {
            return _pmRepository.Insert(structureOrganisation, _pmRepository.GetApplicationDbContext);
        }
        public bool DeleteStructureOrganisation(StructureOrganisation structureOrganisation) {
            _pmRepository.Delete(structureOrganisation, _pmRepository.GetApplicationDbContext);
            return true;
        }
        public StructureOrganisation UpdateStructureOrganisation(StructureOrganisation structureOrganisation) {
            return _pmRepository.Update(structureOrganisation, _pmRepository.GetApplicationDbContext);
        }
        public StructureOrganisation FindStructureOrganisation(int id) {
            return _pmRepository.Find<StructureOrganisation>(id,_pmRepository.GetApplicationDbContext);
        }
        public List<StructureOrganisation> GetStructureOrganisation() {
            return _pmRepository.Get<StructureOrganisation>(_pmRepository.GetApplicationDbContext).ToList();
        }
        public List<StructureOrganisation> GetActiveStructureOrganisation() {
            return _pmRepository.Get<StructureOrganisation>(_pmRepository.GetApplicationDbContext).Where(x => x.DateDeleted == null).ToList();
        }

        //Structure Portfolios
        public StructurePortfolio AddStructurePortfolio(StructurePortfolio structurePortfolio) {
            return _pmRepository.Insert(structurePortfolio, _pmRepository.GetApplicationDbContext);
        }
        public bool DeleteStructurePortfolio(StructurePortfolio structurePortfolio) {
             _pmRepository.Delete(structurePortfolio, _pmRepository.GetApplicationDbContext);
            return true;
        }
        public StructurePortfolio UpdateStructurePortfolio(StructurePortfolio structurePortfolio) {
            return _pmRepository.Update(structurePortfolio, _pmRepository.GetApplicationDbContext);
        }
        public Portfolio FindStructurePortfolio(int id) {
            return SearchPortfolio(id);
        }
        public List<Portfolio> GetStructurePortfolio() {
            return GetPortfolios();
        }
        public List<Portfolio> GetActiveStructurePortfolio() {
            return GetStructurePortfolio().Where(p => p.DeleteDate == null).ToList();
        }

        //Structure Department
        public StructureDepartment AddStructureDepartment(StructureDepartment structureDepartment) {
            return _pmRepository.Insert(structureDepartment, _pmRepository.GetApplicationDbContext);
        }
        public bool DeleteStructureDepartment(StructureDepartment structureDepartment) {
             _pmRepository.Delete(structureDepartment, _pmRepository.GetApplicationDbContext);
            return true;
        }

        public StructureDepartment UpdateStructureDepartment(StructureDepartment structureDepartment) {
            return _pmRepository.Update(structureDepartment, _pmRepository.GetApplicationDbContext);
        }

        public Department FindStructureDepartment(int id) {
            return SearchDepartment(id);
        }

        public List<Department> GetStructureDepartment() {
            return GetDepartment();
        }
        public List<Department> GetActiveStructureDepartment() {
            return GetStructureDepartment().Where(x => x.DeleteDate == null).ToList();
        }

        //Structure Teams
        public StructureTeam AddStructureTeam(StructureTeam structureTeam) {
            return _pmRepository.Insert(structureTeam, _pmRepository.GetApplicationDbContext);
        }
        public bool DeleteStructureTeam(StructureTeam structureTeam) {
             _pmRepository.Delete(structureTeam, _pmRepository.GetApplicationDbContext);
            return true;
        }
        public StructureTeam UpdateStructureTeam(StructureTeam structureTeam) {
            return _pmRepository.Update(structureTeam, _pmRepository.GetApplicationDbContext);
        }
        public Team FindStructureTeam(int id) {
            return SearchTeam(id);
        }
        public List<Team> GetStructureTeam() {
            return GetTeam();
        }
        public List<Team> GetActiveStructureTeam() {
            return GetStructureTeam().Where(t => t.DeleteDate == null).ToList();
        }

        //Document Types
        public PMDocumentType AddDocumentType(PMDocumentType pmDocumentType) {
            return _pmRepository.Insert(pmDocumentType, _pmRepository.GetApplicationDbContext);
        }
        public bool DeleteDocumentType(PMDocumentType pmDocumentType) {
             _pmRepository.Delete(pmDocumentType, _pmRepository.GetApplicationDbContext);
            return true;
        }
        public PMDocumentType UpdateDocumentType(PMDocumentType pmDocumentType) {
            return _pmRepository.Update(pmDocumentType, _pmRepository.GetApplicationDbContext);
        }
        public DocumentType FindDocumentType(int id) {
            return SearchDocumentType(id);
        }
        
        public List<DocumentType> GetActiveDocumentType() {
            return GetDocumentType().Where(x => x.DeleteDate == null).ToList();
        }

        //Reporting Structure
        public PMReviewReportingStructure AddReportingStructure(PMReviewReportingStructure pmReviewReportingStructure) {
            return _pmRepository.Insert(pmReviewReportingStructure, _pmRepository.GetApplicationDbContext);
        }
        public bool DeleteReportingStructure(PMReviewReportingStructure pmReviewReportingStructure) {
            _pmRepository.Delete(pmReviewReportingStructure, _pmRepository.GetApplicationDbContext);
            return true;
        }
        public PMReviewReportingStructure UpdateDocumentType(PMReviewReportingStructure pmReviewReportingStructure) {
            return _pmRepository.Update(pmReviewReportingStructure, _pmRepository.GetApplicationDbContext);
        }
        public PMReviewReportingStructure FindReviewReportingStructure(int id) {
            return _pmRepository.Find<PMReviewReportingStructure>(id,_pmRepository.GetApplicationDbContext);
        }

        public List<ReportingStructure> GetPMReviewReportingStructure() {
            var reportingStructure = new List<ReportingStructure>();
            var results = _pmRepository.Get<PMReviewReportingStructure>(_pmRepository.GetApplicationDbContext).
                          Include(X => X.Status).Include(x => x.Manager).Include(x => x.Owner).Include(x => x.DocumentType).
                          Include(x => x.Owner.JobGrade).Include(x => x.Manager.JobGrade).Where(x => x.DateDeleted == null).ToList();
            foreach(var item in results) {
                reportingStructure.Add(new ReportingStructure {
                    EmployeeRecordId = item.ReviewReportingStructureId,
                    EmployeeCode = item.Owner.EmployeeCode,
                    ManagerEployeeCode = item.Manager.EmployeeCode,
                    JobGradeName = item.Owner.JobGrade.JobGrade,
                    EmployeeName = string.Format("{0} {1}", item.Owner.Name, item.Owner.Surname),
                    ManagerName = string.Format("{0} {1}", item.Manager.Name, item.Manager.Surname),
                    DocumentTypeName = item.DocumentType.DocumentTypeName,
                    StatusName = item.Status.StatusName
                });
            }
            return (reportingStructure);
        }

        //Structure Employees
        public StructureEmployee AddEmployee(StructureEmployee employee) {
            return _pmRepository.Insert(employee, _pmRepository.GetApplicationDbContext);
        }
        public bool DeleteEmployee(StructureEmployee employee) {
            _pmRepository.Delete(employee, _pmRepository.GetApplicationDbContext);
            return true;
        }
        public StructureEmployee UpdateEmployee(StructureEmployee employee) {
            return _pmRepository.Update(employee, _pmRepository.GetApplicationDbContext);
        }
        public StructureEmployee FindEmployee(int id) {
            return _pmRepository.Find<StructureEmployee>(id, _pmRepository.GetApplicationDbContext);
        }

        public List<Employee> GetEmployee() {
            var employees = new List<Employee>();
            var results = _pmRepository.Get<StructureEmployee>(_pmRepository.GetApplicationDbContext).
                          Include(X => X.Status).Include(x => x.JobGrade).Include(x => x.Team).Include(x => x.Team.Department).
                          Where(x => x.DateDeleted == null).ToList();
            foreach (var item in results) {
                employees.Add(new Employee {
                    EmployeeRecordId = item.EmployeeRecordId,
                    EmployeeCode = item.EmployeeCode,
                    JobGradeName = item.JobGrade.JobGrade,
                    EmployeeName = string.Format("{0} {1}", item.Name, item.Surname),
                    Team = item.Team.TeamName,
                    Department = item.Team.Department.DepartmentName,
                    StatusName = item.Status.StatusName
                });
            }
            return (employees);
        }

    }

}
