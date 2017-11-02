using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolProject.WebApplication.ViewModels {
    public class AllReviewModelView {
        public int PerformanceReviewId { get; set; }
        public string ReviewName { get; set; }
        public string EmployeeName { get; set; }
        public string Manager { get; set; }
        public string Department { get; set; }
        public string ReviewStatus { get; set; }
        public string Rating { get; set; }
        public decimal AverageScore { get; set; }
    }
}