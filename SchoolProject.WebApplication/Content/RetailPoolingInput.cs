using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class RetailPoolingInput : IDtoCommonMethod {
      public string ObligorId { get; set; }
      public string FacilityId { get; set; }
      public double RSq { get; set; }
      public double QSq { get; set; }
      public string AssetReturnWeights { get; set; }
      public string RecoveryWeights { get; set; }
      public int Tenors { get; set; }
      public int TimeToMaturity { get; set; }
      public double CumulativePDs { get; set; }
      public double LGD { get; set; }
      public double EAD { get; set; }
      public string SUBMISSION_UNIT_CD { get; set; }
      public string EC_SEG { get; set; }
      public string PROD { get; set; }
      public string EC_CLUSTER { get; set; }
      public string UNIQUE_KEY { get; set; }
      public int FinalPDRank { get; set; }
      public int FinalLGDRank { get; set; }
      public int FinalTMRank { get; set; }
      public DateTime CreatedDate { get; set; }
      public string InputType { get; set; }
      public Guid RowId { get; set; }
      public double PoolingLGD { get; set; }
      public double OrginalPoolingLGD { get; set; }

      public string P1_HEADER_ROW = "ObligorID|FacilityID|RSq|QSq|AssetReturnWeights|RecoveryWeights|Tenors|TimeToMaturity|CumulativePDs|" +
                                     "LGD|EAD|SUBMISSION_UNIT_CD|EC_SEG|PROD|EC_CLUSTER|PROD|UNIQUE_KEY|" +
                                     "FinalPDRank|FinalLGDRank|FinalTMRank|PoolingLGD_Rounded|OrginalLGD";

      public dynamic ConvertToModel(List<string> fileListContents, int jobid, string fileContentDelimeter) {
         var p1List = new List<RetailPoolingInput>();
         foreach(var item in fileListContents) {
            var row = Extensions.LowMemSplit(item, fileContentDelimeter);
            p1List.Add(ConvertToP1(row));
         }
         return p1List;
      }

      public string ConvertListToString<T>(List<T> itemsToConvertToString) where T : class {
         throw new NotImplementedException();
         //var contents = new StringBuilder();
         //var headerRow = P1_HEADER_ROW;
         //contents.AppendLine(headerRow);

         //foreach (var item in itemsToConvertToString) {
         //    var p1 = item as RetailPoolingInput;
         //    var row = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17}|{18}|{19}|{20}|{21}|{22}|{23}|{24}|{25}", p1.ObligorId, p1.FacilityId, p1.RSq, p1.QSq, 
         //                             p1.AssetReturnWeights, p1.RecoveryWeights, p1.Tenors,p1.TimeToMaturity,p1.CumulativePDs,p1.LGD,p1.EAD,p1.SUBMISSION_UNIT_CD, p1.EC_SEG, p1.PROD,p1.EC_CLUSTER,
         //                             p1.UNIQUE_KEY, p1.RankByPD, p1.RankkByLGD, p1.RankByTM, p1.RowId, p1.NewPDRank, p1.NewLGDRank, p1.NewTMRank,p1.FinalPDRank,p1.FinalLGDRank, p1.FinalTMRank);
         //    contents.AppendLine(row);
         //}
         //return contents.ToString();
      }

      public RetailPoolingInput ConvertToP1(string[] content) {
         try {
            return new RetailPoolingInput() {
               ObligorId = content[0].Trim(),
               FacilityId = content[1].Trim(),
               RSq = double.Parse(content[2].Trim()),
               QSq = double.Parse(content[3].Trim()),
               AssetReturnWeights = content[4].Trim(),
               RecoveryWeights = content[5].Trim(),
               Tenors = int.Parse(content[6].Trim()),
               TimeToMaturity = int.Parse(content[7].Trim()),
               CumulativePDs = !string.IsNullOrEmpty(content[8].Trim()) ? double.Parse(content[8].Trim()) : 0,
               LGD = !string.IsNullOrEmpty(content[9].Trim()) ? double.Parse(content[9].Trim()) : 0,
               EAD = !string.IsNullOrEmpty(content[10].Trim()) ? double.Parse(content[10].Trim()) : 0,
               SUBMISSION_UNIT_CD = content[11].Trim(),
               EC_SEG = content[12].Trim(),
               PROD = content[13].Trim(),
               EC_CLUSTER = content[14].Trim(),
               UNIQUE_KEY = content[15].Trim(),
            };
         }
         catch(Exception ex) {
            Console.WriteLine($"{ex.Message} |  Record With Error : {content[0]}");
            throw new NotImplementedException(ex.Message);
         }
      }
   }
}