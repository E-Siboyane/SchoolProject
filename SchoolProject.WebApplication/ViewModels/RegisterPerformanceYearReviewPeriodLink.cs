using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolProject.WebApplication.ViewModels {
    public class RegisterPerformanceYearReviewPeriodLink {
        public int PMReviewPeriodId { get; set; }
        [Required]
        public int PerformanceYearId { get; set; }
        [Required]
        public int ReviewPeriodId { get; set; }
        public FormModeOption FormMode { get; set; }
        public string ProcessingStatusMessage { get; set; }
        public bool ProcessingStatus { get; set; }
        public List<SelectionOptions> PerformanceYears { get; set; }
        public List<SelectionOptions> ReviewPeriods { get; set; }
    }
}