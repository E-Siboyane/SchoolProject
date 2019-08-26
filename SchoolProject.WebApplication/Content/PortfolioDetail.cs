using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class PortfolioDetail : IDtoCommonMethod {
      public int Id { get; set; }
      public int JobId { get; set; }
      public string PortfolioName { get; set; }
      public string InstrumentId { get; set; }
      public double HoldingAmount { get; set; }
      public decimal? NumShare { get; set; }

      private string PORTFOLIODETAILS_HEADER_ROW = "PortfolioName,InstrumentId,HoldingAmount,NumShare";

      public dynamic ConvertToModel(List<string> fileListContents, int jobId, string fileContentDelimeter) {
         var portfolioDetails = new List<PortfolioDetail>();
         fileListContents.ForEach(item => {
            var row = Extensions.ConvertCommaDelimetedStringToArray(item, fileContentDelimeter);
            portfolioDetails.Add(ConvertToPortfolioDetail(row, jobId));
         });
         return portfolioDetails;
      }

      public string ConvertListToString<T>(List<T> itemsToConvertToString) where T : class {
         var contents = new StringBuilder();
         var headerRow = PORTFOLIODETAILS_HEADER_ROW;
         contents.AppendLine(headerRow);

         foreach(var pDetails in itemsToConvertToString) {
            var item = pDetails as PortfolioDetail;
            var row = string.Format("{0},{1},{2},{3}", item.PortfolioName, item.InstrumentId, item.HoldingAmount, item.NumShare);
            contents.AppendLine(row);
         }
         return contents.ToString();
      }

      private PortfolioDetail ConvertToPortfolioDetail(string[] content, int jobId) {
         try {
            return new PortfolioDetail() {
               PortfolioName = (string.IsNullOrEmpty(content[0].Trim()) || string.IsNullOrWhiteSpace(content[0].Trim())) ? null : content[0],
               InstrumentId = (string.IsNullOrEmpty(content[1].Trim()) || string.IsNullOrWhiteSpace(content[1].Trim())) ? null : content[1],
               HoldingAmount = double.Parse(content[2]),
               NumShare = string.IsNullOrEmpty(content[3]) ? (decimal?)null : decimal.Parse(content[3]),
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
