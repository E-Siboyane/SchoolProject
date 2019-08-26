using EconomicCapital.RF.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.BusinessLogic {
    public interface ISimPakDataProcessingManager {
        string BulkRFImport(int jobId, DbContextType dbContextType, IDtoCommonMethod dtoCommonMethod, string fileContentDelimeter);
        string ConvertListToString<T>(List<T> itemsToConvertToString, IDtoCommonMethod dtoCommonMethod) where T : class;
        IEnumerable<Counterparty> GetRFCounterParty();
        IEnumerable<CounterpartyRSquared> GetRFCounterPartyRSquared();
        IEnumerable<ExposureUDV> GetRFExposureUDV();
        IEnumerable<InstrumentPdsFlexible> GetRFInstrumentPdsFlexible();
        IEnumerable<InstrumentRating> GetRFInstrumentRating();
        IEnumerable<Portfolio> GetRFPortfolio();
        IEnumerable<PortfolioDetail> GetRFPortfolioDetail();
        IEnumerable<TermLoanBullet> GetRFTermLoanBullet();
        IEnumerable<TermLoanAmortizing> GetRFTermLoanAmortizing();
        IEnumerable<InstrumentPrice> GetRFInstrumentPrice();
        List<ImportAsOfDate> GetRFImportAsOfDate();
        List<CovarianceModelFactorCoefficients> GetCovarianceModelFactorCoefficients();
        List<YieldCurveHistory> GetRFYieldCurveHistory();
        Correlation GetBarcorCorrelationInformation(int jobId);
        string BulkInsert<T>(List<T> bulkInsertItems, DbContextType dbContextType) where T : class;
        string AddJob(Job jobDetails, DbContextType dbContextType);
        bool UpdateJob(int jobId, DbContextType dbContextType);
        bool AddImportAsOfDate(int jobId, DbContextType dbContextType);
        string AddAnalysisJobSettings(AnalysisJobManager analysisJobSettings, DbContextType dbContextType); 
        List<AnalysisJobsDueForProcessing> GetAnalysisJobSettingsDuesForProcessing(DbContextType dbContextType);       
        bool UpdateAnalysisJobSettings(int analysisJobSettingsId, DbContextType dbContextType);
        List<AnalysisJobReport> GetRFAnalyisJobReportDueForProcessing();
        bool UpdateRFUpdateAnalysisJobReportToProcessed(int id);
        string GenerateReport(int jobId, string asOfDate, string confidenceLevel, string storedProc);
        List<string> GetRFAnalysisJobsReportsSProcs();
        bool SyncCountryFactorRSquared(int jobId, DbContextType dbContextType);

        List<RetailPoolingInput> GetRetailPoolingInput(RetailPoolingFileType retailPoolingFile, string fileContentDelimiter);
        List<RetailBucketNumber> GetRetailBucketNumbers(string fileContentDelimiter, string filterFileName);
        List<RetailPoolingInput> ProcessRetailPooling(List<RetailPoolingInput> contents, List<RetailBucketNumber> retailBucketNumbers);        
        List<RetailPoolingInput> GeneratePoolingRanks(List<RetailPoolingInput> contents, List<RetailBucketNumber> retailBucketNumbers);
        string ReadRetailPoolingFiles(RetailPoolingFileType productType, string filePath, string contentDelimeter, DbContextType dbContextType);
        bool ProcessPoolingRanks();
        bool ProcessPoolingRanks1();
    }
}
