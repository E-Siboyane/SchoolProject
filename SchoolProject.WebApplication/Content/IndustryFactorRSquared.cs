using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
    public class IndustryFactorRSquared {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string CorrelationModelEstimateName { get; set; }
        public string IndustryFactorName { get; set; }
        public decimal industryFactorRSquared { get; set; }
    }
}
