using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class Counterparty : IDtoCommonMethod {
      public int Id { get; set; }
      public int JobId { get; set; }
      public string CounterPartyId { get; set; }
      public string Pid { get; set; }
      public string CounterPartyName { get; set; }
      public string CounterPartyDescription { get; set; }
      public string CounterPartyCountryCode { get; set; }
      public int? UserVariableInt { get; set; }
      public string UserVariableString1 { get; set; }
      public string UserVariableString2 { get; set; }
      public string UserVariableString3 { get; set; }
      public string CounterpartyType { get; set; }
      public bool IsPublicFlag { get; set; }

      private string COUNTERPARTY_HEADER_ROW = string.Format("{0}{1}{2}{3}", "CounterPartyId,Pid,CounterPartyName,",
                                                           "CounterPartyDescription,CounterPartyCountryCode,UserVariableInt,",
                                                           "UserVariableString1,UserVariableString2,UserVariableString3,",
                                                           "CounterpartyType,IsPublicFlag");
      public dynamic ConvertToModel(List<string> fileListContents, int jobId, string fileContentDelimeter) {
         var couterparties = new List<Counterparty>();
         fileListContents.ForEach(item => {
            var row = Extensions.ConvertCommaDelimetedStringToArray(item, fileContentDelimeter);
            couterparties.Add(ConvertToCounterParty(row, jobId));
         });
         return couterparties;
      }

      public string ConvertListToString<T>(List<T> itemsToConvertToString) where T : class {
         var contents = new StringBuilder();
         var headerRow = COUNTERPARTY_HEADER_ROW;
         contents.AppendLine(headerRow);

         foreach(var item in itemsToConvertToString) {
            var counterparty = item as Counterparty;
            var row = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", counterparty.CounterPartyId, counterparty.Pid,
                                    counterparty.CounterPartyName, counterparty.CounterPartyDescription,
                                    counterparty.CounterPartyCountryCode, counterparty.UserVariableInt, counterparty.UserVariableString1,
                                    counterparty.UserVariableString2, counterparty.UserVariableString3, counterparty.CounterpartyType,
                                    counterparty.IsPublicFlag);
            contents.AppendLine(row);
         }
         return contents.ToString();
      }

      private Counterparty ConvertToCounterParty(string[] content, int jobId) {
         try {
            return new Counterparty() {
               CounterPartyId = (string.IsNullOrEmpty(content[0].Trim()) || string.IsNullOrWhiteSpace(content[0].Trim())) ? null : content[0],
               Pid = (string.IsNullOrEmpty(content[1].Trim()) || string.IsNullOrWhiteSpace(content[1].Trim())) ? null : content[1],
               CounterPartyName = (string.IsNullOrEmpty(content[2].Trim()) || string.IsNullOrWhiteSpace(content[2].Trim())) ? null : content[2],
               CounterPartyDescription = (string.IsNullOrEmpty(content[3].Trim()) || string.IsNullOrWhiteSpace(content[3].Trim())) ? null : content[3],
               CounterPartyCountryCode = (string.IsNullOrEmpty(content[4].Trim()) || string.IsNullOrWhiteSpace(content[4].Trim())) ? null : content[4],
               UserVariableInt = string.IsNullOrEmpty(content[5]) ? (int?)null : int.Parse(content[5]),
               UserVariableString1 = (string.IsNullOrEmpty(content[6].Trim()) || string.IsNullOrWhiteSpace(content[6].Trim())) ? null : content[6],
               UserVariableString2 = (string.IsNullOrEmpty(content[7].Trim()) || string.IsNullOrWhiteSpace(content[7].Trim())) ? null : content[7],
               UserVariableString3 = (string.IsNullOrEmpty(content[8].Trim()) || string.IsNullOrWhiteSpace(content[8].Trim())) ? null : content[8],
               CounterpartyType = (string.IsNullOrEmpty(content[9].Trim()) || string.IsNullOrWhiteSpace(content[9].Trim())) ? null : content[9],
               IsPublicFlag = content[10].ToBoolean(),
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
