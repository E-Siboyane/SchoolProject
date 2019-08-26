using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class InstrumentPrice : IDtoCommonMethod {
      public int Id { get; set; }
      public int JobId { get; set; }
      public string InstrumentId { get; set; }
      public DateTime PriceDate { get; set; }
      public decimal AnalysisDatePrice { get; set; }
      public decimal? Horizon { get; set; }
      public decimal? HorizonPrice { get; set; }

      private string INSTRUMENTRICE_HEADER_ROW = "InstrumentId,PriceDate,AnalysisDatePrice,Horizon,HorizonPrice";

      public dynamic ConvertToModel(List<string> fileListContents, int jobId, string fileContentDelimeter) {
         var instrumentPrices = new List<InstrumentPrice>();
         fileListContents.ForEach(item => {
            var row = Extensions.ConvertCommaDelimetedStringToArray(item, fileContentDelimeter);
            instrumentPrices.Add(ConvertToInstrumentPrice(row, jobId));
         });
         return instrumentPrices;
      }

      public string ConvertListToString<T>(List<T> itemsToConvertToString) where T : class {
         var contents = new StringBuilder();
         var headerRow = INSTRUMENTRICE_HEADER_ROW;
         contents.AppendLine(headerRow);

         foreach(var item in itemsToConvertToString) {
            var instrumentPrice = item as InstrumentPrice;
            var row = string.Format("{0},{1},{2},{3},{4}", instrumentPrice.InstrumentId, instrumentPrice.PriceDate,
                                     instrumentPrice.AnalysisDatePrice, instrumentPrice.Horizon, instrumentPrice.HorizonPrice);
            contents.AppendLine(row);
         }
         return contents.ToString();
      }

      public InstrumentPrice ConvertToInstrumentPrice(string[] content, int jobId) {
         try {
            return new InstrumentPrice() {
               InstrumentId = (string.IsNullOrEmpty(content[0].Trim()) || string.IsNullOrWhiteSpace(content[0].Trim())) ? null : content[0],
               PriceDate = DateTime.Parse(content[1]),
               AnalysisDatePrice = decimal.Parse(content[2]),
               Horizon = string.IsNullOrEmpty(content[3]) ? (decimal?)null : decimal.Parse(content[3]),
               HorizonPrice = string.IsNullOrEmpty(content[3]) ? (decimal?)null : decimal.Parse(content[3]),
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
