using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class ABSAPoolingLookup {
      [Key]
      public int Id { get; set; }
      public string Product { get; set; }
      public int PD { get; set; }
      public int LGD { get; set; }
      public int Tenor { get; set; }
      public int FilterMinimum { get; set; }
      public int FilterMaximum { get; set; }
      public DateTime CreatedDate { get; set; }
      public double PDTolerance { get; set; }
      public double LGDTolerance { get; set; }
      public double TenorTolerance { get; set; }
   }
}
