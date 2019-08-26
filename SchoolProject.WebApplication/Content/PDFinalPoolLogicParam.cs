using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class PDFinalPoolLogicParam {
      public RetailPoolingFileType ProductName { get; set; }
      public DbContextType DbContextType { get; set; }
      public int PdBucketNumber { get; set; }
      public double ConstantFirstPercentage { get; set; }
      public double ConstantSecondPercentage { get; set; }
   }
}
