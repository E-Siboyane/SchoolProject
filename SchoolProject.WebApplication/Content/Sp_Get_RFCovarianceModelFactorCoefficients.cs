using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DataAccess {
    public class Sp_Get_RFCovarianceModelFactorCoefficients {
        public string FactorSetName { get; set; }
        public DateTime WeightDate { get; set; }
        public string CounterpartyId { get; set; }
        public string FactorName { get; set; }
        public decimal FactorCoefficient { get; set; }
    }
}
