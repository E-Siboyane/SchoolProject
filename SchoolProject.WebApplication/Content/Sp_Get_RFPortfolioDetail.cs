using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DataAccess {
    public class Sp_Get_RFPortfolioDetail {
        public string PortfolioName { get; set; }
        public string InstrumentId { get; set; }
        public double HoldingAmount { get; set; }
        public decimal? NumShare { get; set; }       
    }
}
