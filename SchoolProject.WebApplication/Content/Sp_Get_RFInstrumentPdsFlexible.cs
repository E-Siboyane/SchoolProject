using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DataAccess {
    public class Sp_Get_RFInstrumentPdsFlexible {
        public string InstrumentId { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Term { get; set; }
        public decimal Pd { get; set; }
    }
}
