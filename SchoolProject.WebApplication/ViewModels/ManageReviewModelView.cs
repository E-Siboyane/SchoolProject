using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SchoolProject.WebApplication.DTO;

namespace SchoolProject.WebApplication.ViewModels {
    public class ManageReviewModelView {

        public string UserRole { get; set; }
        public List<ManagePerformanceReview> EmployeeReviewPeriods { get; set; }
        public List<ManagePerformanceReview> DirectReportReviews  { get; set; }
    }
}