using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class Portfolio : IDtoCommonMethod {
      public int Id { get; set; }
      public int JobId { get; set; }
      public string PortfolioName { get; set; }
      public DateTime? PortfolioActiveUntilDate { get; set; }

      private string PORTFOLIO_HEADER_ROW = "PortfolioName,PortfolioActiveUntilDate";

      public dynamic ConvertToModel(List<string> fileListContents, int jobId, string fileContentDelimeter) {
         var portfolios = new List<Portfolio>();
         fileListContents.ForEach(item => {
            var row = Extensions.ConvertCommaDelimetedStringToArray(item, fileContentDelimeter);
            portfolios.Add(ConvertToPortfolio(row, jobId));
         });
         return portfolios;
      }

      public string ConvertListToString<T>(List<T> itemsToConvertToString) where T : class {
         var contents = new StringBuilder();
         var headerRow = PORTFOLIO_HEADER_ROW;
         contents.AppendLine(headerRow);

         foreach(var portfolio in itemsToConvertToString) {
            var item = portfolio as Portfolio;
            var row = string.Format("{0},{1}", item.PortfolioName, item.PortfolioActiveUntilDate);
            contents.AppendLine(row);
         }
         return contents.ToString();
      }

      private Portfolio ConvertToPortfolio(string[] content, int jobId) {
         try {
            var portfolio = new Portfolio() {
               PortfolioName = (string.IsNullOrEmpty(content[0].Trim()) || string.IsNullOrWhiteSpace(content[0].Trim())) ? null : content[0],
               JobId = jobId
            };
            if(!string.IsNullOrEmpty(content[1]))
               if(Extensions.TryParseDate(content[1]))
                  portfolio.PortfolioActiveUntilDate = DateTime.Parse(content[1]).Date;
            return (portfolio);
         }
         catch(Exception ex) {
            Console.WriteLine($"{ex.Message} | Record  With Error : {content[0]}");
            throw new NotImplementedException(ex.Message);
         }
      }
   }
}
