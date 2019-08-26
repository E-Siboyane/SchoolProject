using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class InstrumentPdsFlexible : IDtoCommonMethod {
      public int Id { get; set; }
      public int JobId { get; set; }
      public string InstrumentId { get; set; }
      public DateTime BeginDate { get; set; }
      public DateTime? EndDate { get; set; }
      public int Term { get; set; }
      public decimal Pd { get; set; }

      private string INSTRUMENTODSFLEXIBLE_HEADER_ROW = "InstrumentId,BeginDate,EndDate,Term,Pd";

      public dynamic ConvertToModel(List<string> fileListContents, int jobId, string fileContentDelimeter) {
         var instrumentPdsFlexible = new List<InstrumentPdsFlexible>();
         fileListContents.ForEach(item => {
            var row = Extensions.ConvertCommaDelimetedStringToArray(item, fileContentDelimeter);
            instrumentPdsFlexible.Add(ConvertToInstrumentPdsFlexible(row, jobId));
         });
         return instrumentPdsFlexible;
      }

      public string ConvertListToString<T>(List<T> itemsToConvertToString) where T : class {
         var contents = new StringBuilder();
         var headerRow = INSTRUMENTODSFLEXIBLE_HEADER_ROW;
         contents.AppendLine(headerRow);

         foreach(var item in itemsToConvertToString) {
            var instrumentPdsFlexible = item as InstrumentPdsFlexible;
            var row = string.Format("{0},{1},{2},{3},{4}", instrumentPdsFlexible.InstrumentId, instrumentPdsFlexible.BeginDate,
                                     instrumentPdsFlexible.EndDate, instrumentPdsFlexible.Term, instrumentPdsFlexible.Pd);
            contents.AppendLine(row);
         }
         return contents.ToString();
      }

      private InstrumentPdsFlexible ConvertToInstrumentPdsFlexible(string[] content, int jobId) {
         try {
            var instrument = new InstrumentPdsFlexible() {
               InstrumentId = (string.IsNullOrEmpty(content[0].Trim()) || string.IsNullOrWhiteSpace(content[0].Trim())) ? null : content[0],
               BeginDate = DateTime.Parse(content[1]).Date,
               Term = int.Parse(content[3]),
               Pd = decimal.Parse(content[4]),
               JobId = jobId
            };

            if(!string.IsNullOrEmpty(content[2]))
               if(Extensions.TryParseDate(content[2]))
                  instrument.EndDate = DateTime.Parse(content[2]).Date;
            return instrument;
         }
         catch(Exception ex) {
            Console.WriteLine($"{ex.Message} | Record Number : {content[0]}");
            throw new NotImplementedException(ex.Message);
         }
      }
   }
}
