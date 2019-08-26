using EconomicCapital.RF.DataAccess;
using EconomicCapital.RF.DTO;
using Ninject;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.BusinessLogic {
   public class RetailPoolingManager : IRetailPoolingManager {
      private INinjectKernel _ninjectKernelBindings;
      private readonly ISimPakDataProcessing _iDbContextAccess;
      private readonly IFileManager fileManager;

      public RetailPoolingManager(INinjectKernel ninjectKernel) {
         _ninjectKernelBindings = ninjectKernel;
         IKernel kernel = _ninjectKernelBindings.GetNinjectBindings();
         _iDbContextAccess = kernel.Get<ISimPakDataProcessing>();
         fileManager = kernel.Get<IFileManager>();
      }

      #region RETAIL_POOLING_LOGIC
      public Guid ReadRetailPoolingFiles(RetailInputFileParam retailInputFileParam) {
         var inputFiles = new List<ABSARetailPoolingInput>();
         var cleanTable = _iDbContextAccess.TruncateRetailPoolingInputTable(retailInputFileParam.DbContextType);

         var rowId = CreateRetailPoolingProgress(retailInputFileParam.DbContextType, retailInputFileParam.ProductName);
         var updateProgressStart = _iDbContextAccess.CreateRetailProcessingProgressDetails(retailInputFileParam.DbContextType, rowId, "Reading Input File", ProgressStatus.Started);

         using(StreamReader reader = new StreamReader(retailInputFileParam.FilePath)) {
            string line = reader.ReadLine();
            while((line = reader.ReadLine()) != null) {
               retailInputFileParam.ContentLine = line;
               var retailInput = FormatABSARetailInputLine(retailInputFileParam, rowId);
               inputFiles.Add(retailInput);
               
               if(inputFiles.Count == 100000) {
                  var bulkInsert = BulkInsert(inputFiles, retailInputFileParam.DbContextType);
                  inputFiles.Clear();
               }
            }

            if(inputFiles.Count > 0) {
               var bulkInsert = BulkInsert(inputFiles, retailInputFileParam.DbContextType);
               inputFiles.Clear();
            }
         }
         var updateProgressEnd = _iDbContextAccess.CreateRetailProcessingProgressDetails(retailInputFileParam.DbContextType, rowId, "Reading Input File", ProgressStatus.Completed);
         return rowId;
      }

      private Guid CreateRetailPoolingProgress(DbContextType dbContextType, RetailPoolingFileType productName) {
         return _iDbContextAccess.CreateRetailProcessingProgress(dbContextType, new ABSARetailProcessingProgress() {
            ProductName = productName.ToString(),
            DateCreated = DateTime.Now
         });
      }

      private string BulkInsert(List<ABSARetailPoolingInput> ABSARetailPoolingInput, DbContextType dbContextType) {
         var result = string.Empty;
         switch(dbContextType) {
            case DbContextType.RFSTAGINGCONTEXT:
               result = _iDbContextAccess.BulkInsert(ABSARetailPoolingInput, _iDbContextAccess.GetRFStagingDbContext());
               break;
            case DbContextType.SOURCEDATACONTEXT:
               result = _iDbContextAccess.BulkInsert(ABSARetailPoolingInput, _iDbContextAccess.GetApplicationDbContext());
               break;
            default:
               break;
         }
         return result;
      }

      private ABSARetailPoolingInput FormatABSARetailInputLine(RetailInputFileParam retailInputFileParam, Guid rowId) {
         string[] content = retailInputFileParam.ContentLine.ConvertDelimetedStringToArray(retailInputFileParam.ContentDelimeter);
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
            InputType = retailInputFileParam.ProductName.ToString(),
            RowId = rowId,
            PoolingLGD = !string.IsNullOrEmpty(content[16].Trim()) ? double.Parse(content[16].Trim()) : 0,
            OrginalPoolingLGD = !string.IsNullOrEmpty(content[16].Trim()) ? double.Parse(content[16].Trim()) : 0
         };
         retailInput.PoolingLGD = Math.Round(retailInput.PoolingLGD, 2);
         return (retailInput);
      }

      public bool ProcessRetailPoolingRanks(DbContextType dbContextType) {
         var retailPoolingConfigs = _iDbContextAccess.GetRetailPoolingConfigs(dbContextType);
         var result = false;
         var inputFilesbaseDirectory = retailPoolingConfigs.FirstOrDefault(x => string.Compare(x.Type, ABSARetailPoolingEnumConfigType.SharedBaseDirectory.ToString(), true) == 0).Value;
         var fileContentDelimeter = retailPoolingConfigs.FirstOrDefault(x => string.Compare(x.Type, ABSARetailPoolingEnumConfigType.InputFileContentDelimeter.ToString(), true) == 0).Value;
         var expectedTotalFile = retailPoolingConfigs.FirstOrDefault(x => string.Compare(x.Type, ABSARetailPoolingEnumConfigType.TotalRequiredFiles.ToString(), true) == 0).Value;
         var outputFilesBaseDirectory = retailPoolingConfigs.FirstOrDefault(x => string.Compare(x.Type, ABSARetailPoolingEnumConfigType.OutputBaseDirectory.ToString(), true) == 0).Value;
         var productNames = Enum.GetValues(typeof(RetailPoolingFileType)).Cast<RetailPoolingFileType>().Select(v => v.ToString()).ToList(); //.Where(x => string.Compare(x,"VA",true) != 0).ToList();

         var chekFiles = fileManager.GetProcessingDirectoryFiles(inputFilesbaseDirectory);

         if(int.Parse(expectedTotalFile) == chekFiles.Count) {
            var outputFolderName = string.Format("Output_{0}_{1}", DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"), DateTime.Now.Ticks.ToString("x"));

            foreach(var productName in productNames) {
               var fileName = string.Format("_{0}_", productName);
               var baseDirectoryLastFolder = fileManager.GetLastFolder(inputFilesbaseDirectory);
               var filePath = fileManager.SearchFiles(baseDirectoryLastFolder).FirstOrDefault(x => x.ToLower().Contains(fileName.ToLower()));
               var productType = (RetailPoolingFileType)Enum.Parse(typeof(RetailPoolingFileType), productName);
               var inputParam = new RetailInputFileParam() {
                  ContentDelimeter = fileContentDelimeter,
                  ProductName = productType,
                  DbContextType = dbContextType,
                  ContentLine = fileContentDelimeter,
                  FilePath = filePath
               };
               var rowId = ReadRetailPoolingFiles(inputParam);

               var pd = PDRank(productType, dbContextType, rowId);
               var lgd = LGDRank(productType, dbContextType, rowId);
               var tm = TMRank(productType, dbContextType, rowId);

               var fileContent = _iDbContextAccess.GetFileContents(productType, dbContextType, rowId);
               var outputFileName = string.Format("{0}_RANKING_PROCESSING_{1}_{2}", productName.ToUpper(), DateTime.Now.ToString("yyyyMMddmmss"), DateTime.Now.Ticks.ToString("x"));
               var writeOutput = fileManager.CreatePoolingOutputFiles($"{outputFileName}_Input", outputFilesBaseDirectory, fileContent, outputFolderName);               

               var pdContents = _iDbContextAccess.GetFileContentsPD(dbContextType, rowId);
               result = fileManager.CreatePoolingOutputFilePD($"{outputFileName}_PDLOGIC", outputFilesBaseDirectory, pdContents, outputFolderName);
               var lgdContents = _iDbContextAccess.GetFileContentsLGD(dbContextType, rowId);
               result = fileManager.CreatePoolingOutputFileLGD($"{outputFileName}_LGDLOGIC", outputFilesBaseDirectory, lgdContents, outputFolderName);
               var tmContents = _iDbContextAccess.GetFileContentsTM(dbContextType, rowId);
               result = fileManager.CreatePoolingOutputFileTM($"{outputFileName}_TMLOGIC", outputFilesBaseDirectory, tmContents, outputFolderName);
               var processingDone = _iDbContextAccess.UpdateRetailProcessingProgress(dbContextType, rowId);

               var updateProgressStart = _iDbContextAccess.CreateRetailProcessingProgressDetails(dbContextType, rowId, "Clearing Temp Data", ProgressStatus.Started);
               var cleanTable = _iDbContextAccess.TruncateRetailPoolingInputTable(dbContextType);
               var updateProgressEnd = _iDbContextAccess.CreateRetailProcessingProgressDetails(dbContextType, rowId, "Clearing Temp Data", ProgressStatus.Completed);
               fileContent.Clear();
            }
            result = true;
         }
         else {
            result = false;
         }
         return (result);
      }

      public bool ProcessSingleProductRetailPoolingRank(RetailPoolingFileType productType, DbContextType dbContextType) {
         var retailPoolingConfigs = _iDbContextAccess.GetRetailPoolingConfigs(dbContextType);
         var result = false;
         var inputFilesbaseDirectory = retailPoolingConfigs.FirstOrDefault(x => string.Compare(x.Type, ABSARetailPoolingEnumConfigType.SharedBaseDirectory.ToString(), true) == 0).Value;
         var fileContentDelimeter = retailPoolingConfigs.FirstOrDefault(x => string.Compare(x.Type, ABSARetailPoolingEnumConfigType.InputFileContentDelimeter.ToString(), true) == 0).Value;
         //var expectedTotalFile = retailPoolingConfigs.FirstOrDefault(x => string.Compare(x.Type, ABSARetailPoolingEnumConfigType.TotalRequiredFiles.ToString(), true) == 0).Value;
         var outputFilesBaseDirectory = retailPoolingConfigs.FirstOrDefault(x => string.Compare(x.Type, ABSARetailPoolingEnumConfigType.OutputBaseDirectory.ToString(), true) == 0).Value;
                 
         var outputFolderName = string.Format("Output_{0}_{1}", DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"), DateTime.Now.Ticks.ToString("x"));
         var fileName = string.Format("_{0}_", productType.ToString());
         var baseDirectoryLastFolder = fileManager.GetLastFolder(inputFilesbaseDirectory);
         var filePath = fileManager.SearchFiles(baseDirectoryLastFolder).FirstOrDefault(x => x.ToLower().Contains(fileName.ToLower()));

         if(!string.IsNullOrEmpty(filePath)) {
            var inputParam = new RetailInputFileParam() {
               ContentDelimeter = fileContentDelimeter,
               ProductName = productType,
               DbContextType = dbContextType,
               ContentLine = fileContentDelimeter,
               FilePath = filePath
            };
            var rowId = ReadRetailPoolingFiles(inputParam);

            var pd = PDRank(productType, dbContextType, rowId);
            var lgd = LGDRank(productType, dbContextType, rowId);
            var tm = TMRank(productType, dbContextType, rowId);

            var fileContent = _iDbContextAccess.GetFileContents(productType, dbContextType, rowId);
            var outputFileName = string.Format("{0}_RANKING_PROCESSING_{1}_{2}", productType.ToString().ToUpper(), DateTime.Now.ToString("yyyyMMddmmss"), DateTime.Now.Ticks.ToString("x"));
            result = fileManager.CreatePoolingOutputFiles($"{outputFileName}_Input", outputFilesBaseDirectory, fileContent, outputFolderName);

            var pdContents = _iDbContextAccess.GetFileContentsPD(dbContextType, rowId);
            result = fileManager.CreatePoolingOutputFilePD($"{outputFileName}_PDLOGIC", outputFilesBaseDirectory, pdContents, outputFolderName);
            var lgdContents = _iDbContextAccess.GetFileContentsLGD(dbContextType, rowId);
            result = fileManager.CreatePoolingOutputFileLGD($"{outputFileName}_LGDLOGIC", outputFilesBaseDirectory, lgdContents, outputFolderName);
            var tmContents = _iDbContextAccess.GetFileContentsTM(dbContextType, rowId);
            result = fileManager.CreatePoolingOutputFileTM($"{outputFileName}_TMLOGIC", outputFilesBaseDirectory, tmContents, outputFolderName);
            var processingDone = _iDbContextAccess.UpdateRetailProcessingProgress(dbContextType, rowId);

            var updateProgressStart = _iDbContextAccess.CreateRetailProcessingProgressDetails(dbContextType, rowId, "Clearing Temp Data", ProgressStatus.Started);
            var cleanTable = _iDbContextAccess.TruncateRetailPoolingInputTable(dbContextType);
            var updateProgressEnd = _iDbContextAccess.CreateRetailProcessingProgressDetails(dbContextType, rowId, "Clearing Temp Data", ProgressStatus.Completed);
            fileContent.Clear();
         }
         return (result);
      }

      public ABSAPoolingLookup GetLookupData(RetailPoolingFileType productName, int totalcount, DbContextType dbContextType) {
         return _iDbContextAccess.GetLookupData(productName, totalcount, dbContextType);
      }

      public bool InitialClusterPDLogic(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId) {
         return _iDbContextAccess.InitialClusterPDLogic(productName, dbContextType, rowId);
      }

      public List<ABSARetailPoolingClusterSegment> GetClusterSegments(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId) {
         return _iDbContextAccess.GetClusterSegments(productName, dbContextType,rowId);
      }

      public List<ABSARetailPoolingPDSegment> GetSegmentContents(DbContextType dbContextType, int clusterRowId, int segmentRowId, string segment, RetailPoolingFileType productName,Guid rowId) {
         var param = new ABSARetailPoolingPDSegmentParam() {
            DbContextType = dbContextType, ClusterRowId = clusterRowId, Segment = segment, SegmentRowId = segmentRowId, RetailPoolingFileType = productName, RowId = rowId
         };

         return _iDbContextAccess.GetSegmentContents(param);
      }

      public bool PDRank(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId) {
         var updateProgressStart = _iDbContextAccess.CreateRetailProcessingProgressDetails(dbContextType, rowId, "PD Processing", ProgressStatus.Started);
         var initialClusterPDLogic = InitialClusterPDLogic(productName, dbContextType, rowId);
         var generatePDRatioInitialRank = GeneratePDRatioInitialRank(productName, dbContextType, rowId);
         var genetatePDFinalPool = GeneratePDFinalPool(productName, dbContextType, rowId);
         var updateInputFileFinalPDRank = _iDbContextAccess.UpdateInputFileFinalPDRank(productName,dbContextType, rowId);
         var updateProgressEnd = _iDbContextAccess.CreateRetailProcessingProgressDetails(dbContextType, rowId, "PD Processing", ProgressStatus.Completed);
         return updateInputFileFinalPDRank;
      }

      private bool GeneratePDFinalPool(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId) {
         var clusterSegments = GetClusterSegments(productName, dbContextType, rowId);

         foreach(var segment in clusterSegments) {
            var segmentContents = GetSegmentContents(dbContextType, segment.ClusterRowId, segment.SegmentRowId, segment.Segment, productName, rowId);
            var numberOfTransactionsInFile = segmentContents.Sum(x => x.CountOfPd);
            var pdLgdTonorLookup = GetLookupData(productName, numberOfTransactionsInFile, dbContextType);
            var numberOfPDPool = pdLgdTonorLookup.PD;
            var constTolerance = pdLgdTonorLookup.PDTolerance;
            var avgNoOfTransactionInPool = ((double)numberOfTransactionsInFile / (double)numberOfPDPool).NearestTenthWithRoundupAlways();

            foreach(var item in segmentContents) {
               if(item.Count == item.CumulativeSum) {
                  item.FinalPool = 0;
               }
               else {
                  var previousItem = segmentContents.FirstOrDefault(x => x.Id == (item.Id - 1));
                  if(previousItem.InitialRank == item.InitialRank) {
                     item.FinalPool = previousItem.InitialRank;
                  }
                  else {
                     if(previousItem.InitialRank != previousItem.FinalPool) {
                        item.FinalPool = item.InitialRank;
                     }
                     else {
                        var tempContents = segmentContents.Where(x => x.Id >= 1).Where(x => x.Id <= (item.Id - 1)).ToList();
                        var sumValue = tempContents.Where(x => x.FinalPool == previousItem.FinalPool).Sum(x => x.Count);
                        var sliceSize = avgNoOfTransactionInPool * constTolerance;
                        if(sumValue < sliceSize) {
                           item.FinalPool = previousItem.FinalPool;
                        }
                        else {
                           item.FinalPool = previousItem.FinalPool + 1;
                        }
                        tempContents.Clear();
                     }
                  }
               }
            }
            //DB update and Clear
            var update = _iDbContextAccess.UpdateDRatioInitialRankFinalPool(segmentContents, dbContextType, rowId);
            segmentContents.Clear();
         }
         clusterSegments.Clear();
         return true;
      }

      private bool GeneratePDRatioInitialRank(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId) {
         var clusterSegments = GetClusterSegments(productName, dbContextType, rowId);

         foreach(var segment in clusterSegments) {
            var segmentContents = GetSegmentContents(dbContextType, segment.ClusterRowId, segment.SegmentRowId, segment.Segment, productName, rowId);
            var numberOfTransactionsInFile = segmentContents.Sum(x => x.CountOfPd);
            var pdLgdTonorLookup = GetLookupData(productName, numberOfTransactionsInFile, dbContextType);
            var numberOfPDPool = pdLgdTonorLookup.PD;

            var avgNoOfTransactionInPool = ((double)numberOfTransactionsInFile /(double) numberOfPDPool).NearestTenthWithRoundupAlways();
            var originalAvgTransactionsInPool = ((double)numberOfTransactionsInFile / (double)numberOfPDPool);

            foreach(var PDSegment in segmentContents) {
               var ratio = (double)PDSegment.CumulativeSum / avgNoOfTransactionInPool;
               PDSegment.Ratio = ratio;
               PDSegment.InitialRank = Math.Min((numberOfPDPool - 1), (int)ratio);
               PDSegment.AveragePerPool = avgNoOfTransactionInPool;
               PDSegment.OriginalAveragePerPool = originalAvgTransactionsInPool;
            }
            //DB update and Clear
            var update = _iDbContextAccess.UpdateDRatioInitialRankFinalPool(segmentContents, dbContextType, rowId);
            segmentContents.Clear();
         }
         clusterSegments.Clear();
         return true;
      }

      public bool LGDRank(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId) {
         var updateProgressStart = _iDbContextAccess.CreateRetailProcessingProgressDetails(dbContextType, rowId, "LGD Processing", ProgressStatus.Started);
         var initialLDGLogic = _iDbContextAccess.InitialLDGLogic(productName, dbContextType,rowId);
         var generatePDRatioInitialRank = GenerateLGDRatioInitialRank(productName, dbContextType, rowId);
         var genetatePDFinalPool = GenerateLGDFinalPool(productName, dbContextType, rowId);
         var updateInputFileFinalPDRank = _iDbContextAccess.UpdateInputFileFinalLGDRank(productName,dbContextType, rowId);
         var updateProgressEnd = _iDbContextAccess.CreateRetailProcessingProgressDetails(dbContextType, rowId, "LGD Processing", ProgressStatus.Completed);
         return updateInputFileFinalPDRank;
      }

      private bool GenerateLGDRatioInitialRank(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId) {
         var lgdPerPdGroupings = _iDbContextAccess.GetLGDPERPDGroup(productName, dbContextType, rowId);

         foreach(var grouping in lgdPerPdGroupings) {
            var param = new ABSARetailPoolingLGDSegmentParam() {
               DbContextType = dbContextType, PdRowLebelIs = grouping.PDRowLabelId, UniqueSegment = grouping.UniqueSegment, SegmentRowId = grouping.SegmentRowId,
               ClusterId = grouping.ClusterRowId, RetailPoolingFileType = productName, RowId = rowId
            };

            var lgdPerPDGroupData = _iDbContextAccess.GetLGDPerPDGroupData(param);
            var numberOfTransactionsInFile = _iDbContextAccess.GetSegmentLGDCount(grouping.UniqueSegment, dbContextType);
            var pdLgdTonorLookup = GetLookupData(productName, numberOfTransactionsInFile, dbContextType);
            var numberOfLGPool = pdLgdTonorLookup.LGD;
            var total = lgdPerPDGroupData.Sum(x => x.LGDCount);

            var avgPerPool = ((double)total / (double)numberOfLGPool).NearestTenthWithRoundupAlways();
            var originalAvgTransactionsInPool = ((double)total / (double)numberOfLGPool);

            foreach(var item in lgdPerPDGroupData) {
               var ratio = (double)item.CumulativeSum / avgPerPool;
               item.Ratio = ratio;
               item.InitialRank = Math.Min((numberOfLGPool - 1), (int)ratio);
               item.AveragePerPool = avgPerPool;
               item.OriginalAveragePerPool = originalAvgTransactionsInPool;
            }
            //DB update and Clear
            var update = _iDbContextAccess.UpdateLGDRatioInitialRankFinalPool(lgdPerPDGroupData, dbContextType, rowId);
            lgdPerPDGroupData.Clear();
         }
         lgdPerPdGroupings.Clear();
         return true;
      }

      private bool GenerateLGDFinalPool(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId) {
         var lgdPerPdGroupings = _iDbContextAccess.GetLGDPERPDGroup(productName, dbContextType, rowId);

         foreach(var grouping in lgdPerPdGroupings) {
            var param = new ABSARetailPoolingLGDSegmentParam() {
               DbContextType = dbContextType, PdRowLebelIs = grouping.PDRowLabelId, UniqueSegment = grouping.UniqueSegment, SegmentRowId = grouping.SegmentRowId,
               ClusterId = grouping.ClusterRowId, RetailPoolingFileType = productName, RowId = rowId
            };

            var lgdPerPDGroupData = _iDbContextAccess.GetLGDPerPDGroupData(param);
            var numberOfTransactionsInFile = _iDbContextAccess.GetSegmentLGDCount(grouping.UniqueSegment, dbContextType);
            var pdLgdTonorLookup = GetLookupData(productName, numberOfTransactionsInFile, dbContextType);
            var numberOfLGPool = pdLgdTonorLookup.LGD;
            var toleranceLevel = pdLgdTonorLookup.LGDTolerance;

            var total = lgdPerPDGroupData.Sum(x => x.LGDCount);

            var avgPerPool = ((double)total / (double)numberOfLGPool).NearestTenthWithRoundupAlways();

            foreach(var item in lgdPerPDGroupData) {
               if(item.Count == item.CumulativeSum) {
                  item.FinalPool = 0;
               }
               else {
                  var previousItem = lgdPerPDGroupData.FirstOrDefault(x => x.Id == (item.Id - 1));
                  if(item.UniqueLGDCountPerPDPool <= numberOfLGPool) {
                     item.FinalPool = previousItem.FinalPool + 1;
                  }
                  else {
                     if(previousItem.FinalPool == item.InitialRank) {
                        item.FinalPool = previousItem.InitialRank;
                     }
                     else {
                        var tempContents = lgdPerPDGroupData.Where(x => x.Id >= 1).Where(x => x.Id <= (item.Id - 1)).ToList();
                        var sumValue = tempContents.Where(x => x.FinalPool == previousItem.FinalPool).Sum(x => x.Count);
                        var sliceSize = item.AveragePerPool * toleranceLevel;
                        if(sumValue < sliceSize) {
                           item.FinalPool = previousItem.FinalPool;
                        }
                        else {
                           item.FinalPool = previousItem.FinalPool + 1;
                        }
                     }
                  }
               }
            }
            //DB update and Clear
            var update = _iDbContextAccess.UpdateLGDRatioInitialRankFinalPool(lgdPerPDGroupData, dbContextType, rowId);
            lgdPerPDGroupData.Clear();
         }
         lgdPerPdGroupings.Clear();
         return true;
      }

      //PD

      public bool TMRank(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId) {
         var updateProgressStart = _iDbContextAccess.CreateRetailProcessingProgressDetails(dbContextType, rowId, "TM Processing", ProgressStatus.Started);
         var initialTMLogic = _iDbContextAccess.InitialTMLogic(productName, dbContextType,rowId);
         var generateTMRatioInitialRank = GenerateTMRatioInitialRank(productName, dbContextType, rowId);
         var generateTMFinalPool = GenerateTMFinalPool(productName, dbContextType, rowId);
         var updateInputFileFinalTMRank = _iDbContextAccess.UpdateInputFileFinalTMRank(productName, dbContextType, rowId);
         var updateProgressEnd = _iDbContextAccess.CreateRetailProcessingProgressDetails(dbContextType, rowId, "TM Processing", ProgressStatus.Completed);
         return (updateInputFileFinalTMRank);
      }

      private bool GenerateTMRatioInitialRank(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId) {
         var tmPerPdLgdGroupings = _iDbContextAccess.GetTMPERPDLGDGroup(productName, dbContextType, rowId);

         foreach(var grouping in tmPerPdLgdGroupings) {
            var param = new ABSARetailPolingTMSegmentParam() {
               DbContextType = dbContextType, ClusterId = grouping.ClusterRowId, LgdRowLabelId = grouping.LGDRowLabelId, PdRowLebelId = grouping.PDRowLabelId,
               RetailPoolingFileType = productName, UniqueSegmentPDLGD = grouping.UniqueSegmentPDLGD, RowId = rowId, SegmentRowId = grouping.SegmentRowId
            };
            var tmPerPDLGDGroupData = _iDbContextAccess.GetTMPerPDLGDGroupData(param);
            var numberOfTransactionsInFile = _iDbContextAccess.GetSegmentTMCount(grouping.UniqueSegmentPDLGD, dbContextType);
            var pdLgdTonorLookup = GetLookupData(productName, numberOfTransactionsInFile, dbContextType);
            var numberOfTMPool = pdLgdTonorLookup.Tenor;
            var total = tmPerPDLGDGroupData.Sum(x => x.TMCount);

            var avgPerPool = ((double)total / (double)numberOfTMPool).NearestTenthWithRoundupAlways();
            var originalAvgPerPool = ((double)total / (double)numberOfTMPool);

            foreach(var item in tmPerPDLGDGroupData) {
               var ratio = (double)item.CumulativeSum / avgPerPool;
               item.Ratio = ratio;
               item.InitialRank = Math.Min((numberOfTMPool - 1), (int)ratio);
               item.AveragePerPool = avgPerPool;
               item.OriginalAveragePerPool = originalAvgPerPool;
            }
            //DB update and Clear
            var update = _iDbContextAccess.UpdateTMRatioInitialRankFinalPool(tmPerPDLGDGroupData, dbContextType, rowId);
            tmPerPDLGDGroupData.Clear();
         }
         tmPerPdLgdGroupings.Clear();
         return true;
      }

      private bool GenerateTMFinalPool(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId) {
         var tmPerPdLgdGroupings = _iDbContextAccess.GetTMPERPDLGDGroup(productName, dbContextType, rowId);

         foreach(var grouping in tmPerPdLgdGroupings) {
            var param = new ABSARetailPolingTMSegmentParam() {
               DbContextType = dbContextType, ClusterId = grouping.ClusterRowId, LgdRowLabelId = grouping.LGDRowLabelId, PdRowLebelId = grouping.PDRowLabelId,
               RetailPoolingFileType = productName, UniqueSegmentPDLGD = grouping.UniqueSegmentPDLGD, RowId = rowId, SegmentRowId = grouping.SegmentRowId
            };

            var tmPerPDLGDGroupData = _iDbContextAccess.GetTMPerPDLGDGroupData(param);
            var numberOfTransactionsInFile = _iDbContextAccess.GetSegmentTMCount(grouping.UniqueSegmentPDLGD, dbContextType);
            var pdLgdTonorLookup = GetLookupData(productName, numberOfTransactionsInFile, dbContextType);
            var numberOfLGPool = pdLgdTonorLookup.Tenor;
            var toleranceLevel = pdLgdTonorLookup.TenorTolerance;

            var total = tmPerPDLGDGroupData.Sum(x => x.TMCount);

            var avgPerPool = ((double)total /(double) numberOfLGPool).NearestTenthWithRoundupAlways();

            foreach(var item in tmPerPDLGDGroupData) {
               if(item.TMCount == item.CumulativeSum) {
                  item.FinalPool = 0;
               }
               else {
                  var previousItem = tmPerPDLGDGroupData.FirstOrDefault(x => x.Id == (item.Id - 1));
                  if(previousItem.FinalPool == item.InitialRank) {
                     item.FinalPool = previousItem.InitialRank;
                  }
                  else {
                     var tempContents = tmPerPDLGDGroupData.Where(x => x.Id >= 1).Where(x => x.Id <= (item.Id - 1)).ToList();
                     var sumValue = tempContents.Where(x => x.FinalPool == previousItem.FinalPool).Sum(x => x.TMCount);
                     var sliceSize = item.AveragePerPool * toleranceLevel;
                     if(sumValue < sliceSize) {
                        item.FinalPool = previousItem.FinalPool;
                     }
                     else {
                        item.FinalPool = previousItem.FinalPool + 1;
                     }
                     tempContents.Clear();
                  }
               }
            }
            //DB update and Clear
            var update = _iDbContextAccess.UpdateTMRatioInitialRankFinalPool(tmPerPDLGDGroupData, dbContextType, rowId);
            tmPerPDLGDGroupData.Clear();
         }
         tmPerPdLgdGroupings.Clear();
         return true;
      }

      #endregion RETAIL_POOLING_LOGIC
   }
}
