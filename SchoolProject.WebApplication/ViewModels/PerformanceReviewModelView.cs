using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolProject.WebApplication.ViewModels {
    public class PerformanceReviewModelView {
        [Required]
        public int ReviewPeriodId { get; set; }
        public List<SelectionOptions> ReviewPeriods { get; set; }
        public int ReportingStructureId { get; set; }
        public string ProcessingStatusMessage { get; set; }
        public bool ProcessingStatus { get; set; }
    }
}