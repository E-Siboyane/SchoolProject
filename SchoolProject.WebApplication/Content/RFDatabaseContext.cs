using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicCapital.RF.DTO;

namespace EconomicCapital.RF.DataAccess {
   public class RFDatabaseContext : DbContext {
      public RFDatabaseContext() : base("RFStagingDbConnectionString") {
         Database.SetInitializer<RFDatabaseContext>(null);
      }

      protected override void OnModelCreating(DbModelBuilder modelBuilder) {
         modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
         modelBuilder.Conventions.Remove<PluralizingEntitySetNameConvention>();
         modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
         base.Configuration.ProxyCreationEnabled = false;
         base.Configuration.AutoDetectChangesEnabled = false;
         base.Configuration.ValidateOnSaveEnabled = false;

         base.OnModelCreating(modelBuilder);
      }
      public DbSet<Job> Jobs { get; set; }
      public DbSet<AnalysisJobManager> AnalysisJobManagers { get; set; }
      public DbSet<AnalysisJobReport> AnalysisJobReports { get; set; }
      public DbSet<ABSARetailPoolingInput> ABSARetailPoolingInputs { get; set; }
      public DbSet<ABSARetailPoolingPDSegment> ABSARetailPoolingPDSegments { get; set; }
      public DbSet<ABSAPoolingLookup> ABSAPoolingLookups { get; set; }
      public DbSet<ABSARetailPoolingLGDSegment> ABSARetailPoolingLGDSegments { get; set; }
      public DbSet<ABSARetailPoolingTMSegment> ABSARetailPoolingTMSegments { get; set; }
      public DbSet<ABSARetailProcessingProgress> ABSARetailProcessingProgresses { get; set; }
      public DbSet<ABSARetailProcessingProgressDetail> ABSARetailProcessingProgressDetails { get; set; }
      public DbSet<ABSARetailPoolingConfigType> ABSARetailPoolingConfigTypes { get; set; }
      public DbSet<ABSARetailPoolingConfigTypeDetail> ABSARetailPoolingConfigTypeDetails { get; set; }
   }
}
