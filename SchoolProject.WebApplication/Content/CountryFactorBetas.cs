using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
    public class CountryFactorBetas: ICorrelation {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string CorrelationModelEstimateName { get; set; }
        public string CountryFactorName { get; set; }
        public string CommonFactorName { get; set; }
        public decimal Beta { get; set; }
    }
}
