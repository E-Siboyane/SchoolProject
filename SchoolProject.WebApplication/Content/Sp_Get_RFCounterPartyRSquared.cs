using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DataAccess {
   public  class Sp_Get_RFCounterPartyRSquared {
       public string CounterPartyId { get; set; }
       public string CorrelationModelEstimateName { get; set; }
       public decimal RSquared { get; set; }
    }
}
