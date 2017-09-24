using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolProject.WebApplication.DTO;
using SchoolProject.WebApplication.Models;

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


        //Status

         AdminStatus AddStatus(AdminStatus Status);
         bool DeleteStatus(AdminStatus Status);

         AdminStatus UpdateStatus(AdminStatus Status);

         AdminStatus FindStatus(int id);

         List<AdminStatus> GetStatuses();

         List<AdminStatus> GetActiveStatuses();

        //Job Grade
         AdminJobGrade AddJobGrade(AdminJobGrade jobGrade);

         bool DeleteJobGrade(AdminJobGrade jobGrade);

         AdminJobGrade UpdateJobGrade(AdminJobGrade jobGrade);

         JobGrade FindJobGrade(int id);

         List<JobGrade> GetJobGrades();

         List<JobGrade> GetActiveJobGrades();

         AdminMeasure AddMeasure(AdminMeasure measure);

         bool DeleteMeasure(AdminMeasure measure);

         AdminMeasure UpdateMeasure(AdminMeasure measure);

         AdminMeasure FindMeasure(int id);

         List<AdminMeasure> GetMeasures();

         List<AdminMeasure> GetActiveMeasures();

         AdminObjective AddObjective(AdminObjective objective);
         bool DeleteObjective(AdminObjective objective);

         AdminObjective UpdateObjective(AdminObjective objective);

         AdminObjective FindObjective(int id);

         List<AdminObjective> GetObjectives();

         List<AdminObjective> GetActiveObjectives();

         AdminPerformanceYear AddPerformanceYear(AdminPerformanceYear performanceYear);

         bool DeletePerformanceYear(AdminPerformanceYear performanceYear);

         AdminPerformanceYear UpdatePerformanceYear(AdminPerformanceYear performanceYear);

         AdminPerformanceYear FindPerformanceYear(int id);

         List<AdminPerformanceYear> GetPerformanceYears();

         List<AdminPerformanceYear> GetActivePerformanceYears();

         AdminReviewPeriod AddReviewPeriod(AdminReviewPeriod reviewPeriod);

         bool DeleteReviewPeriod(AdminReviewPeriod reviewPeriod);

         AdminReviewPeriod UpdateReviewPeriod(AdminReviewPeriod reviewPeriod);

         AdminReviewPeriod FindReviewPeriod(int id);

         List<AdminReviewPeriod> GetReviewPeriods();

         List<AdminReviewPeriod> GetActiveReviewPeriods();

         AdminScoreRating AddScoreRating(AdminScoreRating scoreRating);

         bool DeleteScoreRating(AdminScoreRating scoreRating);

         AdminScoreRating UpdateScoreRating(AdminScoreRating scoreRating);

         AdminScoreRating FindScoreRating(int id);

        List<AdminScoreRating> GetScoreRatings();

         List<AdminScoreRating> GetActiveScoreRatings();

         AdminStrategicGoal AddStrategicGoal(AdminStrategicGoal strategicGoal);

         bool DeleteStrategicGoal(AdminStrategicGoal strategicGoal);

         AdminStrategicGoal UpdateStrategicGoal(AdminStrategicGoal strategicGoal);

         AdminStrategicGoal FindStrategicGoal(int id);

         List<AdminStrategicGoal> GetStrategicGoals();

        List<AdminStrategicGoal> GetActiveStrategicGoals();

        AdminTerm AddTerm(AdminTerm term);

         bool DeleteTerm(AdminTerm term);

        AdminTerm UpdateTerm(AdminTerm term);

        AdminTerm FindTerm(int id);

        List<AdminTerm> GetTerms();

        List<AdminTerm> GetActiveTerms();

        List<LinkPerformanceYearReviewPeriod> GetLinkedPerformanceYearReviews();

        //Structure Organisation

        StructureOrganisation AddStructureOrganisation(StructureOrganisation structureOrganisation);
        bool DeleteStructureOrganisation(StructureOrganisation structureOrganisation);
        StructureOrganisation UpdateStructureOrganisation(StructureOrganisation structureOrganisation);
        StructureOrganisation FindStructureOrganisation(int id);
         List<StructureOrganisation> GetStructureOrganisation();
        List<StructureOrganisation> GetActiveStructureOrganisation();

        //Structure Portfolios
        StructurePortfolio AddStructurePortfolio(StructurePortfolio structurePortfolio);
        bool DeleteStructurePortfolio(StructurePortfolio structurePortfolio);
        StructurePortfolio UpdateStructurePortfolio(StructurePortfolio structurePortfolio);
        Portfolio FindStructurePortfolio(int id);
        List<Portfolio> GetStructurePortfolio();
        List<Portfolio> GetActiveStructurePortfolio();

        //Structure Department
        StructureDepartment AddStructureDepartment(StructureDepartment structureDepartment);
        bool DeleteStructureDepartment(StructureDepartment structureDepartment);

        StructureDepartment UpdateStructureDepartment(StructureDepartment structureDepartment);

        Department FindStructureDepartment(int id);

        List<Department> GetStructureDepartment();
        List<Department> GetActiveStructureDepartment();

        //Structure Teams
        StructureTeam AddStructureTeam(StructureTeam structureTeam);
        bool DeleteStructureTeam(StructureTeam structureTeam);
        StructureTeam UpdateStructureTeam(StructureTeam structureTeam);
        Team FindStructureTeam(int id);
        List<Team> GetStructureTeam();
        List<Team> GetActiveStructureTeam();

        //Document Types
         PMDocumentType AddDocumentType(PMDocumentType pmDocumentType);

        bool DeleteDocumentType(PMDocumentType pmDocumentType);

        PMDocumentType UpdateDocumentType(PMDocumentType pmDocumentType);

        DocumentType FindDocumentType(int id);

       List<DocumentType> GetActiveDocumentType();

        //Reporting Structure
        PMReviewReportingStructure AddReportingStructure(PMReviewReportingStructure pmReviewReportingStructure);
        bool DeleteReportingStructure(PMReviewReportingStructure pmReviewReportingStructure);
        PMReviewReportingStructure UpdateDocumentType(PMReviewReportingStructure pmReviewReportingStructure);
        PMReviewReportingStructure FindReviewReportingStructure(int id);

        List<ReportingStructure> GetPMReviewReportingStructure();

    }
}
