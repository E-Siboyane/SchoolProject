using EconomicCapital.RF.DTO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DataAccess {
   public interface ISimPakDataProcessing {
      ApplicationDbContext GetApplicationDbContext();
      RFDatabaseContext GetRFStagingDbContext();
      IEnumerable<SP_Get_RFCounterParty> GetRFCounterParty();
      IEnumerable<Sp_Get_RFCounterPartyRSquared> GetRFCounterPartyRSquared();
      IEnumerable<Sp_Get_RFExposureUDV> GetRFExposureUDV();
      IEnumerable<Sp_Get_RFInstrumentPdsFlexible> GetRFInstrumentPdsFlexible();
      IEnumerable<Sp_Get_RFInstrumentRating> GetRFInstrumentRating();
      IEnumerable<Sp_Get_RFPortfolio> GetRFPortfolio();
      IEnumerable<Sp_Get_RFPortfolioDetail> GetRFPortfolioDetail();
      IEnumerable<Sp_Get_RFTermLoanBullet> GetRFTermLoanBullet();
      IEnumerable<Sp_Get_RFTermLoanAmortizing> GetRFTermLoanAmortizing();
      IEnumerable<Sp_Get_RFInstrumentPrice> GetRFInstrumentPrice();
      IEnumerable<Sp_Get_RFCovarianceModelFactorCoefficients> GetRFCovarianceModelFactorCoefficients();
      IEnumerable<SP_Get_RFYieldCurveHistory> GetRFYieldCurveHistory();
      bool SyncCountryFactorRSquared(int jobId, DbContextType dbContextType);
      List<string> GetRFBarCorWeights();
      DateTime GetRFImportAsOfDate();

      string BulkInsert<T>(List<T> bulkInsertItems, DbContext _dbContext) where T : class;
      string AddJob(Job jobDetails, DbContextType dbContextType);
      bool UpdateJob(int jobId, DbContextType dbContextType);
      bool AddImportAsOfDate(int jobId, DbContextType dbContextType);
      string AddAnalysisJobSettings(AnalysisJobManager analysisJobSettings, DbContextType dbContextType);
      bool UpdateAnalysisJobSettings(int analysisJobSettingsId, DbContextType dbContextType);
      List<AnalysisJobsDueForProcessing> GetStatgingJobsDueForProcessing();
      List<AnalysisJobReport> GetRFAnalyisJobReportDueForProcessing();
      bool UpdateRFUpdateAnalysisJobReportToProcessed(int id);
      string GenerateReport(int jobId, string asOfDate, string confidenceLevel, string storedProc);
      List<string> GetRFAnalysisJobsReportsSProcs();
      List<RetailPoolingInput> GetRetailPoolingInputContents(string productType, DbContextType dbContextType);
      bool RemoveRetailPoolingInputContents(DbContextType dbContextType);
  

      #region RETAIL_POOLING_LOGIC
      bool TruncateRetailPoolingInputTable(DbContextType dbContextType);
      List<RetailPoolingInput> GetFileContents(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId);
      List<ABSARetailPoolingPDSegment> GetFileContentsPD(DbContextType dbContextType, Guid rowId);
      List<ABSARetailPoolingLGDSegment> GetFileContentsLGD(DbContextType dbContextType, Guid rowId);
      List<ABSARetailPoolingTMSegment> GetFileContentsTM(DbContextType dbContextType, Guid rowId);
      List<ABSARetailPoolingConfigTypeDetail> GetRetailPoolingConfigs(DbContextType dbContextType);

      //PD
      ABSAPoolingLookup GetLookupData(RetailPoolingFileType productName, int totalcount, DbContextType dbContextType);
      bool InitialClusterPDLogic(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId);
      List<ABSARetailPoolingClusterSegment> GetClusterSegments(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId);
      List<ABSARetailPoolingPDSegment> GetSegmentContents(ABSARetailPoolingPDSegmentParam param);
      bool UpdateDRatioInitialRankFinalPool(List<ABSARetailPoolingPDSegment> segmentContents, DbContextType dbContextType, Guid rowId);
      bool UpdateInputFileFinalPDRank(RetailPoolingFileType productName,DbContextType dbContextType,  Guid rowId);

      //LGD
      bool InitialLDGLogic(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId);
      List<ABSARetailPoolingLGDPERPDGROUP> GetLGDPERPDGroup(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId);
      List<ABSARetailPoolingLGDSegment> GetLGDPerPDGroupData(ABSARetailPoolingLGDSegmentParam param);
      int GetSegmentLGDCount(string uniqueSegment, DbContextType dbContextType);
      bool UpdateLGDRatioInitialRankFinalPool(List<ABSARetailPoolingLGDSegment> contents, DbContextType dbContextType, Guid rowId);
      bool UpdateInputFileFinalLGDRank(RetailPoolingFileType productName,DbContextType dbContextType, Guid rowId);

      //TM
      bool InitialTMLogic(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId);
      List<ABSARetailPoolingGroupTM> GetTMPERPDLGDGroup(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId);
      List<ABSARetailPoolingTMSegment> GetTMPerPDLGDGroupData(ABSARetailPolingTMSegmentParam param);
      int GetSegmentTMCount(string uniqueSegmentPDLGD, DbContextType dbContextType);
      bool UpdateTMRatioInitialRankFinalPool(List<ABSARetailPoolingTMSegment> contents, DbContextType dbContextType, Guid rowId);
      bool UpdateInputFileFinalTMRank(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId);
      Guid CreateRetailProcessingProgress(DbContextType dbContextType, ABSARetailProcessingProgress retailProcessingProgress);
     bool CreateRetailProcessingProgressDetails(DbContextType dbContextType, Guid rowId, string message, ProgressStatus progressStatus);
      bool UpdateRetailProcessingProgress(DbContextType dbContextType, Guid rowId);
      #endregion RETAIL_POOLING_LOGIC
   }
}