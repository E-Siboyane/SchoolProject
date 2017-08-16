using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolProject.WebApplication.DTO;

namespace SchoolProject.WebApplication.ServiceManager.Interface {
    public interface IAdmininstrationManager {
        T InsertItem<T>(T item) where T : class;
        bool BulkInsertItem<T>(List<T> bulkInsertItems);
        bool DeleteItem<T>(T item) where T : class;
        T UpdateItem<T>(T item) where T : class;
        T FindItem<T>(int id) where T : class;
        List<T> GetItems<T>() where T : class;

        //Structure Portfolios
        List<Portfolio> GetPortfolios();
        Portfolio SearchPortfolio(int id);

        //Structure Department
        List<Department> GetDepartment();
        Department SearchDepartment(int id);

        //Structure Teams
        List<Team> GetTeam();
        Team SearchTeam(int id);

        //Job Grades
        List<JobGrade> GetJobGrade();
        JobGrade SearchJobGrade(int id);

        //Document Type
        List<DocumentType> GetDocumentType();
        DocumentType SearchDocumentType(int id);
    }
}
