using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class ABSARetailPoolingLGDSegmentParam {
      public DbContextType DbContextType { get; set; }
      public int PdRowLebelIs { get; set; }
      public string UniqueSegment { get; set; }
      public int SegmentRowId { get; set; }
      public int ClusterId { get; set; }
      public RetailPoolingFileType RetailPoolingFileType { get; set; }
      public Guid RowId { get; set; }
   }
}
