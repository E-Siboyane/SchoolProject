using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
   public class ABSARetailPoolingBulkUpdate {
      public int Id { get; set; }
      public double Ratio { get; set; }
      public int InitialRank { get; set; }
      public int FinalPool { get; set; }
      public float OriginalAveragePerPool { get; set; }
      public double AveragePerPool { get; set; }
      public Guid RowId { get; set; }
      public string Product { get; set; }

   }
}
