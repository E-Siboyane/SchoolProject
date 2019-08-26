using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class ExposureUDV : IDtoCommonMethod {
      public int Id { get; set; }
      public int JobId { get; set; }
      public string PortfolioName { get; set; }
      public string InstrumentId { get; set; }
      public string ExposureUDVId { get; set; }
      public string ExposureUDVValue { get; set; }

      private string EXPOSUREUDV_HEADER_ROW = "PortfolioName,InstrumentId,ExposureUDVId,ExposureUDVValue";

      public dynamic ConvertToModel(List<string> fileListContents, int jobId, string fileContentDelimeter) {
         var ExposureUDVs = new List<ExposureUDV>();
         fileListContents.ForEach(item => {
            var row = Extensions.ConvertCommaDelimetedStringToArray(item, fileContentDelimeter);
            ExposureUDVs.Add(ConvertToExposureUDV(row, jobId));
         });
         return ExposureUDVs;
      }

      public string ConvertListToString<T>(List<T> itemsToConvertToString) where T : class {

         var contents = new StringBuilder();
         var headerRow = EXPOSUREUDV_HEADER_ROW;
         contents.AppendLine(headerRow);

         foreach(var item in itemsToConvertToString) {
            var exposureUDV = item as ExposureUDV;
            var row = string.Format("{0},{1},{2},{3}", exposureUDV.PortfolioName, exposureUDV.InstrumentId,
                                     exposureUDV.ExposureUDVId, exposureUDV.ExposureUDVValue);
            contents.AppendLine(row);
         }
         return contents.ToString();
      }

      public ExposureUDV ConvertToExposureUDV(string[] content, int jobId) {
         try {
            return new ExposureUDV() {
               PortfolioName = (string.IsNullOrEmpty(content[0].Trim()) || string.IsNullOrWhiteSpace(content[0].Trim())) ? null : content[0],
               InstrumentId = (string.IsNullOrEmpty(content[1].Trim()) || string.IsNullOrWhiteSpace(content[1].Trim())) ? null : content[1],
               ExposureUDVId = (string.IsNullOrEmpty(content[2].Trim()) || string.IsNullOrWhiteSpace(content[2].Trim())) ? null : content[2],
               ExposureUDVValue = (string.IsNullOrEmpty(content[3].Trim()) || string.IsNullOrWhiteSpace(content[3].Trim())) ? null : content[3],
               JobId = jobId
            };
         }
         catch(Exception ex) {
            Console.WriteLine($"{ex.Message} | Record  With Error : {content[0]}");
            throw new NotImplementedException(ex.Message);
         }
      }
   }
}
