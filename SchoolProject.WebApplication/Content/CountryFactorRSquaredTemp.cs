using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
    public class CountryFactorRSquared {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string CorrelationModelEstimateName { get; set; }
        public string countryFactorName { get; set; }
        public decimal countryFactorRSquared { get; set; }
    }
}
