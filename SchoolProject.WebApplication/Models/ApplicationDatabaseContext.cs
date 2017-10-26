using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SchoolProject.WebApplication.Models {
      public class ApplicationDatabaseContext : IdentityDbContext<ApplicationUser> {
        public ApplicationDatabaseContext(): base("PerformanceManagementConnectionString") {
            base.Configuration.ProxyCreationEnabled = false;
        }

        public static ApplicationDatabaseContext Create() {
            return new ApplicationDatabaseContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            Configuration.ProxyCreationEnabled = false;
        }
        public DbSet<AdminJobGrade> JobGrade { get; set; }
        public DbSet<AdminMeasure> Measure { get; set; }
        public DbSet<AdminObjective> Objective { get; set; }
        public DbSet<AdminPerformanceYear> PerformanceYear { get; set; }
        public DbSet<AdminReviewPeriod> ReviewPeriod { get; set; }
        public DbSet<AdminScoreRating> ScoreRating { get; set; }
        public DbSet<AdminStatus> Status { get; set; }
        public DbSet<AdminStrategicGoal> StrategicGoal { get; set; }
        public DbSet<AdminTerm> Term { get; set; }
        public DbSet<PMDocumentType> PMDocumentType { get; set; }
        public DbSet<PMeasure> PMMeasure { get; set; }
        public DbSet<PMObjective> PMObjective { get; set; }
        public DbSet<PMProcessStage> PMProcessStage { get; set; }
        public DbSet<PMReview> PMReview { get; set; }
        public DbSet<PMReviewPeriod> PMReviewPeriod { get; set; }
        public DbSet<PMReviewProgressStatus> PMReviewProgressStatus { get; set; }
        public DbSet<PMReviewReportingStructure> PMReviewReportingStructure { get; set; }
        public DbSet<PMStrategicGoal> PMStrategicGoal { get; set; }
        public DbSet<StructureDepartment> StructureDepartment { get; set; }
        public DbSet<StructureEmployee> StructureEmployee { get; set; }
        public DbSet<StructureOrganisation> StructureOrganisation { get; set; }
        public DbSet<StructurePortfolio> StructurePortfolio { get; set; }
        public DbSet<StructureTeam> StructureTeam { get; set; }
    }
}
