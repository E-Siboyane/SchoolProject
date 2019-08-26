using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class RetailBucketNumber : IDtoCommonMethod {
      public string Prod { get; set; }
      public string Seg { get; set; }
      public string PD_buckets { get; set; }
      public string LGD_buckets { get; set; }
      public string TM_buckets { get; set; }

      private string RETAILBUCKET_HEADER_ROW = "Prod,Seg,PD_buckets,LGD_buckets,TM_buckets";

      public dynamic ConvertToModel(List<string> fileListContents, int jobid, string fileContentDelimeter) {
         var retailBucketNumbers = new List<RetailBucketNumber>();
         fileListContents.ForEach(item => {
            var row = Extensions.LowMemSplit(item, fileContentDelimeter);
            retailBucketNumbers.Add(ConvertToRetailBucketNumber(row));
         });
         return retailBucketNumbers;
      }

      public string ConvertListToString<T>(List<T> itemsToConvertToString) where T : class {

         var contents = new StringBuilder();
         var headerRow = RETAILBUCKET_HEADER_ROW;
         contents.AppendLine(headerRow);

         foreach(var item in itemsToConvertToString) {
            var retailBucket = item as RetailBucketNumber;
            var row = string.Format("{0},{1},{2},{3},{4}", retailBucket.Prod, retailBucket.Seg, retailBucket.PD_buckets, retailBucket.LGD_buckets, retailBucket.TM_buckets);
            contents.AppendLine(row);
         }
         return contents.ToString();
      }

      public RetailBucketNumber ConvertToRetailBucketNumber(string[] content) {
         try {
            return new RetailBucketNumber() {
               Prod = content[0].Trim(),
               Seg = content[1].Trim(),
               PD_buckets = content[2].Trim(),
               LGD_buckets = content[3].Trim(),
               TM_buckets = content[4].Trim()
            };
         }
         catch(Exception ex) {
            Console.WriteLine($"{ex.Message} | Record  With Error : {content[0]}");
            throw new NotImplementedException(ex.Message);
         }
      }
   }
}
