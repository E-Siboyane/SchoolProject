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
    }
}
