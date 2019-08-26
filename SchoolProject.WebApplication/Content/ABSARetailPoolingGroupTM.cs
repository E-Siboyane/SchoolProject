using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class ABSARetailPoolingGroupTM {
      public int PDRowLabelId { get; set; }
      public int LGDRowLabelId { get; set; }
      public string UniqueSegmentPDLGD { get; set; }
      public int SegmentRowId { get; set; }
      public int ClusterRowId { get; set; }
   }
}
