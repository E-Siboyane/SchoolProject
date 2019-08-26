using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class InstrumentRating : IDtoCommonMethod {
      public int Id { get; set; }
      public int JobId { get; set; }
      public string InstrumentId { get; set; }
      public DateTime RatingDate { get; set; }
      public DateTime? RatingEndDate { get; set; }
      public string RatingSystemName { get; set; }
      public string Rating { get; set; }

      private string INSTRUMENT_HEADER_ROW = "InstrumentId,RatingDate,RatingEndDate,RatingSystemName,Rating";

      public dynamic ConvertToModel(List<string> fileListContents, int jobId, string fileContentDelimeter) {
         var instrumentRatings = new List<InstrumentRating>();
         fileListContents.ForEach(item => {
            var row = Extensions.ConvertCommaDelimetedStringToArray(item, fileContentDelimeter);
            instrumentRatings.Add(ConvertToInstrumentRating(row, jobId));
         });
         return instrumentRatings;
      }

      public string ConvertListToString<T>(List<T> itemsToConvertToString) where T : class {
         var contents = new StringBuilder();
         var headerRow = INSTRUMENT_HEADER_ROW;
         contents.AppendLine(headerRow);

         foreach(var item in itemsToConvertToString) {
            var instrumentRating = item as InstrumentRating;
            var row = string.Format("{0},{1},{2},{3},{4}", instrumentRating.InstrumentId, instrumentRating.RatingDate,
                                    instrumentRating.RatingEndDate, instrumentRating.RatingSystemName, instrumentRating.Rating);
            contents.AppendLine(row);
         }
         return contents.ToString();
      }

      public InstrumentRating ConvertToInstrumentRating(string[] content, int jobId) {
         try {
            var rating = new InstrumentRating() {
               InstrumentId = (string.IsNullOrEmpty(content[0].Trim()) || string.IsNullOrWhiteSpace(content[0].Trim())) ? null : content[0],
               RatingDate = DateTime.Parse(content[1]).Date,
               RatingSystemName = (string.IsNullOrEmpty(content[3].Trim()) || string.IsNullOrWhiteSpace(content[3].Trim())) ? null : content[3],
               Rating = (string.IsNullOrEmpty(content[4].Trim()) || string.IsNullOrWhiteSpace(content[4].Trim())) ? null : content[4],
               JobId = jobId
            };

            if(!string.IsNullOrEmpty(content[2]))
               if(Extensions.TryParseDate(content[2]))
                  rating.RatingEndDate = DateTime.Parse(content[2]);
            return (rating);
         }
         catch(Exception ex) {
            Console.WriteLine($"{ex.Message} | Record  With Error : {content[0]}");
            throw new NotImplementedException(ex.Message);
         }
      }
   }
}
