using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class ImportAsOfDate : IDtoCommonMethod {
      public int JobId { get; set; }
      public DateTime AsOfDate { get; set; }

      private string IMPORTASOFDATE_HEADER_ROW = "AsOfDate";

      public dynamic ConvertToModel(List<string> fileListContents, int jobId, string delimeterType) {
         var ImportAsOfDate = new List<ImportAsOfDate>();
         fileListContents.ForEach(item => {
            var row = Extensions.ConvertCommaDelimetedStringToArray(item, delimeterType);
            ImportAsOfDate.Add(ConvertToImportAsOfDate(row, jobId));
         });
         return ImportAsOfDate;
      }

      public string ConvertListToString<T>(List<T> itemsToConvertToString) where T : class {
         var contents = new StringBuilder();
         var headerRow = IMPORTASOFDATE_HEADER_ROW;
         contents.AppendLine(headerRow);
         foreach(var item in itemsToConvertToString) {
            var importAsOfDate = item as ImportAsOfDate;
            var row = string.Format("{0}", importAsOfDate.AsOfDate.Date);
            contents.AppendLine(row);
            //Add 1st row only
            break;
         }
         return contents.ToString();
      }

      private ImportAsOfDate ConvertToImportAsOfDate(string[] content, int jobId) {
         try {
            return new ImportAsOfDate() {
               JobId = jobId,
               AsOfDate = DateTime.Parse(content.FirstOrDefault()).Date
            };
         }
         catch(Exception ex) {
            Console.WriteLine($"{ex.Message} | Record  With Error : {content[0]}");
            throw new NotImplementedException(ex.Message);
         }
      }
   }
}