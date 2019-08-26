using EconomicCapital.RF.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EconomicCapital.RF.BusinessLogic {
   public interface IFileManager {
      bool CreateContent(string fileName, string baseDirectory, string fileTextContents, string batchRunFolderName);
      bool CreatePoolingOutputFiles(string fileName, string baseDirectory, List<RetailPoolingInput> fileTextContents, string batchRunFolderName);
      bool CreatePoolingOutputFilePD(string fileName, string baseDirectory, List<ABSARetailPoolingPDSegment> contents, string batchRunFolderName);
      bool CreatePoolingOutputFileLGD(string fileName, string baseDirectory, List<ABSARetailPoolingLGDSegment> contents, string batchRunFolderName);
      bool CreatePoolingOutputFileTM(string fileName, string baseDirectory, List<ABSARetailPoolingTMSegment> contents, string batchRunFolderName);
      List<string> ReadCSVFile(string filePath);
      string GetLastFolder(string baseDirectory);
      List<FileInfo> GetProcessingDirectoryFiles(string baseDirectory);
      List<string> SearchFiles(string baseDirectory);
      string[] ConvertCommaDelimetedStringToArray(string contentToConvertToArray);
   }
}
