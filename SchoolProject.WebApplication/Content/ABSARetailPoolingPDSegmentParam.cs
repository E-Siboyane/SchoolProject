using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class ABSARetailPoolingPDSegmentParam {
      public DbContextType DbContextType { get; set; }
      public int ClusterRowId { get; set; }
      public int SegmentRowId { get; set; }
      public string Segment { get; set; }
      public RetailPoolingFileType RetailPoolingFileType { get; set; }
      public Guid RowId { get; set; }
   }
}
