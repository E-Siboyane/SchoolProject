using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using EconomicCapital.RF.DTO;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Data.EntityClient;
using System.Data.Entity.Migrations;

namespace EconomicCapital.RF.DataAccess {
   public class SimPakDataProcessing : ISimPakDataProcessing, IDisposable {

      private readonly ApplicationDbContext _dbContextSourceData;
      private readonly RFDatabaseContext _dbContextRFStaging;

      public SimPakDataProcessing(ApplicationDbContext dbContext, RFDatabaseContext dbRFStaging) {
         _dbContextSourceData = dbContext;
         _dbContextRFStaging = dbRFStaging;

         _dbContextSourceData.Database.CommandTimeout = 0;
         _dbContextRFStaging.Database.CommandTimeout = 0;
      }

      public RFDatabaseContext GetRFStagingDbContext() {
         return _dbContextRFStaging;
      }

      public ApplicationDbContext GetApplicationDbContext() {
         return (_dbContextSourceData);
      }

      public IEnumerable<SP_Get_RFCounterParty> GetRFCounterParty() {
         var result = _dbContextSourceData.Database.SqlQuery<SP_Get_RFCounterParty>(string.Format("[dbo].Sp_Get_RFCounterParty")).ToList();
         return (result);
      }

      public IEnumerable<Sp_Get_RFCounterPartyRSquared> GetRFCounterPartyRSquared() {
         return _dbContextSourceData.Database.SqlQuery<Sp_Get_RFCounterPartyRSquared>(string.Format("[dbo].[sp_Get_RFCounterpartyRSquared]")).ToList();
      }

      public IEnumerable<Sp_Get_RFExposureUDV> GetRFExposureUDV() {
         return _dbContextSourceData.Database.SqlQuery<Sp_Get_RFExposureUDV>(string.Format("[dbo].[sp_Get_RFExposureUDV]")).ToList();
      }

      public IEnumerable<Sp_Get_RFInstrumentPdsFlexible> GetRFInstrumentPdsFlexible() {
         return _dbContextSourceData.Database.SqlQuery<Sp_Get_RFInstrumentPdsFlexible>(string.Format("[dbo].[sp_Get_RFinstrumentPdsFlexible]")).ToList();
      }

      public IEnumerable<Sp_Get_RFInstrumentRating> GetRFInstrumentRating() {
         return _dbContextSourceData.Database.SqlQuery<Sp_Get_RFInstrumentRating>(string.Format("[dbo].[sp_Get_RFinstrumentRating]")).ToList();
      }

      public IEnumerable<Sp_Get_RFPortfolio> GetRFPortfolio() {
         return _dbContextSourceData.Database.SqlQuery<Sp_Get_RFPortfolio>(string.Format("[dbo].[sp_Get_RFPortfolio]")).AsEnumerable();
      }

      public IEnumerable<Sp_Get_RFPortfolioDetail> GetRFPortfolioDetail() {
         return _dbContextSourceData.Database.SqlQuery<Sp_Get_RFPortfolioDetail>(string.Format("[dbo].[sp_Get_RFPortfolioDetail]")).AsEnumerable();
      }

      public IEnumerable<Sp_Get_RFTermLoanBullet> GetRFTermLoanBullet() {
         return _dbContextSourceData.Database.SqlQuery<Sp_Get_RFTermLoanBullet>(string.Format("[dbo].[sp_Get_RFTermloanBullet]")).AsEnumerable();
      }

      public IEnumerable<Sp_Get_RFTermLoanAmortizing> GetRFTermLoanAmortizing() {
         return _dbContextSourceData.Database.SqlQuery<Sp_Get_RFTermLoanAmortizing>(string.Format("[dbo].[sp_Get_RFtermLoanAmortizing]")).AsEnumerable();
      }

      public IEnumerable<Sp_Get_RFInstrumentPrice> GetRFInstrumentPrice() {
         return _dbContextSourceData.Database.SqlQuery<Sp_Get_RFInstrumentPrice>(string.Format("[dbo].[sp_Get_RFinstrumentPrice]")).AsEnumerable();
      }

      public DateTime GetRFImportAsOfDate() {
         return _dbContextSourceData.Database.SqlQuery<DateTime>(string.Format("[dbo].[Sp_Get_RFImportAsOfDate]")).FirstOrDefault();
      }

      public IEnumerable<Sp_Get_RFCovarianceModelFactorCoefficients> GetRFCovarianceModelFactorCoefficients() {
         return _dbContextSourceData.Database.SqlQuery<Sp_Get_RFCovarianceModelFactorCoefficients>(string.Format("[dbo].[sp_Get_RFCovarianceModelFactorCoefficients]")).AsEnumerable();
      }

      public IEnumerable<SP_Get_RFYieldCurveHistory> GetRFYieldCurveHistory() {
         return _dbContextSourceData.Database.SqlQuery<SP_Get_RFYieldCurveHistory>(string.Format("[dbo].[sp_Get_RFyieldCurveHistory]")).AsEnumerable();
      }

      public string BulkInsert<T>(List<T> bulkInsertItems, DbContext dbContext) where T : class {
         var conString = GetRFDbContextConnectionString();

         if(string.Compare(dbContext.ToString().Split('.')[3], "ApplicationDbContext", true) == 0)
            conString = dbContext.Database.Connection.ConnectionString;

         try {
            using(var connection = new SqlConnection(conString)) {
               connection.Open();
               SqlTransaction transaction = connection.BeginTransaction();

               using(var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction)) {
                  bulkCopy.BatchSize = 100;
                  var tableName = bulkInsertItems.GetType().ToString().Split('.').LastOrDefault();
                  tableName = tableName.Remove(tableName.Length - 1);
                  bulkCopy.DestinationTableName = string.Format("[dbo].[{0}]", tableName.ToLower());

                  try {
                     var data = tableName.Equals("ABSARetailPoolingBulkUpdate", StringComparison.OrdinalIgnoreCase) ? bulkInsertItems.AsDataTableWithId() : bulkInsertItems.AsDataTable();
                     bulkCopy.BulkCopyTimeout = 0;
                     bulkCopy.WriteToServer(data);
                  }
                  catch(Exception exception) {
                     Console.WriteLine(exception.Message.ToString());
                     transaction.Rollback();
                     connection.Close();
                  }
               }
               transaction.Commit();
            }
            return string.Format("Successfull");
         }
         catch(Exception ex) {
            return string.Format("Error: {0}", ex.Message.ToString());
         }
      }

      public string AddJob(Job jobDetails, DbContextType dbContextType) {
         var result = string.Empty;
         try {
            switch(dbContextType) {
               case DbContextType.RFSTAGINGCONTEXT:
                  _dbContextRFStaging.Jobs.Add(jobDetails);
                  _dbContextRFStaging.SaveChanges();
                  break;
               case DbContextType.SOURCEDATACONTEXT:
                  _dbContextSourceData.Jobs.Add(jobDetails);
                  _dbContextSourceData.SaveChanges();
                  break;
               default:
                  throw new NotImplementedException();
            }

            return jobDetails.JobId.ToString();
         }
         catch(DbEntityValidationException e) {
            result = string.Empty;
            foreach(var eve in e.EntityValidationErrors) {
               foreach(var ve in eve.ValidationErrors) {
                  result += string.Format("{0}: {1} || ", ve.PropertyName, ve.ErrorMessage);
               }
            }
            return result;
         }
         catch(Exception ex) {
            return ex.Message.ToString();
         }
      }

      public string AddAnalysisJobSettings(AnalysisJobManager analysisJobSettings, DbContextType dbContextType) {
         var result = string.Empty;
         try {
            switch(dbContextType) {
               case DbContextType.RFSTAGINGCONTEXT:
                  return AddStagingDbAnalysisJobSettings(analysisJobSettings);
               case DbContextType.SOURCEDATACONTEXT:
                  _dbContextSourceData.AnalysisJobManagers.Add(analysisJobSettings);
                  _dbContextSourceData.SaveChanges();
                  return analysisJobSettings.AnalysisJobManagerId.ToString();
               default:
                  throw new NotImplementedException();
            }

         }
         catch(DbEntityValidationException e) {
            result = string.Empty;
            foreach(var eve in e.EntityValidationErrors) {
               foreach(var ve in eve.ValidationErrors) {
                  result += string.Format("{0}: {1} || ", ve.PropertyName, ve.ErrorMessage);
               }
            }
            return result;
         }
         catch(Exception ex) {
            return ex.Message.ToString();
         }
      }

      private string AddStagingDbAnalysisJobSettings(AnalysisJobManager analysisJobSettings) {
         var sql = string.Format("{0}{1}", "[dbo].[ABSA_INSERT_AnalysisJobManager_CORE_DB] @JobId,@PortfolioName,@HoldingDate,",
                                  "@AnalysisDate,@AnalysisSettings,@DataSettings");
         var jobId = new SqlParameter("@JobId", analysisJobSettings.JobId);
         var portfolioName = new SqlParameter("@PortfolioName", analysisJobSettings.PortfolioName);
         var holdingDate = new SqlParameter("@HoldingDate", analysisJobSettings.HoldingDate);
         var analysisDate = new SqlParameter("@AnalysisDate", analysisJobSettings.AnalysisDate);
         var analysisSettings = new SqlParameter("@AnalysisSettings", analysisJobSettings.AnalysisSettings);
         var dataSettings = new SqlParameter("@DataSettings", analysisJobSettings.DataSettings);
         _dbContextRFStaging.Database.ExecuteSqlCommand(sql, jobId, portfolioName, holdingDate,
                                                                analysisDate, analysisSettings, dataSettings);
         return "Successful";
      }

      private bool UpdateJobName(int jobId, DbContextType dbContextType) {
         switch(dbContextType) {
            case DbContextType.RFSTAGINGCONTEXT:
               var job = _dbContextRFStaging.Jobs.Find(jobId);
               job.JobName = string.Format("{0}{1}", job.JobName, job.JobId);
               _dbContextRFStaging.SaveChanges();
               break;
            case DbContextType.SOURCEDATACONTEXT:
               var jobSourceDb = _dbContextSourceData.Jobs.Find(jobId);
               jobSourceDb.JobName = string.Format("{0}{1}", jobSourceDb.JobName, jobSourceDb.JobId);
               _dbContextSourceData.SaveChanges();
               break;
            default:
               throw new NotImplementedException();
         }

         return true;
      }

      public bool UpdateJob(int jobId, DbContextType dbContextType) {
         var sql = string.Format("UPDATE Job SET status = 'Loading Complete' WHERE jobId = {0}", jobId);
         switch(dbContextType) {
            case DbContextType.RFSTAGINGCONTEXT:
               _dbContextRFStaging.Database.ExecuteSqlCommand(sql);
               return true;
            case DbContextType.SOURCEDATACONTEXT:
               _dbContextSourceData.Database.ExecuteSqlCommand(sql);
               return true;
            default:
               throw new NotImplementedException();
         }
      }

      public bool UpdateAnalysisJobSettings(int analysisJobSettingsId, DbContextType dbContextType) {
         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               var sql = string.Format("UPDATE AnalysisJobManager SET [IsProcessed] = 1, [ProcessingDate] = GETDATE() WHERE [AnalysisJobManagerId] = {0}", analysisJobSettingsId);
               _dbContextSourceData.Database.ExecuteSqlCommand(sql);
               return true;
            case DbContextType.RFSTAGINGCONTEXT:
               var sqlStagingDb = string.Empty;
               sqlStagingDb = string.Format("[dbo].[ABSA_Update_AnalysisJobManager_CORE_DB] @AnalysisJobManagerId");
               _dbContextRFStaging.Database.ExecuteSqlCommand(sqlStagingDb, new SqlParameter("@AnalysisJobManagerId", analysisJobSettingsId));
               return true;
            default:
               throw new NotImplementedException();
         }
      }

      public bool AddImportAsOfDate(int jobId, DbContextType dbContextType) {
         var sql = string.Format("INSERT INTO  importAsOfDate(jobId,asOfDate) VALUES({0},'{1}')", jobId, DateTime.Now.Date);
         switch(dbContextType) {
            case DbContextType.RFSTAGINGCONTEXT:
               _dbContextRFStaging.Database.ExecuteSqlCommand(sql);
               return true;
            case DbContextType.SOURCEDATACONTEXT:
               _dbContextSourceData.Database.ExecuteSqlCommand(sql);
               return true;
            default:
               throw new NotImplementedException();
         }
      }

      public List<AnalysisJobReport> GetRFAnalyisJobReportDueForProcessing() {
         //Refresh AnalysisJobReports
         var refreshReportsQueue = RFAnalysisJobQueueReports();
         var sql = string.Format("[dbo].[ABSA_SELECT_AnalysisJobReportsDueForProcessing]");
         return _dbContextRFStaging.Database.SqlQuery<AnalysisJobReport>(sql).ToList();
      }

      private bool RFAnalysisJobQueueReports() {
         var sql = string.Format("[dbo].[ABSA_QUEUE_AnalysisJobReports_CORE_DB]");
         _dbContextRFStaging.Database.ExecuteSqlCommand(sql);
         return true;
      }

      public bool UpdateRFUpdateAnalysisJobReportToProcessed(int id) {
         var sql = string.Format("[dbo].[ABSA_UPDATE_AnalysisJobReportToProcessed_CORE_DB] @Id");
         _dbContextRFStaging.Database.ExecuteSqlCommand(sql, new SqlParameter("@Id", id));
         return true;
      }

      public List<AnalysisJobsDueForProcessing> GetStatgingJobsDueForProcessing() {
         var sql = string.Format("[dbo].[ABSA_Select_RFAnalysisJobsDueForScheduling]");
         return _dbContextRFStaging.Database.SqlQuery<AnalysisJobsDueForProcessing>(sql).ToList();
      }

      public List<RetailPoolingInput> GetRetailPoolingInputContents(string productType, DbContextType dbContextType) {
         List<RetailPoolingInput> Result = new List<RetailPoolingInput>();
         var sql = string.Format("SELECT  ObligorId, FacilityId, RSq, QSq, AssetReturnWeights, RecoveryWeights, Tenors, TimeToMaturity, CumulativePDs, " +
                                 "LGD, EAD, SUBMISSION_UNIT_CD, EC_SEG, PROD, EC_CLUSTER, UNIQUE_KEY, RankByPD, RankkByLGD, RankByTM, RowId, NewPDRank, " +
                                 "NewLGDRank, NewTMRank, FinalPDRank, FinalLGDRank, FinalTMRank " +
                                 "FROM [dbo].[ABSARetailPoolingInput]" +
                                 "WHERE [InputType] = '{0}'", productType);
         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               Result = _dbContextSourceData.Database.SqlQuery<RetailPoolingInput>(sql).ToList();
               break;
            case DbContextType.RFSTAGINGCONTEXT:
               Result = _dbContextRFStaging.Database.SqlQuery<RetailPoolingInput>(sql).ToList();
               break;
            default:
               throw new NotImplementedException();
         }
         return (Result);
      }

      public bool RemoveRetailPoolingInputContents(DbContextType dbContextType) {
         var sql = string.Format("TRUNCATE TABLE [dbo].[ABSARetailPoolingInput]");
         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               _dbContextSourceData.Database.ExecuteSqlCommand(sql);
               break;
            case DbContextType.RFSTAGINGCONTEXT:
               _dbContextRFStaging.Database.ExecuteSqlCommand(sql);
               break;
            default:
               throw new NotImplementedException();
         }
         return true;
      }

      public List<string> GetRFAnalysisJobsReportsSProcs() {
         var sql = string.Format("[dbo].[ABSA_Reports_AnalysisReports]");
         return _dbContextRFStaging.Database.SqlQuery<string>(sql).ToList();
      }

      public string GenerateReport(int jobId, string asOfDate, string confidenceLevel, string storedProc) {
         var result = new StringBuilder();
         var conString = GetRFDbContextConnectionString();

         using(var conn = new SqlConnection(conString)) {
            conn.Open();
            SqlCommand sqlCommand = new SqlCommand(storedProc, conn);
            sqlCommand.Parameters.Add(new SqlParameter("@jobId", jobId));
            sqlCommand.Parameters.Add(new SqlParameter("@ConfidenceLevel", confidenceLevel));
            sqlCommand.Parameters.Add(new SqlParameter("@AsOfDate", asOfDate));
            sqlCommand.CommandTimeout = 0;
            sqlCommand.CommandType = CommandType.StoredProcedure;
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

            if(sqlDataReader.HasRows) {
               result.AppendLine(ExtractRowHeader(sqlDataReader));
               result.AppendLine(GenerateReportContent(sqlDataReader));

            }
            return (result.ToString());
         }
      }

      private string ExtractRowHeader(SqlDataReader sqlDataReader) {
         var headerRowColumns = string.Empty;
         if(sqlDataReader.HasRows) {
            for(int i = 0; i < sqlDataReader.FieldCount; i++) {
               headerRowColumns += sqlDataReader.GetName(i);
               if(i != sqlDataReader.FieldCount - 1)
                  headerRowColumns += ",";
            }
         }
         return (headerRowColumns);
      }

      private string GenerateReportContent(SqlDataReader sqlDataReader) {
         var result = new StringBuilder();
         if(sqlDataReader.HasRows) {
            while(sqlDataReader.Read()) {
               var row = string.Empty;
               for(int index = 0; index < sqlDataReader.FieldCount; index++) {
                  row += sqlDataReader[index] + ",";
               }
               result.AppendLine(RemoveLastComma(row));
            }
         }
         return (result.ToString());
      }

      private string RemoveLastComma(string row) {
         return row.Substring(0, row.Length - 1);
      }

      private string GetRFDbContextConnectionString() {
         return new RFDatabaseContext().Database.Connection.ConnectionString;
      }

      public List<string> GetRFBarCorWeights() {
         var result = new List<string>();
         var conString = _dbContextSourceData.Database.Connection.ConnectionString;

         using(var conn = new SqlConnection(conString)) {
            conn.Open();
            SqlCommand sqlCommand = new SqlCommand(string.Format("[dbo].[SP_Get_RFBarCorWeights]"), conn);
            sqlCommand.CommandTimeout = 0;
            sqlCommand.CommandType = CommandType.StoredProcedure;
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

            if(sqlDataReader.HasRows) {
               result.Add(ExtractRowHeader(sqlDataReader));
               result.AddRange(GenerateBarcorWeightsContent(sqlDataReader));
            }
            return (result);
         }
      }

      private List<string> GenerateBarcorWeightsContent(SqlDataReader sqlDataReader) {
         var result = new List<string>();
         if(sqlDataReader.HasRows) {
            while(sqlDataReader.Read()) {
               var row = string.Empty;
               for(int index = 0; index < sqlDataReader.FieldCount; index++) {
                  row += sqlDataReader[index] + ",";
               }
               result.Add(RemoveLastComma(row));
            }
         }
         return (result);
      }

      public bool SyncCountryFactorRSquared(int jobId, DbContextType dbContextType) {
         var sql = string.Format("[dbo].[ABSA_SyncCountryRSqured] @jobId");
         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               _dbContextSourceData.Database.ExecuteSqlCommand(sql, new SqlParameter("@jobId", jobId));
               return true;
            case DbContextType.RFSTAGINGCONTEXT:
               _dbContextRFStaging.Database.ExecuteSqlCommand(sql, new SqlParameter("@jobId", jobId));
               return true;
            default:
               throw new NotImplementedException();
         }
      }

      #region RETAIL_POOLING_LOGIC
      public bool TruncateRetailPoolingInputTable(DbContextType dbContextType) {
         var sql = $"[dbo].[ABSA_CLEANRETAILINPUTTABLES] ";
         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               _dbContextSourceData.Database.ExecuteSqlCommand(sql);
               return true;
            case DbContextType.RFSTAGINGCONTEXT:
               _dbContextRFStaging.Database.ExecuteSqlCommand(sql);
               return true;
            default:
               throw new NotImplementedException();
         }
      }

      public List<ABSARetailPoolingConfigTypeDetail> GetRetailPoolingConfigs(DbContextType dbContextType) {
         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               return _dbContextSourceData.ABSARetailPoolingConfigTypeDetails.Where(x=> string.Compare(x.Code,ABSARetailPoolingEnumConfigType.RP.ToString(),true) == 0).ToList();
            case DbContextType.RFSTAGINGCONTEXT:
               return _dbContextRFStaging.ABSARetailPoolingConfigTypeDetails.Where(x => string.Compare(x.Code, ABSARetailPoolingEnumConfigType.RP.ToString(), true) == 0).ToList();
            default:
               throw new NotImplementedException();
         }
      }

      public List<RetailPoolingInput> GetFileContents(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId) {
         var result = new List<RetailPoolingInput>();
         var sql = $"[dbo].[ABSA_RetailPoolingInput] '{productName.ToString()}', '{rowId}'";

         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               result = _dbContextSourceData.Database.SqlQuery<RetailPoolingInput>(sql).ToList();
               break;
            case DbContextType.RFSTAGINGCONTEXT:
               result = _dbContextRFStaging.Database.SqlQuery<RetailPoolingInput>(sql).ToList();
               break;
            default:
               throw new NotImplementedException();
         }
         return (result);
      }

      public List<ABSARetailPoolingPDSegment> GetFileContentsPD(DbContextType dbContextType, Guid rowId) {
         var result = new List<ABSARetailPoolingPDSegment>();
         var sql = $"[dbo].[ABSA_PDPoolingOutput] '{rowId}'";

         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               result = _dbContextSourceData.Database.SqlQuery<ABSARetailPoolingPDSegment>(sql).ToList();
               break;
            case DbContextType.RFSTAGINGCONTEXT:
               result = _dbContextRFStaging.Database.SqlQuery<ABSARetailPoolingPDSegment>(sql).ToList();
               break;
            default:
               throw new NotImplementedException();
         }
         return (result);
      }

      public List<ABSARetailPoolingLGDSegment> GetFileContentsLGD(DbContextType dbContextType, Guid rowId) {
         var result = new List<ABSARetailPoolingLGDSegment>();
         var sql = $"[dbo].[ABSA_LGDPoolingOutput] '{rowId}'";

         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               result = _dbContextSourceData.Database.SqlQuery<ABSARetailPoolingLGDSegment>(sql).ToList();
               break;
            case DbContextType.RFSTAGINGCONTEXT:
               result = _dbContextRFStaging.Database.SqlQuery<ABSARetailPoolingLGDSegment>(sql).ToList();
               break;
            default:
               throw new NotImplementedException();
         }
         return (result);
      }

      public List<ABSARetailPoolingTMSegment> GetFileContentsTM(DbContextType dbContextType, Guid rowId) {
         var result = new List<ABSARetailPoolingTMSegment>();
         var sql = $"[dbo].[ABSA_TMPoolingOutput] '{rowId}'";

         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               result = _dbContextSourceData.Database.SqlQuery<ABSARetailPoolingTMSegment>(sql).ToList();
               break;
            case DbContextType.RFSTAGINGCONTEXT:
               result = _dbContextRFStaging.Database.SqlQuery<ABSARetailPoolingTMSegment>(sql).ToList();
               break;
            default:
               throw new NotImplementedException();
         }
         return (result);
      }

      public ABSAPoolingLookup GetLookupData(RetailPoolingFileType productName, int totalcount, DbContextType dbContextType) {
         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               return _dbContextSourceData.ABSAPoolingLookups.Where(x => totalcount <= x.FilterMaximum).Where(x => totalcount >= x.FilterMinimum).
                                                               Where(x => string.Compare(x.Product, productName.ToString(), true) == 0).ToList().FirstOrDefault();
            case DbContextType.RFSTAGINGCONTEXT:
               return _dbContextRFStaging.ABSAPoolingLookups.Where(x => totalcount <= x.FilterMaximum).Where(x => totalcount >= x.FilterMinimum).
                                                               Where(x => string.Compare(x.Product, productName.ToString(), true) == 0).ToList().FirstOrDefault();
            default:
               throw new NotImplementedException();
         }
      }

      public bool InitialClusterPDLogic(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId) {
         var sql = $"[dbo].[ABSA_INITIALIZEPDLOGIC] '{productName.ToString()}', '{rowId}'";
         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               _dbContextSourceData.Database.ExecuteSqlCommand(sql);
               return true;
            case DbContextType.RFSTAGINGCONTEXT:
               _dbContextRFStaging.Database.ExecuteSqlCommand(sql);
               return true;
            default:
               throw new NotImplementedException();
         }
      }

      //public bool InitialClusterPDLogic(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId) {
      //   var sql = string.Format("[dbo].[ABSA_INITIALIZEPDLOGIC] @prod'");
      //   switch(dbContextType) {
      //      case DbContextType.SOURCEDATACONTEXT:
      //         _dbContextSourceData.Database.ExecuteSqlCommand(sql, new SqlParameter("@prod", productName.ToString()));
      //         return true;
      //      case DbContextType.RFSTAGINGCONTEXT:
      //         _dbContextRFStaging.Database.ExecuteSqlCommand(sql, new SqlParameter("@prod", productName.ToString()));
      //         return true;
      //      default:
      //         throw new NotImplementedException();
      //   }
      //}

      public List<ABSARetailPoolingClusterSegment> GetClusterSegments(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId) {
         var result = new List<ABSARetailPoolingClusterSegment>();
         var sql = $"[dbo].[ABSA_PRODUCTSEGMENTS] '{productName.ToString()}', '{rowId}'";

         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               result = _dbContextSourceData.Database.SqlQuery<ABSARetailPoolingClusterSegment>(sql).ToList();
               break;
            case DbContextType.RFSTAGINGCONTEXT:
               result = _dbContextRFStaging.Database.SqlQuery<ABSARetailPoolingClusterSegment>(sql).ToList();
               break;
            default:
               throw new NotImplementedException();
         }
         return (result);
      }

      public List<ABSARetailPoolingPDSegment> GetSegmentContents(ABSARetailPoolingPDSegmentParam param) {
         var result = new List<ABSARetailPoolingPDSegment>();
         var sql = $"[dbo].[ABSA_PDGETSEGMENTCONTENTS] {param.ClusterRowId},{param.SegmentRowId},'{param.Segment}', '{param.RetailPoolingFileType.ToString()}', '{param.RowId}'";

         switch(param.DbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               result = _dbContextSourceData.Database.SqlQuery<ABSARetailPoolingPDSegment>(sql).ToList();
               break;
            case DbContextType.RFSTAGINGCONTEXT:
               result = _dbContextRFStaging.Database.SqlQuery<ABSARetailPoolingPDSegment>(sql).ToList();
               break;
            default:
               throw new NotImplementedException();
         }
         return (result);
      }

      public bool UpdateDRatioInitialRankFinalPool(List<ABSARetailPoolingPDSegment> segmentContents, DbContextType dbContextType, Guid rowId) {
         var bulkUpdateContents = new List<ABSARetailPoolingBulkUpdate>();
         foreach(var item in segmentContents) {
            bulkUpdateContents.Add(JsonConvert.DeserializeObject<ABSARetailPoolingBulkUpdate>(JsonConvert.SerializeObject(item)));
         }
       
         var sql = $"[dbo].[ABSA_UpdatePopulatePDRatioInitialRankFinalPool] '{segmentContents.FirstOrDefault().Product}', '{rowId}'";
         var update = BulkUpdatePoolingContents(bulkUpdateContents, dbContextType, sql);
         bulkUpdateContents.Clear();
         return (true);
      }

      public bool UpdateInputFileFinalPDRank(RetailPoolingFileType productName,DbContextType dbContextType, Guid rowId) {
         var sql = $"[dbo].[ABSA_PDUPDATEFINALRANK] '{productName.ToString()}', '{rowId}'";
         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               _dbContextSourceData.Database.ExecuteSqlCommand(sql);
               return true;
            case DbContextType.RFSTAGINGCONTEXT:
               _dbContextRFStaging.Database.ExecuteSqlCommand(sql);
               return true;
            default:
               throw new NotImplementedException();
         }
      }

      public bool UpdateInputFileFinalLGDRank(RetailPoolingFileType productName,DbContextType dbContextType, Guid rowId) {
         var sql = $"[dbo].[ABSA_LGDUPDATEFINALRANK] '{productName.ToString()}', '{rowId}'";
         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               _dbContextSourceData.Database.ExecuteSqlCommand(sql);
               return true;
            case DbContextType.RFSTAGINGCONTEXT:
               _dbContextRFStaging.Database.ExecuteSqlCommand(sql);
               return true;
            default:
               throw new NotImplementedException();
         }
      }

      //PD
      public bool InitialLDGLogic(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId) {
         var sql = $"[dbo].[ABSA_INITIALIZELGDLOGIC] '{productName.ToString()}', '{rowId}'";
         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               _dbContextSourceData.Database.ExecuteSqlCommand(sql);
               return true;
            case DbContextType.RFSTAGINGCONTEXT:
               _dbContextRFStaging.Database.ExecuteSqlCommand(sql);
               return true;
            default:
               throw new NotImplementedException();
         }
      }

      public List<ABSARetailPoolingLGDPERPDGROUP> GetLGDPERPDGroup(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId) {
         var result = new List<ABSARetailPoolingLGDPERPDGROUP>();
         var sql = $"[dbo].[ABSA_LGDPERPDGROUP] '{productName.ToString()}', '{rowId}'";

         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               result = _dbContextSourceData.Database.SqlQuery<ABSARetailPoolingLGDPERPDGROUP>(sql).ToList();
               break;
            case DbContextType.RFSTAGINGCONTEXT:
               result = _dbContextRFStaging.Database.SqlQuery<ABSARetailPoolingLGDPERPDGROUP>(sql).ToList();
               break;
            default:
               throw new NotImplementedException();
         }
         return (result);
      }

      public List<ABSARetailPoolingLGDSegment> GetLGDPerPDGroupData(ABSARetailPoolingLGDSegmentParam param) {
         var result = new List<ABSARetailPoolingLGDSegment>();
         var sql = $"[dbo].[ABSA_LGDPERPDGROUPDATA] {param.PdRowLebelIs}, '{param.UniqueSegment}', {param.SegmentRowId}, {param.ClusterId}, " +
                                                 $"'{param.RetailPoolingFileType.ToString()}', '{param.RowId}'";

         switch(param.DbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               result = _dbContextSourceData.Database.SqlQuery<ABSARetailPoolingLGDSegment>(sql).ToList();
               break;
            case DbContextType.RFSTAGINGCONTEXT:
               result = _dbContextRFStaging.Database.SqlQuery<ABSARetailPoolingLGDSegment>(sql).ToList();
               break;
            default:
               throw new NotImplementedException();
         }
         return (result);
      }

      public int GetSegmentLGDCount(string uniqueSegment, DbContextType dbContextType) {
         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               return _dbContextSourceData.ABSARetailPoolingLGDSegments.Where(x => string.Compare(x.UniqueSegment, uniqueSegment, true) == 0).Sum(x => x.LGDCount);
            case DbContextType.RFSTAGINGCONTEXT:
               return _dbContextRFStaging.ABSARetailPoolingLGDSegments.Where(x => string.Compare(x.UniqueSegment, uniqueSegment, true) == 0).Sum(x => x.LGDCount);
            default:
               throw new NotImplementedException();
         }
      }

      public bool UpdateLGDRatioInitialRankFinalPool(List<ABSARetailPoolingLGDSegment> contents, DbContextType dbContextType, Guid rowId) {
         var bulkUpdateContents = new List<ABSARetailPoolingBulkUpdate>();
         foreach(var item in contents) {
            bulkUpdateContents.Add(JsonConvert.DeserializeObject<ABSARetailPoolingBulkUpdate>(JsonConvert.SerializeObject(item)));
         }
         var sql = $"[dbo].[ABSA_UpdatePopulateLGDRatioInitialRankFinalPool] '{contents.FirstOrDefault().Product}', '{rowId}'";
         var update = BulkUpdatePoolingContents(bulkUpdateContents, dbContextType, sql);
         bulkUpdateContents.Clear();
         return (true);
      }

      private bool BulkUpdatePoolingContents<T>(List<T> contents, DbContextType dbContextType, string sql) where T : class {
         var bulkInsert = string.Empty;
         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               bulkInsert = BulkInsert(contents, _dbContextSourceData);
               break;
            case DbContextType.RFSTAGINGCONTEXT:
               bulkInsert = BulkInsert(contents, _dbContextRFStaging);
               break;
            default:
               throw new NotImplementedException();
         }
         if(bulkInsert.Contains("Success")) {
            return RatioInitialRankFinalPoolUpdate(dbContextType, sql);
         }
         return false;
      }

      private bool RatioInitialRankFinalPoolUpdate(DbContextType dbContextType, string sql) {
         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               _dbContextSourceData.Database.ExecuteSqlCommand(sql);
               return true;
            case DbContextType.RFSTAGINGCONTEXT:
               _dbContextRFStaging.Database.ExecuteSqlCommand(sql);
               return true;
            default:
               throw new NotImplementedException();
         }
      }

      //TM
      public bool InitialTMLogic(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId) {
         var sql = $"[dbo].[ABSA_INITIALIZETMLOGIC] '{productName.ToString()}', '{rowId}'";
         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               _dbContextSourceData.Database.ExecuteSqlCommand(sql);
               return true;
            case DbContextType.RFSTAGINGCONTEXT:
               _dbContextRFStaging.Database.ExecuteSqlCommand(sql);
               return true;
            default:
               throw new NotImplementedException();
         }
      }

      public List<ABSARetailPoolingGroupTM> GetTMPERPDLGDGroup(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId) {
         var result = new List<ABSARetailPoolingGroupTM>();
         var sql = $"[dbo].[ABSA_TMPERPDLGDGROUP] '{productName.ToString()}', '{rowId}'";

         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               result = _dbContextSourceData.Database.SqlQuery<ABSARetailPoolingGroupTM>(sql).ToList();
               break;
            case DbContextType.RFSTAGINGCONTEXT:
               result = _dbContextRFStaging.Database.SqlQuery<ABSARetailPoolingGroupTM>(sql).ToList();
               break;
            default:
               throw new NotImplementedException();
         }
         return (result);
      }

      public List<ABSARetailPoolingTMSegment> GetTMPerPDLGDGroupData(ABSARetailPolingTMSegmentParam param) {
         var result = new List<ABSARetailPoolingTMSegment>();
         var sql = $"[dbo].[ABSA_TMPERPDLGDGROUPDATA] {param.PdRowLebelId}, {param.LgdRowLabelId}, '{param.UniqueSegmentPDLGD}', {param.SegmentRowId}, " +
                                                      $"{param.ClusterId}, '{param.RetailPoolingFileType.ToString()}', '{param.RowId}'";

         switch(param.DbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               result = _dbContextSourceData.Database.SqlQuery<ABSARetailPoolingTMSegment>(sql).ToList();
               break;
            case DbContextType.RFSTAGINGCONTEXT:
               result = _dbContextRFStaging.Database.SqlQuery<ABSARetailPoolingTMSegment>(sql).ToList();
               break;
            default:
               throw new NotImplementedException();
         }
         return (result);
      }

      public int GetSegmentTMCount(string uniqueSegmentPDLGD, DbContextType dbContextType) {
         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               return _dbContextSourceData.ABSARetailPoolingTMSegments.Where(x => string.Compare(x.UniqueSegmentPDLGD, uniqueSegmentPDLGD, true) == 0).Sum(x => x.TMCount);
            case DbContextType.RFSTAGINGCONTEXT:
               return _dbContextRFStaging.ABSARetailPoolingTMSegments.Where(x => string.Compare(x.UniqueSegmentPDLGD, uniqueSegmentPDLGD, true) == 0).Sum(x => x.TMCount);
            default:
               throw new NotImplementedException();
         }
      }

      public bool UpdateTMRatioInitialRankFinalPool(List<ABSARetailPoolingTMSegment> contents, DbContextType dbContextType, Guid rowId) {
         var bulkUpdateContents = new List<ABSARetailPoolingBulkUpdate>();
         foreach(var item in contents) {
            bulkUpdateContents.Add(JsonConvert.DeserializeObject<ABSARetailPoolingBulkUpdate>(JsonConvert.SerializeObject(item)));
         }
         var sql = $"[dbo].[ABSA_UpdatePopulateTMRatioInitialRankFinalPool] '{contents.FirstOrDefault().Product}', '{rowId}'";
         var update = BulkUpdatePoolingContents(bulkUpdateContents, dbContextType, sql);
         bulkUpdateContents.Clear();
         return (true);
      }

      public bool UpdateInputFileFinalTMRank(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId) {
         var sql = $"[dbo].[ABSA_TMUPDATEFINALRANK] '{productName.ToString()}', '{rowId}'";
         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               _dbContextSourceData.Database.ExecuteSqlCommand(sql);
               return true;
            case DbContextType.RFSTAGINGCONTEXT:
               _dbContextRFStaging.Database.ExecuteSqlCommand(sql);
               return true;
            default:
               throw new NotImplementedException();
         }
      }
      public Guid CreateRetailProcessingProgress(DbContextType dbContextType, ABSARetailProcessingProgress retailProcessingProgress) {
         retailProcessingProgress.DateCreated = DateTime.Now;
         retailProcessingProgress.Status = ProgressStatus.Started.ToString();
         retailProcessingProgress.RowId = Guid.NewGuid();
         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               _dbContextSourceData.ABSARetailProcessingProgresses.AddOrUpdate(retailProcessingProgress);
               _dbContextSourceData.SaveChanges();
               break;
            case DbContextType.RFSTAGINGCONTEXT:
               _dbContextRFStaging.ABSARetailProcessingProgresses.Add(retailProcessingProgress);
               break;
            default:
               throw new NotImplementedException();
         }
         return (retailProcessingProgress.RowId);
      }

      public bool UpdateRetailProcessingProgress(DbContextType dbContextType, Guid rowId) {
         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               var item = _dbContextSourceData.ABSARetailProcessingProgresses.FirstOrDefault(x => string.Compare(x.RowId.ToString(), rowId.ToString(), true) == 0);
               item.Status = ProgressStatus.Completed.ToString();
               _dbContextSourceData.Entry(item).State = System.Data.Entity.EntityState.Modified;
               _dbContextSourceData.SaveChanges();
               break;
            case DbContextType.RFSTAGINGCONTEXT:
               var sourceItem = _dbContextRFStaging.ABSARetailProcessingProgresses.FirstOrDefault(x => string.Compare(x.RowId.ToString(), rowId.ToString(), true) == 0);
               sourceItem.Status = ProgressStatus.Completed.ToString();
               _dbContextRFStaging.Entry(sourceItem).State = System.Data.Entity.EntityState.Modified;
               _dbContextRFStaging.SaveChanges();
               break;
            default:
               throw new NotImplementedException();
         }
         return (true);
      }

      public bool CreateRetailProcessingProgressDetails(DbContextType dbContextType, Guid rowId, string message, ProgressStatus progressStatus) {
         var details = new ABSARetailProcessingProgressDetail() {
            RowId = rowId,
            Status = progressStatus.ToString(),
            Message = message,
            CreatedDate = DateTime.Now
         };
         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               _dbContextSourceData.ABSARetailProcessingProgressDetails.AddOrUpdate(details);
               _dbContextSourceData.SaveChanges();
               break;
            case DbContextType.RFSTAGINGCONTEXT:
               _dbContextRFStaging.ABSARetailProcessingProgressDetails.AddOrUpdate(details);
               break;
            default:
               throw new NotImplementedException();
         }
         return (true);
      }

      #endregion RETAIL_POOLING_LOGIC

      public void Dispose() {
         _dbContextRFStaging.Dispose();
         _dbContextSourceData.Dispose();
      }
   }

   
}
