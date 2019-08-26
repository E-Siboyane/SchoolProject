using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class ABSARetailPoolingConfigType {
      [Key]
      public int ABSARetailPoolingConfigTypeId { get; set; }
      public string Name { get; set; }
      public DateTime CreatedDate { get; set; }
      public string Code { get; set; }
   }
}
