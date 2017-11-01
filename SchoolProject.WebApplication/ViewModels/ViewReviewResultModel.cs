using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolProject.WebApplication.ViewModels {
    public class ViewReviewResultModel {
        public string Username { get; set; }
        public decimal AverageReviewScore { get; set; }
        public string ReviewRating { get; set; }
        public List<PerformanceReviewScoringContent> ReviewMeasures { get; set; }
    }
}