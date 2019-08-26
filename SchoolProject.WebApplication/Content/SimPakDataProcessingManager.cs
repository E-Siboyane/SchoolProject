using EconomicCapital.RF.DataAccess;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using EconomicCapital.RF.DTO;
using Newtonsoft.Json;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Diagnostics;

namespace EconomicCapital.RF.BusinessLogic {
   public class SimPakDataProcessingManager : ISimPakDataProcessingManager {
      private INinjectKernel _ninjectKernelBindings;
      private readonly ISimPakDataProcessing _iSourceDataProcessing;
      private readonly IFileManager fileManager;
      const string JOBSTATUS = "Importing Complete";

      private string BASEDIRECTORY { get { return ConfigurationManager.AppSettings.Get("SharedBaseDirectory"); } }
      private string RF_FILENAME_ABBR { get { return ConfigurationManager.AppSettings.Get("RF_FileName_Prefix"); } }
      private string CORRELATIONMODELTYPENAME { get { return ConfigurationManager.AppSettings.Get("FactorModel"); } }
      private string CORRELATIONMODELESTIMATENAME { get { return ConfigurationManager.AppSettings.Get("CorrelationName"); } }
      private decimal STANDARDDEVIATION { get { return 1.0M; } }
      private string INDUSTRYFACTORNAME { get { return "ABSA_INDUSTRY".ToUpper(); } }


      public SimPakDataProcessingManager(INinjectKernel ninjectKernel) {
         _ninjectKernelBindings = ninjectKernel;
         IKernel kernel = _ninjectKernelBindings.GetNinjectBindings();
         _iSourceDataProcessing = kernel.Get<ISimPakDataProcessing>();
         fileManager = kernel.Get<IFileManager>();
      }

      public IEnumerable<Counterparty> GetRFCounterParty() {
         return (JsonConvert.DeserializeObject<List<Counterparty>>(JsonConvert.SerializeObject(_iSourceDataProcessing.GetRFCounterParty())));
      }

      public IEnumerable<CounterpartyRSquared> GetRFCounterPartyRSquared() {
         return JsonConvert.DeserializeObject<List<DTO.CounterpartyRSquared>>(JsonConvert.SerializeObject(_iSourceDataProcessing.GetRFCounterPartyRSquared()));
      }

      public IEnumerable<ExposureUDV> GetRFExposureUDV() {
         return JsonConvert.DeserializeObject<List<DTO.ExposureUDV>>(JsonConvert.SerializeObject(_iSourceDataProcessing.GetRFExposureUDV()));
      }

      public IEnumerable<InstrumentPdsFlexible> GetRFInstrumentPdsFlexible() {
         return JsonConvert.DeserializeObject<List<InstrumentPdsFlexible>>(JsonConvert.SerializeObject(_iSourceDataProcessing.GetRFInstrumentPdsFlexible()));
      }

      public IEnumerable<InstrumentRating> GetRFInstrumentRating() {
         return JsonConvert.DeserializeObject<List<InstrumentRating>>(JsonConvert.SerializeObject(_iSourceDataProcessing.GetRFInstrumentRating()));
      }

      public IEnumerable<Portfolio> GetRFPortfolio() {
         return JsonConvert.DeserializeObject<List<Portfolio>>(JsonConvert.SerializeObject(_iSourceDataProcessing.GetRFPortfolio()));
      }

      public IEnumerable<PortfolioDetail> GetRFPortfolioDetail() {
         return JsonConvert.DeserializeObject<List<PortfolioDetail>>(JsonConvert.SerializeObject(_iSourceDataProcessing.GetRFPortfolioDetail()));
      }

      public IEnumerable<TermLoanBullet> GetRFTermLoanBullet() {
         return JsonConvert.DeserializeObject<List<TermLoanBullet>>(JsonConvert.SerializeObject(_iSourceDataProcessing.GetRFTermLoanBullet()));
      }

      public IEnumerable<TermLoanAmortizing> GetRFTermLoanAmortizing() {
         return JsonConvert.DeserializeObject<List<TermLoanAmortizing>>(JsonConvert.SerializeObject(_iSourceDataProcessing.GetRFTermLoanAmortizing()));
      }

      public IEnumerable<InstrumentPrice> GetRFInstrumentPrice() {
         return JsonConvert.DeserializeObject<List<InstrumentPrice>>(JsonConvert.SerializeObject(_iSourceDataProcessing.GetRFInstrumentPrice()));
      }

      public List<ImportAsOfDate> GetRFImportAsOfDate() {
         var importAsOfDates = new List<ImportAsOfDate>();
         var result = _iSourceDataProcessing.GetRFImportAsOfDate();
         importAsOfDates.Add(new ImportAsOfDate() {
            AsOfDate = result.Date
         });
         return importAsOfDates;
      }

      public List<CovarianceModelFactorCoefficients> GetCovarianceModelFactorCoefficients() {
         return JsonConvert.DeserializeObject<List<CovarianceModelFactorCoefficients>>(JsonConvert.SerializeObject(_iSourceDataProcessing.GetRFCovarianceModelFactorCoefficients()));
      }

      public List<YieldCurveHistory> GetRFYieldCurveHistory() {
         return JsonConvert.DeserializeObject<List<YieldCurveHistory>>(JsonConvert.SerializeObject(_iSourceDataProcessing.GetRFYieldCurveHistory()));
      }

      public string BulkInsert<T>(List<T> bulkInsertItems, DbContextType dbContextType) where T : class {
         var result = string.Empty;
         switch(dbContextType) {
            case DbContextType.RFSTAGINGCONTEXT:
               result = _iSourceDataProcessing.BulkInsert(bulkInsertItems, _iSourceDataProcessing.GetRFStagingDbContext());
               break;
            case DbContextType.SOURCEDATACONTEXT:
               result = _iSourceDataProcessing.BulkInsert(bulkInsertItems, _iSourceDataProcessing.GetApplicationDbContext());
               break;
            default:
               break;
         }
         return result;
      }

      public string ConvertListToString<T>(List<T> itemsToConvertToString, IDtoCommonMethod dtoCommonMethod) where T : class {
         return dtoCommonMethod.ConvertListToString(itemsToConvertToString);
      }

      public string BulkRFImport(int jobId, DbContextType dbContextType, IDtoCommonMethod dtoCommonMethod, string fileContentDelimeter) {
         var fileName = string.Format("{0}{1}", RF_FILENAME_ABBR, dtoCommonMethod.GetType().Name.ToString());
         var baseDirectoryLastFolder = fileManager.GetLastFolder(BASEDIRECTORY);
         var filePath = fileManager.SearchFiles(baseDirectoryLastFolder).FirstOrDefault(x => x.ToLower().Contains(fileName.ToLower()));
         var contents = fileManager.ReadCSVFile(filePath);
         var noContentMessage = string.Format("File : {0}, contains no contents or the file name provided was not found in the input folder", fileName.ToLower());
         return contents.Count > 0 ? FormatContentAndBulkUpload(jobId, dbContextType, dtoCommonMethod, contents, fileContentDelimeter) : noContentMessage;
      }

      private string FormatContentAndBulkUpload(int jobId, DbContextType dbContextType, IDtoCommonMethod dtoCommonMethod, List<string> bulkImportItems, string fileContentDelimeter) {
         var itemsToImport = dtoCommonMethod.ConvertToModel(bulkImportItems, jobId, fileContentDelimeter);
         var classWithDifferentOutput = new List<string> { "Portfolio", "ImportAsOfDate" }; //Bad
         var bulkImportOutput = BulkInsert(itemsToImport, dbContextType);
         if((classWithDifferentOutput.Contains(dtoCommonMethod.GetType().Name)))
            bulkImportOutput = string.Format("{0}|{1}", bulkImportOutput, GetExtraInformation(bulkImportItems));
         return bulkImportOutput;
      }

      private string GetExtraInformation(List<string> bulkImportItems) {
         return bulkImportItems.FirstOrDefault().Split(',')[0];
      }

      public string AddJob(Job jobDetails, DbContextType dbContextType) {
         return _iSourceDataProcessing.AddJob(jobDetails, dbContextType);
      }

      public string AddAnalysisJobSettings(AnalysisJobManager analysisJobSettings, DbContextType dbContextType) {
         return _iSourceDataProcessing.AddAnalysisJobSettings(analysisJobSettings, dbContextType);
      }
      public bool UpdateAnalysisJobSettings(int analysisJobSettingsId, DbContextType dbContextType) {
         return _iSourceDataProcessing.UpdateAnalysisJobSettings(analysisJobSettingsId, dbContextType);
      }

      public bool UpdateJob(int jobId, DbContextType dbContextType) {
         return _iSourceDataProcessing.UpdateJob(jobId, dbContextType);
      }

      public bool AddImportAsOfDate(int jobId, DbContextType dbContextType) {
         return _iSourceDataProcessing.AddImportAsOfDate(jobId, dbContextType);
      }

      public List<AnalysisJobsDueForProcessing> GetAnalysisJobSettingsDuesForProcessing(DbContextType dbContextType) {
         switch(dbContextType) {
            case DbContextType.SOURCEDATACONTEXT:
               var result = _iSourceDataProcessing.GetApplicationDbContext().AnalysisJobManagers.Include(x => x.Job).
                                     Where(x => string.Compare(x.Job.Status.Trim(), JOBSTATUS.Trim(), true) == 0 && !x.IsProcessed).ToList();
               return JsonConvert.DeserializeObject<List<AnalysisJobsDueForProcessing>>(JsonConvert.SerializeObject(result));
            case DbContextType.RFSTAGINGCONTEXT:
               return _iSourceDataProcessing.GetStatgingJobsDueForProcessing();
            default:
               throw new NotImplementedException();
         }
      }

      public List<AnalysisJobReport> GetRFAnalyisJobReportDueForProcessing() {
         return _iSourceDataProcessing.GetRFAnalyisJobReportDueForProcessing();
      }

      public bool UpdateRFUpdateAnalysisJobReportToProcessed(int id) {
         return _iSourceDataProcessing.UpdateRFUpdateAnalysisJobReportToProcessed(id);
      }

      public string GenerateReport(int jobId, string asOfDate, string confidenceLevel, string storedProc) {
         return _iSourceDataProcessing.GenerateReport(jobId, asOfDate, confidenceLevel, storedProc);
      }

      public List<string> GetRFAnalysisJobsReportsSProcs() {
         return _iSourceDataProcessing.GetRFAnalysisJobsReportsSProcs();
      }

      public Correlation GetBarcorCorrelationInformation(int jobId) {
         var correlation = new Correlation();
         var correlationSourceData = _iSourceDataProcessing.GetRFBarCorWeights();
         var hearder = correlationSourceData.FirstOrDefault().Split(',');
         var contents = correlationSourceData.Skip(1).ToList();

         if(correlationSourceData.Count > 1) {
            correlation.CountryFactorBetas = CreateCountryFactorBetas(jobId, correlationSourceData);
            correlation.CountryFactors = CreateCountryFactors(jobId, correlationSourceData);
            correlation.CommonFactors = CreateCommonFactors(jobId, hearder);
            correlation.CorrelationModels = CreateCorrelationModel(jobId);
            correlation.CommonFactorStandardDeviations = CreateCommonFactorStandardDeviations(jobId, hearder);
            correlation.CountryFactorRSquareds = CreateCountryFactorRSquared(jobId, correlationSourceData);
            correlation.ImportAsOfDate = CreateImportAsOfDate(jobId);
            correlation.IndustryFactorBetas = CreateIndustryFactorBetas(jobId, correlationSourceData.Take(2).ToList());
            correlation.IndustryFactorRSquareds = CreateIndustryFactorRSquared(jobId);
            correlation.IndustryFactors = CreateIndustryFactors(jobId);
         }
         return correlation;
      }

      private List<CountryFactorBetas> CreateCountryFactorBetas(int jobId, List<string> countryFactorBetaItems) {
         var countryFactorBetas = new List<CountryFactorBetas>();

         if(countryFactorBetaItems.Count > 1) {
            var hearder = countryFactorBetaItems.FirstOrDefault().Split(',');
            var contents = countryFactorBetaItems.Skip(1).ToList();
            foreach(var barcorWeightRow in contents) {
               var row = barcorWeightRow.Split(',');
               for(int i = 0; i < row.Length; i++) {
                  if(i > 0) {  //Index zero is the Key field
                     var item = new CountryFactorBetas() {
                        CommonFactorName = hearder[i].ToUpper(),
                        CountryFactorName = row[0],
                        Beta = decimal.Parse(row[i], System.Globalization.NumberStyles.Float),
                        CorrelationModelEstimateName = CORRELATIONMODELESTIMATENAME,
                        JobId = jobId
                     };
                     countryFactorBetas.Add(item);
                  }
               }
            }
         }
         return (countryFactorBetas);
      }

      private List<IndustryFactorBetas> CreateIndustryFactorBetas(int jobId, List<string> countryFactorBetaItems) {
         var industryFactorBetas = new List<IndustryFactorBetas>();

         if(countryFactorBetaItems.Count > 1) {
            var hearder = countryFactorBetaItems.FirstOrDefault().Split(',');
            var contents = countryFactorBetaItems.Skip(1).Take(1).ToList();
            foreach(var barcorWeightRow in contents) {
               var row = barcorWeightRow.Split(',');
               for(int i = 0; i < row.Length; i++) {
                  if(i > 0) {  //Index zero is the Key field
                     var item = new IndustryFactorBetas() {
                        CommonFactorName = hearder[i].ToUpper(),
                        IndustryFactorName = INDUSTRYFACTORNAME.ToUpper(), //read from config file
                        Beta = 0.0M,
                        CorrelationModelEstimateName = CORRELATIONMODELESTIMATENAME,
                        JobId = jobId
                     };
                     industryFactorBetas.Add(item);
                  }
               }
               break;
            }
         }
         return (industryFactorBetas);
      }

      private List<ImportAsOfDate> CreateImportAsOfDate(int jobId) {
         var asOfDate = _iSourceDataProcessing.GetRFImportAsOfDate();
         var asOfDates = new List<ImportAsOfDate>();
         asOfDates.Add(new ImportAsOfDate() { JobId = jobId, AsOfDate = asOfDate.Date });
         return (asOfDates);
      }

      private List<CountryFactors> CreateCountryFactors(int jobId, List<string> countryFactorBetaItems) {
         var countryFactors = new List<CountryFactors>();
         if(countryFactorBetaItems.Count > 1) {
            var contents = countryFactorBetaItems.Skip(1).ToList();
            foreach(var barcorWeightRow in contents) {
               countryFactors.Add(new CountryFactors() {
                  JobId = jobId,
                  FactorSetName = CORRELATIONMODELESTIMATENAME,
                  CountryFactorName = barcorWeightRow.Split(',')[0]
               });
            }
         }
         return (countryFactors.Count == 0) ? countryFactors : countryFactors.GroupBy(x => x.CountryFactorName).Select(x => x.FirstOrDefault()).ToList();
      }

      private List<IndustryFactors> CreateIndustryFactors(int jobId) {
         var industryFactors = new List<IndustryFactors>();
         industryFactors.Add(new IndustryFactors() {
            JobId = jobId,
            FactorSetName = CORRELATIONMODELESTIMATENAME,
            IndustryFactorName = INDUSTRYFACTORNAME
         });
         return industryFactors.GroupBy(x => x.IndustryFactorName).Select(x => x.FirstOrDefault()).ToList();
      }

      private List<CommonFactors> CreateCommonFactors(int jobId, string[] commonFactorNames) {
         var commonFactors = new List<CommonFactors>();
         for(int i = 0; i < commonFactorNames.Length; i++) {
            if(i > 0) {
               commonFactors.Add(new CommonFactors() {
                  JobId = jobId,
                  FactorSetName = CORRELATIONMODELESTIMATENAME.ToUpper(),
                  CommonFactorName = commonFactorNames[i].ToUpper()
               });
            }
         }
         return (commonFactors);
      }

      private List<CommonFactorStandardDeviation> CreateCommonFactorStandardDeviations(int jobId, string[] commonFactorNames) {
         var commonFactorStandardDeviations = new List<CommonFactorStandardDeviation>();
         for(int i = 0; i < commonFactorNames.Length; i++) {
            if(i > 0) {
               commonFactorStandardDeviations.Add(new CommonFactorStandardDeviation() {
                  JobId = jobId,
                  CorrelationModelEstimateName = CORRELATIONMODELESTIMATENAME.ToUpper(),
                  CommonFactorName = commonFactorNames[i].ToUpper(),
                  StandardDeviation = 1.0M
               });
            }
         }
         return (commonFactorStandardDeviations);
      }

      private List<CorrelationModels> CreateCorrelationModel(int jobId) {
         var correlationModels = new List<CorrelationModels>();
         correlationModels.Add(new CorrelationModels {
            FactorSetName = CORRELATIONMODELESTIMATENAME,
            CorrelationModelTypeName = CORRELATIONMODELTYPENAME,
            JobId = jobId,
            CorrelationModelEstimateName = CORRELATIONMODELESTIMATENAME
         });
         return (correlationModels);
      }

      private List<CountryFactorRSquared> CreateCountryFactorRSquared(int jobId, List<string> countryFactorBetaItems) {
         var countryFactorRSquareds = new List<CountryFactorRSquared>();
         if(countryFactorBetaItems.Count > 1) {
            var contents = countryFactorBetaItems.Skip(1).ToList();
            foreach(var barcorWeightRow in contents) {
               countryFactorRSquareds.Add(new CountryFactorRSquared() {
                  JobId = jobId,
                  CorrelationModelEstimateName = CORRELATIONMODELESTIMATENAME,
                  countryFactorName = barcorWeightRow.Split(',')[0],
                  countryFactorRSquared = 1.0M
               });
            }
         }
         return (countryFactorRSquareds.Count == 0) ? countryFactorRSquareds : countryFactorRSquareds.GroupBy(x => x.countryFactorName).Select(x => x.FirstOrDefault()).ToList();
      }

      private List<IndustryFactorRSquared> CreateIndustryFactorRSquared(int jobId) {
         var industryFactorRSquareds = new List<IndustryFactorRSquared>();
         industryFactorRSquareds.Add(new IndustryFactorRSquared() {
            JobId = jobId,
            CorrelationModelEstimateName = CORRELATIONMODELESTIMATENAME,
            IndustryFactorName = INDUSTRYFACTORNAME.ToUpper(),
            industryFactorRSquared = 1.0M
         });
         return (industryFactorRSquareds.Count == 0) ? industryFactorRSquareds : industryFactorRSquareds.GroupBy(x => x.IndustryFactorName).Select(x => x.FirstOrDefault()).ToList();
      }

      public bool SyncCountryFactorRSquared(int jobId, DbContextType dbContextType) {
         return _iSourceDataProcessing.SyncCountryFactorRSquared(jobId, dbContextType);
      }

      public bool ProcessPoolingRanks() {
         var result = false;
         var inputFilesbaseDirectory = ConfigurationManager.AppSettings.Get("SharedBaseDirectory");
         var fileContentDelimeter = ConfigurationManager.AppSettings.Get("InputFileContentDelimeter");
         var expectedTotalFile = ConfigurationManager.AppSettings.Get("TotalRequiredFiles");
         var retailBucketContentDelimeter = ConfigurationManager.AppSettings.Get("RetailBucketContentDelimeter");
         var bucketFileName = ConfigurationManager.AppSettings.Get("BucketFileName");
         var outputFilesBaseDirectory = ConfigurationManager.AppSettings.Get("OutputBaseDirectory");

         var exclude = new List<string>() { "CH", "CC" };
         var productTypes = Enum.GetValues(typeof(RetailPoolingFileType)).Cast<RetailPoolingFileType>().Select(v => v.ToString()).Where(x => !exclude.Contains(x)).ToList();

         var chekFiles = fileManager.GetProcessingDirectoryFiles(inputFilesbaseDirectory);

         if(int.Parse(expectedTotalFile) == chekFiles.Count) {
            var outputFolderName = string.Format("Output_{0}_{1}", DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"), DateTime.Now.Ticks.ToString("x"));
            var retailBucket = GetRetailBucketNumbers(retailBucketContentDelimeter, bucketFileName);
            foreach(var product in productTypes) {
               var inputType = (RetailPoolingFileType)Enum.Parse(typeof(RetailPoolingFileType), product);
               var poolingInputFile = GetRetailPoolingInput(inputType, fileContentDelimeter);

               var generateRanks = GeneratePoolingRanks(poolingInputFile, retailBucket);
               if(generateRanks.Count == poolingInputFile.Count) {
                  var fileName = string.Format("{0}_RANKING_PROCESSING_{1}_{2}", inputType.ToString().ToUpper(), DateTime.Now.ToString("yyyyMMddmmss"), DateTime.Now.Ticks.ToString("x"));
                  var createFile = fileManager.CreatePoolingOutputFiles(fileName, outputFilesBaseDirectory, generateRanks, outputFolderName);
               }
               poolingInputFile.Clear();
            }

            var chFiles = GetRetailPoolingInput(RetailPoolingFileType.CH, fileContentDelimeter);
            var chFilesProcessing = GeneratePoolingRanks(chFiles, retailBucket);
            if(chFilesProcessing.Count == chFiles.Count) {
               var fileName = string.Format("{0}_RANKING_PROCESSING_{1}_{2}", RetailPoolingFileType.CH.ToString().ToUpper(), DateTime.Now.ToString("yyyyMMddmmss"), DateTime.Now.Ticks.ToString("x"));
               var createFile = fileManager.CreatePoolingOutputFiles(fileName, outputFilesBaseDirectory, chFilesProcessing, outputFolderName);
            }

            var ccFiles = GetRetailPoolingInput(RetailPoolingFileType.CC, fileContentDelimeter);
            var ccFilesProcessing = GeneratePoolingRanks(ccFiles, retailBucket);
            if(ccFilesProcessing.Count == ccFiles.Count) {
               var fileName = string.Format("{0}_RANKING_PROCESSING_{1}_{2}", RetailPoolingFileType.CC.ToString().ToUpper(), DateTime.Now.ToString("yyyyMMddmmss"), DateTime.Now.Ticks.ToString("x"));
               var createFile = fileManager.CreatePoolingOutputFiles(fileName, outputFilesBaseDirectory, ccFilesProcessing, outputFolderName);
            }

            result = true;
         }
         return (result);
      }


      public bool ProcessPoolingRanks1() {
         var dbContextType = (DbContextType)Enum.Parse(typeof(DbContextType), ConfigurationManager.AppSettings.Get("DatabaseServer"), true);
         var result = false;
         var inputFilesbaseDirectory = ConfigurationManager.AppSettings.Get("SharedBaseDirectory");
         var fileContentDelimeter = ConfigurationManager.AppSettings.Get("InputFileContentDelimeter");
         var expectedTotalFile = ConfigurationManager.AppSettings.Get("TotalRequiredFiles");
         var retailBucketContentDelimeter = ConfigurationManager.AppSettings.Get("RetailBucketContentDelimeter");
         var bucketFileName = ConfigurationManager.AppSettings.Get("BucketFileName");
         var outputFilesBaseDirectory = ConfigurationManager.AppSettings.Get("OutputBaseDirectory");

         var exclude = new List<string>() { "CH", "CC" };
         var productTypes = Enum.GetValues(typeof(RetailPoolingFileType)).Cast<RetailPoolingFileType>().Select(v => v.ToString()).Where(x => !exclude.Contains(x)).ToList();

         var chekFiles = fileManager.GetProcessingDirectoryFiles(inputFilesbaseDirectory);

         if(int.Parse(expectedTotalFile) == chekFiles.Count) {
            var outputFolderName = string.Format("Output_{0}_{1}", DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"), DateTime.Now.Ticks.ToString("x"));
            var retailBucket = GetRetailBucketNumbers(retailBucketContentDelimeter, bucketFileName);
            foreach(var product in exclude) {
               var inputType = (RetailPoolingFileType)Enum.Parse(typeof(RetailPoolingFileType), product);

               var fileName = string.Format("_{0}_", product);
               var baseDirectoryLastFolder = fileManager.GetLastFolder(BASEDIRECTORY);
               var filePath = fileManager.SearchFiles(baseDirectoryLastFolder).FirstOrDefault(x => x.ToLower().Contains(fileName.ToLower()));
               var readFiles = ReadRetailPoolingFiles(inputType, filePath, fileContentDelimeter, dbContextType);

               var poolingInputFile = _iSourceDataProcessing.GetRetailPoolingInputContents(readFiles, dbContextType);

               var generateRanks = GeneratePoolingRanks(poolingInputFile, retailBucket);
               if(generateRanks.Count == poolingInputFile.Count) {
                  var outputFileName = string.Format("{0}_RANKING_PROCESSING_{1}_{2}", inputType.ToString().ToUpper(), DateTime.Now.ToString("yyyyMMddmmss"), DateTime.Now.Ticks.ToString("x"));
                  var createFile = fileManager.CreatePoolingOutputFiles(outputFileName, outputFilesBaseDirectory, generateRanks, outputFolderName);
               }
               poolingInputFile.Clear();
               var deleteContents = _iSourceDataProcessing.RemoveRetailPoolingInputContents(dbContextType);
            }
            result = true;
         }
         return (result);
      }

      public List<RetailPoolingInput> GeneratePoolingRanks(List<RetailPoolingInput> contents, List<RetailBucketNumber> retailBucketNumbers) {
         //var orderContentsStepPdLogic = contents.OrderBy(x => x.EC_CLUSTER).ThenBy(x => x.EC_SEG).ThenBy(x => x.CumulativePDs).ToList();
         ////PD
         //var row = 1;
         //var previousPD = orderContentsStepPdLogic.FirstOrDefault().CumulativePDs;
         //foreach (var currentItem in orderContentsStepPdLogic) {
         //    if (previousPD != currentItem.CumulativePDs) {
         //        previousPD = currentItem.CumulativePDs;
         //        row++;
         //    }
         //    currentItem.RankByPD = row;
         //}

         ////LGD
         //row = 1;
         //var orderContentsStepLGDLogic = orderContentsStepPdLogic.OrderBy(x => x.EC_CLUSTER).ThenBy(x => x.EC_SEG).ThenBy(x => x.RankByPD).ThenBy(x => x.LGD).ToList();           
         //var previousLgd = orderContentsStepLGDLogic.FirstOrDefault().LGD;
         //var previousRankedPd = orderContentsStepLGDLogic.FirstOrDefault().RankByPD;
         //foreach (var item in orderContentsStepLGDLogic) {
         //    if (previousLgd != item.LGD) {
         //        previousLgd = item.LGD;
         //        row++;
         //    }
         //    if (previousRankedPd != item.RankByPD) {
         //        previousRankedPd = item.RankByPD;
         //        row--;
         //        row++;
         //    }
         //    item.RankkByLGD = row;
         //}

         //////TM
         //row = 1;
         //var OrderContentsStepTMLogic = orderContentsStepLGDLogic.OrderBy(x => x.EC_CLUSTER).ThenBy(x => x.EC_SEG).ThenBy(x => x.RankByPD).ThenBy(x => x.RankkByLGD).ThenBy(x => x.TimeToMaturity).ToList();
         //var previousTM = OrderContentsStepTMLogic.FirstOrDefault().TimeToMaturity;
         //var previousRankedLGD = OrderContentsStepTMLogic.FirstOrDefault().RankkByLGD;
         //foreach (var item in OrderContentsStepTMLogic) {
         //    if (previousTM != item.TimeToMaturity) {
         //        previousTM = item.TimeToMaturity;
         //        row++;
         //    }
         //    if (previousRankedLGD != item.RankkByLGD) {
         //        previousRankedLGD = item.RankkByLGD;
         //        row++;
         //    }
         //    item.RankByTM = row;
         //}

         //row = 1;
         //var OrderByPD = OrderContentsStepTMLogic.OrderBy(x => x.EC_CLUSTER).ThenBy(e => e.EC_SEG).ThenBy(x =>x.CumulativePDs).ToList();
         //foreach (var item in OrderByPD) {
         //    item.RowId = (row++);
         //}


         //var pdResults = (RankBasedOnPD(OrderByPD, retailBucketNumbers));
         //row = 1;
         //var OrderByLGD = pdResults.OrderBy(x => x.EC_CLUSTER).ThenBy(e => e.EC_SEG).ThenBy(x => x.LGD).ToList();
         //foreach (var item in OrderByLGD) {
         //    item.RowId = (row++);
         //}
         //var lgdResult = (RankBasedOnLGD(OrderByLGD, retailBucketNumbers));


         //row = 1;
         //var OrderByTM = lgdResult.OrderBy(x => x.EC_CLUSTER).ThenBy(e => e.EC_SEG).ThenBy(x => x.TimeToMaturity).ToList();
         //foreach (var item in OrderByTM) {
         //    item.RowId = (row++);
         //}
         //var tmResult = (RankBasedOnTM(OrderByTM, retailBucketNumbers));
         //return (tmResult);
         throw new NotImplementedException();
      }


      private List<RetailPoolingInput> RankBasedOnPD(List<RetailPoolingInput> contents, List<RetailBucketNumber> retailBucketNumbers) {
         //var contentsCount = contents.GroupBy(x => new { x.PROD }, (key, groupResult) => new {
         //    Segment = groupResult.FirstOrDefault().EC_SEG, ResultCount = groupResult.Count(), Prod = contents.FirstOrDefault().PROD
         //}).ToList();

         ////expect one Prod but if more take the first one.
         //if (contentsCount.Count >= 2) {
         //    var count = contentsCount.Sum(x => x.ResultCount);
         //    var segment = contentsCount.FirstOrDefault().Segment;
         //    var prod = contentsCount.FirstOrDefault().Prod;
         //    contentsCount.Clear();
         //    contentsCount.Add(new { Segment = segment, ResultCount = count, Prod = prod });
         //}

         //var filterRetailBackeList = new List<RetailBucketNumber>();
         //var pd = 0.0M;

         //var poolingIntervals = new List<PoolingInterval>();
         //foreach (var item in contentsCount) {
         //    filterRetailBackeList = retailBucketNumbers.Where(x => string.Compare(x.Prod, item.Prod, true) == 0).ToList();
         //    foreach (var division in filterRetailBackeList) {
         //        var value = (decimal.Parse(division.PD_buckets) * filterRetailBackeList.Count);
         //        pd = Math.Ceiling(item.ResultCount / (decimal.Parse(division.PD_buckets) * filterRetailBackeList.Count));
         //        var intervalContent = string.Empty;
         //        for (int i = 0; i <= value; i++) {
         //            if (i != 0)
         //                intervalContent += (pd * i).ToString() + "|";
         //        }
         //        poolingIntervals.Add(new PoolingInterval() {
         //            Prod = item.Prod,
         //            Seg = item.Segment,
         //            Type = "PD",
         //            Interval = intervalContent.RemoveLastString()
         //        });
         //    }
         //}

         ////PD
         //var lgdPool = poolingIntervals.FirstOrDefault(x => string.Compare(x.Type, "PD", true) == 0);
         //var intervals = lgdPool.Interval.Split('|').ToArray();
         //var processNewRanking = new List<ProcessNewRanking>();

         //for (int i = 0; i < intervals.Length; i++) {
         //    var newD = new ProcessNewRanking();
         //    newD.RowId = i;
         //    if (i == 0) {
         //        newD.Minimum = i + 1;
         //        newD.Maximum = int.Parse(intervals[i]);
         //    }
         //    else {
         //        newD.Minimum = int.Parse(intervals[i]) - int.Parse(intervals[0]);
         //        newD.Maximum = int.Parse(intervals[(i)]);
         //    }
         //    processNewRanking.Add(newD);
         //}

         //foreach (var c in contents) {
         //    var newRowIdRecord = processNewRanking.Where(x => c.RowId >= x.Minimum).Where(x => c.RowId <= x.Maximum).FirstOrDefault();
         //    c.NewPDRank = newRowIdRecord.RowId;
         //}

         //var properOrdered = contents.OrderBy(x => x.NewPDRank).ToList();
         //var finalContents = new List<RetailPoolingInput>();
         //var items = new List<RetailPoolingInput>();
         //var secondItems = new List<RetailPoolingInput>();
         //var intervalMaximumBreakPoint = processNewRanking.FirstOrDefault().Maximum;

         //if (processNewRanking.Count > 1) {
         //    for (int i = 0; i < processNewRanking.Count; i++) {
         //        var firstSetItemsNumber = processNewRanking[i];
         //        var secondSetItemsNumber = processNewRanking[i + 1];

         //        if (i == 0) {
         //            items = properOrdered.Take(firstSetItemsNumber.Maximum).ToList();
         //            secondItems = properOrdered.Skip(secondSetItemsNumber.Minimum).Take(secondSetItemsNumber.Minimum).ToList();
         //            items.ForEach(x => { x.FinalPDRank = i; });
         //            finalContents.AddRange(items);

         //            secondItems.ForEach(x => { x.FinalPDRank = (i + 1); });

         //            if (secondItems.Count > 0) {
         //                if (items.LastOrDefault().CumulativePDs == secondItems.FirstOrDefault().CumulativePDs) {
         //                    var matchingPDs = secondItems.Where(x => x.CumulativePDs == items.LastOrDefault().CumulativePDs).ToList();
         //                    matchingPDs.ForEach(x => { x.FinalPDRank = i; });
         //                    finalContents.AddRange(matchingPDs);
         //                    var remaining = secondItems.Where(x => x.CumulativePDs != items.LastOrDefault().CumulativePDs).ToList();
         //                    if (remaining.Count() > 0)
         //                        finalContents.AddRange(remaining);
         //                }
         //                else {
         //                    finalContents.AddRange(secondItems);
         //                }
         //            }
         //        }
         //        else {
         //            items = finalContents;
         //            secondItems = properOrdered.Skip(secondSetItemsNumber.Minimum).Take(intervalMaximumBreakPoint).ToList();
         //            secondItems.ForEach(x => { x.FinalPDRank = (i + 1); });

         //            if (secondItems.Count > 0) {
         //                if (items.LastOrDefault().CumulativePDs == secondItems.FirstOrDefault().CumulativePDs) {
         //                    var matchingPDs = secondItems.Where(x => x.CumulativePDs == items.LastOrDefault().CumulativePDs).ToList();
         //                    matchingPDs.ForEach(x => { x.FinalPDRank = items.LastOrDefault().FinalPDRank; });
         //                    finalContents.AddRange(matchingPDs);
         //                    var remaining = secondItems.Where(x => x.CumulativePDs != items.LastOrDefault().CumulativePDs).ToList();
         //                    if (remaining.Count() > 0)
         //                        finalContents.AddRange(remaining);
         //                }
         //                else {
         //                    finalContents.AddRange(secondItems);
         //                }
         //            }
         //        }

         //        if (finalContents.Count() >= properOrdered.Count())
         //            break;
         //    }
         //}
         //else {
         //    items = properOrdered.Take(processNewRanking.FirstOrDefault().Maximum).ToList();              
         //    items.ForEach(x => { x.FinalPDRank = 0; });
         //    finalContents.AddRange(items);
         //}
         //return (finalContents);
         throw new NotImplementedException();
      }

      private List<RetailPoolingInput> RankBasedOnLGD(List<RetailPoolingInput> contents, List<RetailBucketNumber> retailBucketNumbers) {
         //var contentsCount = contents.GroupBy(x => new { x.PROD }, (key, groupResult) => new {
         //    Segment = groupResult.FirstOrDefault().EC_SEG, ResultCount = groupResult.Count(), Prod = contents.FirstOrDefault().PROD
         //}).ToList();

         ////expect one Prod but if more take the first one.
         //if (contentsCount.Count >= 2) {
         //    var count = contentsCount.Sum(x => x.ResultCount);
         //    var segment = contentsCount.FirstOrDefault().Segment;
         //    var prod = contentsCount.FirstOrDefault().Prod;
         //    contentsCount.Clear();
         //    contentsCount.Add(new { Segment = segment, ResultCount = count, Prod = prod });
         //}

         //var filterRetailBackeList = new List<RetailBucketNumber>();
         //var lgd = 0.0M;

         //var poolingIntervals = new List<PoolingInterval>();
         //foreach (var item in contentsCount) {
         //    filterRetailBackeList = retailBucketNumbers.Where(x => string.Compare(x.Prod, item.Prod, true) == 0).ToList();
         //    foreach (var division in filterRetailBackeList) {
         //        var value = (decimal.Parse(division.LGD_buckets) * filterRetailBackeList.Count);
         //        lgd = Math.Ceiling(item.ResultCount / (decimal.Parse(division.LGD_buckets) * filterRetailBackeList.Count));
         //        var intervalContent = string.Empty;
         //        for (int i = 0; i <= value; i++) {
         //            if (i != 0)
         //                intervalContent += (lgd * i).ToString() + "|";
         //        }
         //        poolingIntervals.Add(new PoolingInterval() {
         //            Prod = item.Prod,
         //            Seg = item.Segment,
         //            Type = "LGD",
         //            Interval = intervalContent.RemoveLastString()
         //        });
         //    }
         //}

         ////LGD
         //var lgdPool = poolingIntervals.FirstOrDefault(x => string.Compare(x.Type, "LGD", true) == 0);
         //var intervals = lgdPool.Interval.Split('|').ToArray();
         //var processNewRanking = new List<ProcessNewRanking>();

         //for (int i = 0; i < intervals.Length; i++) {
         //    var newD = new ProcessNewRanking();
         //    newD.RowId = i;
         //    if (i == 0) {
         //        newD.Minimum = i + 1;
         //        newD.Maximum = int.Parse(intervals[i]);
         //    }
         //    else {
         //        newD.Minimum = int.Parse(intervals[i]) - int.Parse(intervals[0]);
         //        newD.Maximum = int.Parse(intervals[(i)]);
         //    }
         //    processNewRanking.Add(newD);
         //}

         //foreach (var c in contents) {
         //    var newRowIdRecord = processNewRanking.Where(x => c.RowId >= x.Minimum).Where(x => c.RowId <= x.Maximum).FirstOrDefault();
         //    c.NewLGDRank = newRowIdRecord.RowId;
         //}

         //var properOrdered = contents.OrderBy(x => x.NewPDRank).ToList();
         //var finalContents = new List<RetailPoolingInput>();
         //var items = new List<RetailPoolingInput>();
         //var secondItems = new List<RetailPoolingInput>();
         //var intervalMaximumBreakPoint = processNewRanking.FirstOrDefault().Maximum;

         //if (processNewRanking.Count == 1) {
         //    processNewRanking.Add(new ProcessNewRanking() { Maximum = -1, Minimum = -1, RowId = -1 });
         //}

         //if (processNewRanking.Count > 1) {
         //    for (int i = 0; i < processNewRanking.Count; i++) {
         //        var firstSetItemsNumber = processNewRanking[i];
         //        var secondSetItemsNumber = processNewRanking[i + 1];

         //        if (i == 0) {
         //            items = properOrdered.Take(firstSetItemsNumber.Maximum).ToList();
         //            secondItems = properOrdered.Skip(secondSetItemsNumber.Minimum).Take(secondSetItemsNumber.Minimum).ToList();
         //            items.ForEach(x => { x.FinalLGDRank = i; });
         //            finalContents.AddRange(items);

         //            secondItems.ForEach(x => { x.FinalLGDRank = (i + 1); });

         //            if (secondItems.Count > 0) {
         //                if (items.LastOrDefault().LGD == secondItems.FirstOrDefault().LGD) {
         //                    var matchingPDs = secondItems.Where(x => x.LGD == items.LastOrDefault().LGD).ToList();
         //                    matchingPDs.ForEach(x => { x.FinalLGDRank = i; });
         //                    finalContents.AddRange(matchingPDs);
         //                    var remaining = secondItems.Where(x => x.LGD != items.LastOrDefault().LGD).ToList();

         //                    if (remaining.Count() > 0)
         //                        finalContents.AddRange(remaining);
         //                }
         //                else {
         //                    finalContents.AddRange(secondItems);
         //                }
         //            }
         //        }
         //        else {
         //            items = finalContents;
         //            secondItems = properOrdered.Skip(secondSetItemsNumber.Minimum).Take(intervalMaximumBreakPoint).ToList();
         //            secondItems.ForEach(x => { x.FinalLGDRank = (i + 1); });

         //            if (secondItems.Count > 0) {
         //                if (items.LastOrDefault().LGD == secondItems.FirstOrDefault().LGD) {
         //                    var matchingPDs = secondItems.Where(x => x.LGD == items.LastOrDefault().LGD).ToList();
         //                    matchingPDs.ForEach(x => { x.FinalLGDRank = items.LastOrDefault().FinalLGDRank; });
         //                    finalContents.AddRange(matchingPDs);
         //                    var remaining = secondItems.Where(x => x.LGD != items.LastOrDefault().LGD).ToList();

         //                    if (remaining.Count() > 0)
         //                        finalContents.AddRange(remaining);
         //                }
         //                else {
         //                    finalContents.AddRange(secondItems);
         //                }
         //            }
         //        }

         //        if (finalContents.Count() >= properOrdered.Count())
         //            break;
         //    }
         //}
         //else {
         //    items = properOrdered.Take(processNewRanking.FirstOrDefault().Maximum).ToList();
         //    items.ForEach(x => { x.FinalPDRank = 0; });
         //    finalContents.AddRange(items);
         //}

         //return (finalContents);
         throw new NotImplementedException();
      }

      private List<RetailPoolingInput> RankBasedOnTM(List<RetailPoolingInput> contents, List<RetailBucketNumber> retailBucketNumbers) {
         //var contentsCount = contents.GroupBy(x => new { x.PROD }, (key, groupResult) => new {
         //    Segment = groupResult.FirstOrDefault().EC_SEG, ResultCount = groupResult.Count(), Prod = contents.FirstOrDefault().PROD
         //}).ToList();

         ////expect one Prod but if more take the first one.
         //if (contentsCount.Count >= 2) {
         //    var count = contentsCount.Sum(x => x.ResultCount);
         //    var segment = contentsCount.FirstOrDefault().Segment;
         //    var prod = contentsCount.FirstOrDefault().Prod;
         //    contentsCount.Clear();
         //    contentsCount.Add(new { Segment = segment, ResultCount = count, Prod = prod });
         //}

         //var filterRetailBackeList = new List<RetailBucketNumber>();
         //var tm = 0.0M;

         //var poolingIntervals = new List<PoolingInterval>();
         //foreach (var item in contentsCount) {
         //    filterRetailBackeList = retailBucketNumbers.Where(x => string.Compare(x.Prod, item.Prod, true) == 0).ToList();
         //    foreach (var division in filterRetailBackeList) {
         //        var value = (decimal.Parse(division.TM_buckets) * filterRetailBackeList.Count);
         //        tm = Math.Ceiling(item.ResultCount / (decimal.Parse(division.TM_buckets) * filterRetailBackeList.Count));
         //        var intervalContent = string.Empty;
         //        for (int i = 0; i <= value; i++) {
         //            if (i != 0)
         //                intervalContent += (tm * i).ToString() + "|";
         //        }
         //        poolingIntervals.Add(new PoolingInterval() {
         //            Prod = item.Prod,
         //            Seg = item.Segment,
         //            Type = "TM",
         //            Interval = intervalContent.RemoveLastString()
         //        });
         //    }
         //}

         ////TM
         //var lgdPool = poolingIntervals.FirstOrDefault(x => string.Compare(x.Type, "TM", true) == 0);
         //var intervals = lgdPool.Interval.Split('|').ToArray();
         //var processNewRanking = new List<ProcessNewRanking>();

         //for (int i = 0; i < intervals.Length; i++) {
         //    var newD = new ProcessNewRanking();
         //    newD.RowId = i;
         //    if (i == 0) {
         //        newD.Minimum = i + 1;
         //        newD.Maximum = int.Parse(intervals[i]);
         //    }
         //    else {
         //        newD.Minimum = int.Parse(intervals[i]) - int.Parse(intervals[0]);
         //        newD.Maximum = int.Parse(intervals[(i)]);
         //    }
         //    processNewRanking.Add(newD);
         //}

         //foreach (var c in contents) {
         //    var newRowIdRecord = processNewRanking.Where(x => c.RowId >= x.Minimum).Where(x => c.RowId <= x.Maximum).FirstOrDefault();
         //    c.NewTMRank = newRowIdRecord.RowId;
         //}

         //var properOrdered = contents.OrderBy(x => x.NewPDRank).ToList();
         //var finalContents = new List<RetailPoolingInput>();
         //var items = new List<RetailPoolingInput>();
         //var secondItems = new List<RetailPoolingInput>();
         //var intervalMaximumBreakPoint = processNewRanking.FirstOrDefault().Maximum;

         //if (processNewRanking.Count > 1) {
         //    for (int i = 0; i < processNewRanking.Count; i++) {
         //        var firstSetItemsNumber = processNewRanking[i];
         //        var secondSetItemsNumber = processNewRanking[i + 1];

         //        if (i == 0) {
         //            items = properOrdered.Take(firstSetItemsNumber.Maximum).ToList();
         //            secondItems = properOrdered.Skip(secondSetItemsNumber.Minimum).Take(secondSetItemsNumber.Minimum).ToList();
         //            items.ForEach(x => { x.FinalTMRank = i; });
         //            finalContents.AddRange(items);

         //            secondItems.ForEach(x => { x.FinalTMRank = (i + 1); });
         //            if (secondItems.Count > 0) {
         //                if (items.LastOrDefault().TimeToMaturity == secondItems.FirstOrDefault().TimeToMaturity) {
         //                    var matchingPDs = secondItems.Where(x => x.TimeToMaturity == items.LastOrDefault().TimeToMaturity).ToList();
         //                    matchingPDs.ForEach(x => { x.FinalTMRank = i; });
         //                    finalContents.AddRange(matchingPDs);
         //                    var remaining = secondItems.Where(x => x.TimeToMaturity != items.LastOrDefault().TimeToMaturity).ToList();

         //                    if (remaining.Count() > 0)
         //                        finalContents.AddRange(remaining);
         //                }
         //                else {
         //                    finalContents.AddRange(secondItems);
         //                }
         //            }
         //        }
         //        else {
         //            items = finalContents;
         //            secondItems = properOrdered.Skip(secondSetItemsNumber.Minimum).Take(intervalMaximumBreakPoint).ToList();
         //            secondItems.ForEach(x => { x.FinalTMRank = (i + 1); });

         //            if (secondItems.Count > 0) {
         //                if (items.LastOrDefault().TimeToMaturity == secondItems.FirstOrDefault().TimeToMaturity) {
         //                    var matchingPDs = secondItems.Where(x => x.TimeToMaturity == items.LastOrDefault().TimeToMaturity).ToList();
         //                    matchingPDs.ForEach(x => { x.FinalTMRank = items.LastOrDefault().FinalTMRank; });
         //                    finalContents.AddRange(matchingPDs);
         //                    var remaining = secondItems.Where(x => x.TimeToMaturity != items.LastOrDefault().TimeToMaturity).ToList();

         //                    if (remaining.Count() > 0)
         //                        finalContents.AddRange(remaining);
         //                }
         //                else {
         //                    finalContents.AddRange(secondItems);
         //                }
         //            }
         //        }

         //        if (finalContents.Count() >= properOrdered.Count())
         //            break;
         //    }
         //}
         //else {
         //    items = properOrdered.Take(processNewRanking.FirstOrDefault().Maximum).ToList();
         //    items.ForEach(x => { x.FinalPDRank = 0; });
         //    finalContents.AddRange(items);
         //}

         //return (finalContents);
         throw new NotImplementedException();
      }

      public List<RetailPoolingInput> ProcessRetailPooling(List<RetailPoolingInput> contents, List<RetailBucketNumber> retailBucketNumbers) {
         //var row = 1;
         //var p1RankedPd = contents.OrderBy(x => x.EC_CLUSTER).OrderBy(x => x.EC_SEG).Select(x => x.CumulativePDs).Distinct().OrderBy(x => x).Select(x => new { RowId = row++, RankedPD = x }).ToList();

         //contents.OrderBy(x => x.EC_CLUSTER).OrderBy(x => x.EC_SEG).ToList().OrderBy(x => x.CumulativePDs).ToList().ForEach(item => {
         //    var filterRank = p1RankedPd.SingleOrDefault(x => x.RankedPD == item.CumulativePDs);
         //    item.RankByPD = filterRank.RowId;
         //});

         //var result = contents.OrderByDescending(x => x.RankByPD).ToList();
         //var resultPDRankings = result.OrderBy(x => x.RankByPD).ToList();


         //row = 1;
         //var p1RankedByLGD = contents.OrderBy(x => x.EC_CLUSTER).OrderBy(x => x.EC_SEG).OrderBy(x => x.RankByPD).Select(x => x.LGD).Distinct().OrderBy(x => x).
         //                    Select(x => new { RowId = row++, RankLGD = x }).ToList();

         //contents.OrderBy(x => x.EC_CLUSTER).OrderBy(x => x.EC_SEG).ToList().OrderBy(x => x.LGD).ToList().ForEach(item => {
         //    var filterRank = p1RankedByLGD.SingleOrDefault(x => x.RankLGD == item.LGD);
         //    item.RankkByLGD = filterRank.RowId;
         //});

         //var resultLGD = contents.OrderByDescending(x => x.RankkByLGD).ToList();
         //var resultLGDRankings = result.OrderBy(x => x.RankkByLGD).ToList();

         //row = 1;
         //var p1RankedByTM = contents.OrderBy(x => x.EC_CLUSTER).OrderBy(x => x.EC_SEG).OrderBy(x => x.RankkByLGD).Select(x => x.TimeToMaturity).Distinct().OrderBy(x => x).
         //                    Select(x => new { RowId = row++, RankTM = x }).ToList();

         //contents.OrderBy(x => x.EC_CLUSTER).OrderBy(x => x.EC_SEG).ToList().OrderBy(x => x.LGD).ToList().ForEach(item => {
         //    var filterRank = p1RankedByTM.SingleOrDefault(x => x.RankTM == item.TimeToMaturity);
         //    item.RankByTM = filterRank.RowId;
         //});

         //var resultTM = contents.OrderByDescending(x => x.RankByTM).ToList();
         //var resultTMRankings = result.OrderBy(x => x.RankByTM).ToList();


         //row = 1;
         //var OrderByRankedPD = resultTMRankings.OrderBy(x => x.CumulativePDs).ToList();
         //OrderByRankedPD.ForEach(item => { item.RowId = (row++); });



         //var contentsCount = OrderByRankedPD.GroupBy(x => new { x.EC_SEG }, (key, groupResult) => new { Segment = key.EC_SEG, ResultCount = groupResult.Count(), Prod = resultTMRankings.FirstOrDefault().PROD }).ToList();

         //var filterRetailBackeList = new List<RetailBucketNumber>();

         //var pd = 0.0M;
         //var lgd = 0.0M;
         //var tm = 0.0M;

         //var poolingIntervals = new List<PoolingInterval>();
         //foreach (var item in contentsCount) {
         //    filterRetailBackeList = retailBucketNumbers.Where(x => (string.Compare(x.Seg, item.Segment, true) == 0) && (string.Compare(x.Prod, item.Prod, true) == 0)).ToList();
         //    foreach (var division in filterRetailBackeList) {
         //        pd = Math.Ceiling(item.ResultCount / decimal.Parse(division.PD_buckets));
         //        var intervalContent = string.Empty;
         //        for (int i = 0; i <= int.Parse(division.PD_buckets); i++) {
         //            if (i != 0)
         //                intervalContent += (pd * i).ToString() + "|";
         //        }
         //        poolingIntervals.Add(new PoolingInterval() {
         //            Prod = item.Prod,
         //            Seg = item.Segment,
         //            Type = "PD",
         //            Interval = intervalContent.RemoveLastString()
         //        });

         //        intervalContent = string.Empty;
         //        lgd = Math.Ceiling(item.ResultCount / decimal.Parse(division.LGD_buckets));
         //        for (int i = 0; i <= int.Parse(division.LGD_buckets); i++) {
         //            if (i != 0)
         //            intervalContent += (lgd * i).ToString() + "|";
         //        }
         //        poolingIntervals.Add(new PoolingInterval() {
         //            Prod = item.Prod,
         //            Seg = item.Segment,
         //            Type = "LGD",
         //            Interval = intervalContent.RemoveLastString()
         //        });

         //        intervalContent = string.Empty;
         //        tm = Math.Ceiling(item.ResultCount / decimal.Parse(division.TM_buckets));
         //        for (int i = 0; i <= int.Parse(division.TM_buckets); i++) {
         //            if (i != 0)
         //                intervalContent += (tm * i).ToString() + "|";
         //        }
         //        poolingIntervals.Add(new PoolingInterval() {
         //            Prod = item.Prod,
         //            Seg = item.Segment,
         //            Type = "TM",
         //            Interval = intervalContent.RemoveLastString()
         //        });
         //    }
         //}


         ////PD
         //var lgdPool = poolingIntervals.SingleOrDefault(x => string.Compare(x.Type, "PD", true) == 0);
         //var intervals = lgdPool.Interval.Split('|').ToArray();
         //var processNewRanking = new List<ProcessNewRanking>();

         //for (int i = 0; i < intervals.Length; i++) {
         //    var newD = new ProcessNewRanking();
         //    newD.RowId = i;
         //    if (i == 0) {
         //        newD.Minimum = i + 1;
         //        newD.Maximum = int.Parse(intervals[i]);
         //    }
         //    else {
         //        newD.Minimum = int.Parse(intervals[i]) - int.Parse(intervals[0]);
         //        newD.Maximum = int.Parse(intervals[(i)]);
         //    }
         //    processNewRanking.Add(newD);
         //}

         //resultTMRankings.ForEach(c => {
         //    var newRowIdRecord = processNewRanking.Where(x => c.RowId >= x.Minimum).Where(x => c.RowId <= x.Maximum).SingleOrDefault();
         //    c.NewPDRank = newRowIdRecord.RowId;
         //});

         //var properOrdered = resultTMRankings.OrderBy(x => x.NewPDRank).ToList();

         //var finalContents = new List<RetailPoolingInput>();

         //var intervalMax = processNewRanking.FirstOrDefault().Maximum;
         //for (int i = 0; i < processNewRanking.Count; i++) {               
         //    var dataSetOne = properOrdered.Skip(finalContents.Count).Take(intervalMax).ToList();
         //    var dataSetTwo = properOrdered.Skip((finalContents.Count + intervalMax)).Take(intervalMax).ToList();

         //    if (dataSetTwo.Count > 0) {
         //        if (int.Equals(dataSetOne.LastOrDefault().RankByPD, dataSetTwo.FirstOrDefault().RankByPD)) {
         //            var newSet = (dataSetOne).Concat(dataSetTwo.Where(x => x.RankByPD == dataSetTwo.FirstOrDefault().RankByPD).ToList()).ToList();
         //            newSet.ForEach(x => {
         //                x.FinalPDRank = dataSetOne.FirstOrDefault().NewPDRank;
         //            });
         //            finalContents.AddRange(newSet);
         //        }
         //    }
         //    else {
         //        if (dataSetOne.Count > 0) {
         //            dataSetOne.ForEach(x => { x.FinalPDRank = x.NewPDRank; });
         //            finalContents.AddRange(dataSetOne);
         //        }
         //    }

         //    if (dataSetTwo.Count == 0)
         //        break;
         //}

         //var exportContent = finalContents.OrderBy(x => x.FinalPDRank).ToList();
         //return (exportContent);
         throw new NotImplementedException();
      }

      public string ReadRetailPoolingFiles(RetailPoolingFileType productType, string filePath, string contentDelimeter, DbContextType dbContextType) {
         var inputFiles = new List<ABSARetailPoolingInput>();

         using(StreamReader reader = new StreamReader(filePath)) {
            string line = reader.ReadLine();
            while((line = reader.ReadLine()) != null) {
               string[] content = line.ConvertDelimetedStringToArray(contentDelimeter);
               var retailInput = new ABSARetailPoolingInput() {
                  ObligorId = content[0].Trim(),
                  FacilityId = content[1].Trim(),
                  RSq = double.Parse(content[2].Trim()),
                  QSq = double.Parse(content[3].Trim()),
                  AssetReturnWeights = content[4].Trim(),
                  RecoveryWeights = content[5].Trim(),
                  Tenors = int.Parse(content[6].Trim()),
                  TimeToMaturity = int.Parse(content[7].Trim()),
                  CumulativePDs = !string.IsNullOrEmpty(content[8].Trim()) ? double.Parse(content[8].Trim()) : 0,
                  LGD = !string.IsNullOrEmpty(content[9].Trim()) ? double.Parse(content[9].Trim()) : 0,
                  EAD = !string.IsNullOrEmpty(content[10].Trim()) ? double.Parse(content[10].Trim()) : 0,
                  SUBMISSION_UNIT_CD = content[11].Trim(),
                  EC_SEG = content[12].Trim(),
                  PROD = content[13].Trim(),
                  EC_CLUSTER = content[14].Trim(),
                  UNIQUE_KEY = content[15].Trim(),
                  FinalPDRank = 0,
                  FinalLGDRank = 0,
                  FinalTMRank = 0,
                  CreatedDate = DateTime.Now,
                  InputType = productType.ToString()
               };

               inputFiles.Add(retailInput);

               if(inputFiles.Count == 30000) {
                  var bulkInsert = BulkInsert(inputFiles, dbContextType);
                  inputFiles.Clear();
               }
            }
            if(inputFiles.Count > 0) {
               var bulkInsert = BulkInsert(inputFiles, dbContextType);
               inputFiles.Clear();
            }
         }
         return productType.ToString();
      }

      public List<RetailPoolingInput> GetRetailPoolingInput(RetailPoolingFileType retailPoolingFile, string fileContentDelimiter) {
         var fileName = string.Format("_{0}_", retailPoolingFile.ToString());
         var baseDirectoryLastFolder = fileManager.GetLastFolder(BASEDIRECTORY);
         var filePath = fileManager.SearchFiles(baseDirectoryLastFolder).FirstOrDefault(x => x.ToLower().Contains(fileName.ToLower()));
         var contents = fileManager.ReadCSVFile(filePath);
         var P1ItemsDueForProcesing = new RetailPoolingInput().ConvertToModel(contents, 0, fileContentDelimiter);
         return P1ItemsDueForProcesing;
      }

      public List<RetailBucketNumber> GetRetailBucketNumbers(string fileContentDelimiter, string filterFileName) {
         var fileName = string.Format("{0}", filterFileName);
         var baseDirectoryLastFolder = fileManager.GetLastFolder(BASEDIRECTORY);
         var filePath = fileManager.SearchFiles(baseDirectoryLastFolder).FirstOrDefault(x => x.ToLower().Contains(fileName.ToLower()));
         var contents = fileManager.ReadCSVFile(filePath);
         var P1ItemsDueForProcesing = new RetailBucketNumber().ConvertToModel(contents, 0, fileContentDelimiter);
         return P1ItemsDueForProcesing;
      }
   }
}
