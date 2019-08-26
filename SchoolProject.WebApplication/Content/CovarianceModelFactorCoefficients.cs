using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class CovarianceModelFactorCoefficients : IDtoCommonMethod {
      public int Id { get; set; }
      public int JobId { get; set; }
      public string FactorSetName { get; set; }
      public DateTime WeightDate { get; set; }
      public string CounterpartyId { get; set; }
      public string FactorName { get; set; }
      public decimal FactorCoefficient { get; set; }

      private string CMFC_HEADER_ROW = "FactorSetName,WeightDate,CounterpartyId,FactorName,FactorCoefficient";

      public dynamic ConvertToModel(List<string> fileListContents, int jobId, string fileContentDelimeter) {
         var cmFactorCoefficients = new List<CovarianceModelFactorCoefficients>();
         fileListContents.ForEach(item => {
            var row = Extensions.ConvertCommaDelimetedStringToArray(item, fileContentDelimeter);
            cmFactorCoefficients.Add(ConvertToCovarianceModelFactorCoefficients(row, jobId));
         });
         return cmFactorCoefficients;
      }

      public string ConvertListToString<T>(List<T> itemsToConvertToString) where T : class {

         var contents = new StringBuilder();
         var headerRow = CMFC_HEADER_ROW;
         contents.AppendLine(headerRow);

         foreach(var item in itemsToConvertToString) {
            var cmfc = item as CovarianceModelFactorCoefficients;
            var row = string.Format("{0},{1},{2},{3},{4}", cmfc.FactorSetName, cmfc.WeightDate,
                                     cmfc.CounterpartyId, cmfc.FactorName, cmfc.FactorCoefficient);
            contents.AppendLine(row);
         }
         return contents.ToString();
      }

      public CovarianceModelFactorCoefficients ConvertToCovarianceModelFactorCoefficients(string[] content, int jobId) {
         try {
            return new CovarianceModelFactorCoefficients() {
               FactorSetName = (string.IsNullOrEmpty(content[0].Trim()) || string.IsNullOrWhiteSpace(content[0].Trim())) ? null : content[0],
               WeightDate = DateTime.Parse(content[1]).Date,
               CounterpartyId = (string.IsNullOrEmpty(content[2].Trim()) || string.IsNullOrWhiteSpace(content[2].Trim())) ? null : content[2],
               FactorName = (string.IsNullOrEmpty(content[3].Trim()) || string.IsNullOrWhiteSpace(content[3].Trim())) ? null : content[3],
               FactorCoefficient = decimal.Parse(content[4]),
               JobId = jobId
            };
         }
         catch(Exception ex) {
            Console.WriteLine($"{ex.Message} | Record Number : {content[0]}");
            throw new NotImplementedException(ex.Message);
         }
      }
   }
}
