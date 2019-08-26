using EconomicCapital.RF.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using System.Diagnostics;
using EconomicCapital.RF.DataAccess;
using Ninject;


namespace EconomicCapital.RF.BusinessLogic {
   public class FileManager : IFileManager {
      private string BATCHRUNFOLDER { get { return string.Format("EC_RUN_{0}_{1}", DateTime.Now.Date.ToString("yyyyMMdd"), DateTime.Now.Ticks); } }
      private string BASEDIRECTORY { get { return ConfigurationManager.AppSettings.Get("SharedBaseDirectory"); } }

      public FileManager() {; }

      public List<FileInfo> GetProcessingDirectoryFiles(string baseDirectory) {
         var directoryFiles = new List<FileInfo>();
         if(Directory.Exists(@baseDirectory))
            directoryFiles = new DirectoryInfo(GetLastFolder(BASEDIRECTORY)).GetFiles().ToList();
         return directoryFiles;
      }

      public bool CreateContent(string fileName, string baseDirectory, string fileTextContents, string batchRunFolderName) {
         var result = false;
         if(Directory.Exists(@baseDirectory)) {
            var folderName = batchRunFolderName;
            var batchFolder = Path.Combine(baseDirectory, folderName);
            if(!Directory.Exists(batchFolder)) {
               Directory.CreateDirectory(batchFolder);
            }

            var fileNamePath = string.Format(@"{0}\{1}.csv", batchFolder, fileName);
            if(!File.Exists(fileNamePath)) {
               File.WriteAllText(fileNamePath, fileTextContents);
               result = true;
            }
            else {
               result = false;
            }
         }
         return result;
      }

      public bool CreatePoolingOutputFiles(string fileName, string baseDirectory, List<RetailPoolingInput> contents, string batchRunFolderName) {
         var result = false;
         if(contents.Count > 0) {
            if(Directory.Exists(@baseDirectory)) {
               var batchFolder = Path.Combine(baseDirectory, batchRunFolderName);
               if(!Directory.Exists(batchFolder)) {
                  Directory.CreateDirectory(batchFolder);
               }

               var fileNamePath = string.Format(@"{0}\{1}.csv", batchFolder, fileName);
               if(!File.Exists(fileNamePath)) {
                  using(File.Create(fileNamePath)) { }
               }
               using(StreamWriter sr = File.AppendText(fileNamePath)) {
                  sr.WriteLine("ObligorId|FacilityId|RSq|QSq|AssetReturnWeights|" +
                               $"RecoveryWeights|Tenors|TimeToMaturity|CumulativePDs|" +
                               $"LGD|EAD|SUBMISSION_UNIT_CD|EC_SEG|PROD|EC_CLUSTER|" +
                               $"UNIQUE_KEY|FinalPDRank|FinalLGDRank|FinalTMRank|PoolingLGD|OrginalPoolingLGD");

                  foreach(var item in contents) {
                     var p1 = item as RetailPoolingInput;
                     var row = $"{p1.ObligorId}|{p1.FacilityId}|{p1.RSq}|{p1.QSq}|{p1.AssetReturnWeights}|" +
                               $"{p1.RecoveryWeights}|{p1.Tenors}|{p1.TimeToMaturity}|{p1.CumulativePDs}|" +
                               $"{p1.LGD}|{p1.EAD}|{p1.SUBMISSION_UNIT_CD}|{p1.EC_SEG}|{p1.PROD}|{p1.EC_CLUSTER}|" +
                               $"{p1.UNIQUE_KEY}|{p1.FinalPDRank}|{p1.FinalLGDRank}|{p1.FinalTMRank}|{p1.PoolingLGD}|{p1.OrginalPoolingLGD}";
                     sr.WriteLine(row);
                  }
                  result = true;
               }
            }
         }
         return result;
      }

      public bool CreatePoolingOutputFilePD(string fileName, string baseDirectory, List<ABSARetailPoolingPDSegment> contents, string batchRunFolderName) {
         var result = false;
         if(contents.Count > 0) {
            if(Directory.Exists(@baseDirectory)) {
               var batchFolder = Path.Combine(baseDirectory, batchRunFolderName);
               if(!Directory.Exists(batchFolder)) {
                  Directory.CreateDirectory(batchFolder);
               }

               var fileNamePath = string.Format(@"{0}\{1}.csv", batchFolder, fileName);
               if(!File.Exists(fileNamePath)) {
                  using(File.Create(fileNamePath)) { }
               }
               using(StreamWriter sr = File.AppendText(fileNamePath)) {
                  var header = "Cluster|Segment|PD|CountOfPD|CumulativeSum|Ratio|InitialRank|Count|AvgPerPool|RoundedUpAvgPerPool|FinalPool";
                  sr.WriteLine(header);

                  foreach(var item in contents) {
                     var p = item as ABSARetailPoolingPDSegment;
                     var row = $"{p.Cluster}|{p.Segment}|{p.PD}|{p.CountOfPd}|{p.CumulativeSum}|{p.Ratio}|{p.InitialRank}|{p.Count}|{p.AveragePerPool}|" +
                               $"{p.OriginalAveragePerPool}|{p.FinalPool}";
                     sr.WriteLine(row);
                  }
                  result = true;
               }
            }
         }
         return result;
      }

      public bool CreatePoolingOutputFileLGD(string fileName, string baseDirectory, List<ABSARetailPoolingLGDSegment> contents, string batchRunFolderName) {
         var result = false;
         if(contents.Count > 0) {
            if(Directory.Exists(@baseDirectory)) {
               var batchFolder = Path.Combine(baseDirectory, batchRunFolderName);
               if(!Directory.Exists(batchFolder)) {
                  Directory.CreateDirectory(batchFolder);
               }

               var fileNamePath = string.Format(@"{0}\{1}.csv", batchFolder, fileName);
               if(!File.Exists(fileNamePath)) {
                  using(File.Create(fileNamePath)) { }
               }
               using(StreamWriter sr = File.AppendText(fileNamePath)) {
                  var header = $"Cluster|Segment|PDRowLabel|LGD|LGDCount|CumulativeSum|Ratio|InitialRank|" +
                               $"RoundedUpAvgPerPool|AveragePerPool|Count|UniqueLGDCountPerPDPool|FinalPool";
                  sr.WriteLine(header);

                  foreach(var item in contents) {
                     var p = item as ABSARetailPoolingLGDSegment;
                     var row = $"{p.Cluster}|{p.Segment}|{p.PDRowLabelId}|{p.LGD}|{p.LGDCount}|{p.CumulativeSum}|{p.Ratio}|{p.InitialRank}" +
                               $"|{p.AveragePerPool}|{p.OriginalAveragePerPool}|{p.Count}|{p.UniqueLGDCountPerPDPool}|{p.FinalPool}";
                     sr.WriteLine(row);
                  }
                  result = true;
               }
            }
         }
         return result;
      }

      public bool CreatePoolingOutputFileTM(string fileName, string baseDirectory, List<ABSARetailPoolingTMSegment> contents, string batchRunFolderName) {
         var result = false;
         if(contents.Count > 0) {
            if(Directory.Exists(@baseDirectory)) {
               var batchFolder = Path.Combine(baseDirectory, batchRunFolderName);
               if(!Directory.Exists(batchFolder)) {
                  Directory.CreateDirectory(batchFolder);
               }

               var fileNamePath = string.Format(@"{0}\{1}.csv", batchFolder, fileName);
               if(!File.Exists(fileNamePath)) {
                  using(File.Create(fileNamePath)) { }
               }
               using(StreamWriter sr = File.AppendText(fileNamePath)) {
                  var header = $"Cluster|Segment|PDRowLabelId|LGDRowLabelId|TM|TMCount|CumulativeSum|Ratio|InitialRank|" +
                               $"RoundedUpAvgPerPool|AveragePerPool|FinalPool";
                  sr.WriteLine(header);

                  foreach(var item in contents) {
                     var p = item as ABSARetailPoolingTMSegment;
                     var row = $"{p.Cluster}|{p.Segment}|{p.PDRowLabelId}|{p.LGDRowLabelId}|{p.TM}|{p.TMCount}|{p.CumulativeSum}|{p.Ratio}|{p.InitialRank}|" +
                               $"{p.AveragePerPool}|{p.OriginalAveragePerPool}|{p.FinalPool}";
                     sr.WriteLine(row);
                  }
                  result = true;
               }
            }
         }
         return result;
      }

      public List<string> ReadCSVFile(string filePath) {
         var result = new List<string>();
         if(File.Exists(@filePath)) {
            var fileInfo = new FileInfo(@filePath);
            result = fileInfo.Length > 0 ? File.ReadAllLines(@filePath).ToList() : null;
         }
         //Replace the header record
         var output = result.Skip(1).ToList();
         return output;
      }

      public string[] ConvertCommaDelimetedStringToArray(string contentToConvertToArray) {
         if(!string.IsNullOrEmpty(contentToConvertToArray)) {
            return contentToConvertToArray.Split(new string[] { "," }, StringSplitOptions.None);
         }
         return null;
      }

      public string[] ConvertPipedDelimetedStringToArray(string contentToConvertToArray) {
         if(!string.IsNullOrEmpty(contentToConvertToArray)) {
            return contentToConvertToArray.Split(new string[] { "|" }, StringSplitOptions.None);
         }
         return null;
      }

      public string GetLastFolder(string baseDirectory) {
         var directory = string.Empty;
         if(Directory.Exists(@baseDirectory)) {
            directory = new DirectoryInfo(baseDirectory).GetDirectories()
                  .OrderByDescending(d => d.LastWriteTimeUtc).FirstOrDefault().FullName;
         }
         return directory;
      }

      public List<string> SearchFiles(string baseDirectory) {
         var result = new List<string>();
         var supportedExtentions = ConfigurationManager.AppSettings.Get("SupportedFileExtention").ToLower();
         if(Directory.Exists(@baseDirectory)) {
            result = Directory.GetFiles(@baseDirectory, "*.*", SearchOption.TopDirectoryOnly).
                                         Where(f => supportedExtentions.Contains(Path.GetExtension(f).ToLower())).ToList();
         }
         return result;
      }
   }
}
