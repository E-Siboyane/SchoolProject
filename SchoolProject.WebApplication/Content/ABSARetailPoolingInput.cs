using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class ABSARetailPoolingInput {
      [Key]
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
   }
}
