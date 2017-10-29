using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolProject.WebApplication.DTO {
    public class ManagePerformanceReview {
        public long PerformancereviewPeriodStageId { get; set; }
        public int ProcessStageId { get; set; }
        public string ReviewStageName { get; set; }
        public int PMReviewId { get; set; }
        public string ReviewPeriodName { get; set; }
        public int DocumentOwnerId { get; set; }
        public int LineManagerId { get; set; }
        public string LineManagerName { get; set; }
        public string EmployeeName { get; set; }
        public string NetworkUsername { get; set; }
        public string UserId { get; set; }
        public string DocumentType { get; set; }
    }
}