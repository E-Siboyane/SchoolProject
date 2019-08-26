using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class YieldCurveHistory : IDtoCommonMethod {
      public int Id { get; set; }
      public int JobId { get; set; }
      public DateTime CurveDate { get; set; }
      public string YieldCurve { get; set; }
      public decimal Tenor { get; set; }
      public decimal Rate { get; set; }

      private string YCH_HEADER_ROW = "curveDate,yieldCurve,tenor,rate";

      public dynamic ConvertToModel(List<string> fileListContents, int jobId, string fileContentDelimeter) {
         var yieldCurveHistory = new List<YieldCurveHistory>();
         fileListContents.ForEach(item => {
            var row = Extensions.ConvertCommaDelimetedStringToArray(item, fileContentDelimeter);
            yieldCurveHistory.Add(ConvertToCovarianceModelFactorCoefficients(row, jobId));
         });
         return yieldCurveHistory;
      }

      public string ConvertListToString<T>(List<T> itemsToConvertToString) where T : class {

         var contents = new StringBuilder();
         var headerRow = YCH_HEADER_ROW;
         contents.AppendLine(headerRow);

         foreach(var item in itemsToConvertToString) {
            var yieldCurveHistory = item as YieldCurveHistory;
            var row = string.Format("{0},{1},{2},{3}", yieldCurveHistory.CurveDate.Date, yieldCurveHistory.YieldCurve,
                                     yieldCurveHistory.Tenor, yieldCurveHistory.Rate);
            contents.AppendLine(row);
         }
         return contents.ToString();
      }

      public YieldCurveHistory ConvertToCovarianceModelFactorCoefficients(string[] content, int jobId) {
         try {
            return new YieldCurveHistory() {
               CurveDate = DateTime.Parse(content[0]).Date,
               YieldCurve = (string.IsNullOrEmpty(content[1].Trim()) || string.IsNullOrWhiteSpace(content[1].Trim())) ? null : content[1],
               Tenor = decimal.Parse(content[2]),
               Rate = decimal.Parse(content[3]),
               JobId = jobId
            };
         }
         catch(Exception ex) {
            Console.WriteLine($"{ex.Message} | Record With Error : {content[0]}");
            throw new NotImplementedException(ex.Message);
         }
      }
   }
}
