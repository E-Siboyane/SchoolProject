using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class ParamPDRatioInitialRank {
      public RetailPoolingFileType ProductName { get; set; }
      public DbContextType DbContextType { get; set; }
      public int TotalCount { get; set; }
      public int ClusterRowId { get; set; }
      public int SegmentRowId { get; set; }
      public string Segment { get; set; }
   }
}
