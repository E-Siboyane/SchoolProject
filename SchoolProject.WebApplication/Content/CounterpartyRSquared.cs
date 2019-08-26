using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
    public class CounterpartyRSquared: IDtoCommonMethod {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string CounterPartyId { get; set; }
        public string CorrelationModelEstimateName { get; set; }
        public decimal RSquared { get; set; }

        private string COUNTERPARTYRSQURED_HEADER_ROW = "CounterPartyId,CorrelationModelEstimateName,RSquared";
        public dynamic ConvertToModel(List<string> fileListContents, int jobId, string fileContentDelimeter) {
            var counterpartyRSquares = new List<CounterpartyRSquared>();
            fileListContents.ForEach(item => {
                var row = Extensions.ConvertCommaDelimetedStringToArray(item, fileContentDelimeter);
                counterpartyRSquares.Add(ConvertToCounterPartyRSquared(row, jobId));
            });
            return counterpartyRSquares;
        }
        public string ConvertListToString<T>(List<T> itemsToConvertToString) where T : class {
            var contents = new StringBuilder();
            var headerRow = COUNTERPARTYRSQURED_HEADER_ROW;
            contents.AppendLine(headerRow);

            foreach (var item in itemsToConvertToString) {
                var counterpartyRSquared = item as CounterpartyRSquared;
                var row = string.Format("{0},{1},{2}", counterpartyRSquared.CounterPartyId, counterpartyRSquared.CorrelationModelEstimateName, 
                                         counterpartyRSquared.RSquared);
                contents.AppendLine(row);
            }
            return contents.ToString();
        }

        public CounterpartyRSquared ConvertToCounterPartyRSquared(string[] content, int jobId) {
         try {
            return new CounterpartyRSquared() {
               CounterPartyId = (string.IsNullOrEmpty(content[0].Trim()) || string.IsNullOrWhiteSpace(content[0].Trim())) ? null : content[0],
               CorrelationModelEstimateName = (string.IsNullOrEmpty(content[1].Trim()) || string.IsNullOrWhiteSpace(content[1].Trim())) ? null : content[1],
               RSquared = decimal.Parse(content[2]),
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
