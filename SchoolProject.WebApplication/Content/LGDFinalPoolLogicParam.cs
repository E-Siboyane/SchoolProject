using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class LGDFinalPoolLogicParam {
      public RetailPoolingFileType ProductName { get; set; }
      public DbContextType DbContextType { get; set; }
      public int LGDBucketNumber { get; set; }
      public double ConstantFirstPercentage { get; set; }
   }
}
