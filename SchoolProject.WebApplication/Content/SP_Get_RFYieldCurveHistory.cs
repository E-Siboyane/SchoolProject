using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DataAccess {
    public class SP_Get_RFYieldCurveHistory {
        public DateTime CurveDate { get; set; }
        public string YieldCurve { get; set; }
        public decimal Tenor { get; set; }
        public decimal Rate { get; set; }
    }
}
