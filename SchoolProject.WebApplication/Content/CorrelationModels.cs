using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
    public class CorrelationModels : ICorrelation {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string FactorSetName { get; set; }
        public string CorrelationModelTypeName { get; set; }
        public string CorrelationModelEstimateName { get; set; }
    }
}
