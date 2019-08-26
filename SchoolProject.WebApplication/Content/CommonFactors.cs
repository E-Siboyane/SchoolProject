using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
    public class CommonFactors : ICorrelation {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string FactorSetName { get; set; }
        public string CommonFactorName { get; set; }
    }
}
