using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class ABSARetailProcessingProgressDetail {
      public int Id { get; set; }
      public Guid RowId { get; set; }
      public string Status { get; set; }
      public string Message { get; set; }
      public DateTime CreatedDate { get; set; }
   }
}
