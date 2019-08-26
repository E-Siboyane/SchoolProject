using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
    public class AnalysisJobReport {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int PortfolioId { get; set; }
        public string PortfolioName { get; set; }
        public DateTime AsOfDate { get; set; }
        public bool IsReportGenerated { get; set; }
        public string ConfidenceLevel { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
