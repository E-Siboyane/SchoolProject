using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
    public class AnalysisJobsDueForProcessing {
        public int AnalysisJobManagerId { get; set; }
        public int JobId { get; set; }
        public string PortfolioName { get; set; }
        public string HoldingDate { get; set; }
        public string AnalysisDate { get; set; }
        public string AnalysisSettings { get; set; }
        public string DataSettings { get; set; }
    }
}
