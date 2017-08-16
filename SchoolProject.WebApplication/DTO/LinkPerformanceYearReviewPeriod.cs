using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolProject.WebApplication.DTO {
    public class LinkPerformanceYearReviewPeriod {
        public int PMReviewPeriodId { get; set; }
        public int ReviewPeriodId { get; set; }
        public int PerformanceYearId { get; set; }
        public int StatusId { get; set; }
        public string ReviewPeriod { get; set; }
        public string PerformanceYear { get; set; }
        public string PerformanceYearStartEndDate { get; set; }
        public string StatusDescription { get; set; }
    }
}
