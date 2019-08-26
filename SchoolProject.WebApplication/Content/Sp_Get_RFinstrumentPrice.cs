using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DataAccess {
    public class Sp_Get_RFInstrumentPrice {
        public string InstrumentId { get; set; }
        public DateTime PriceDate { get; set; }
        public decimal AnalysisDatePrice { get; set; }
        public decimal? Horizon { get; set; }
        public decimal? HorizonPrice { get; set; }
    }
}
