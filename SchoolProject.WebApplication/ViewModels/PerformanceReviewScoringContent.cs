using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolProject.WebApplication.ViewModels {
    public class PerformanceReviewScoringContent {
        public int StrategicGoalID { get; set; }
        public string StrategicGoalName { get; set; }
        public string ObjectiveName { get; set; }
        public long MeasureId { get; set; }
        public string MeasureName { get; set; }
        public decimal MeasureWeight { get; set; }
        public decimal EmployeeScore { get; set; }
        public decimal ManagerScore { get; set; }
        public string EmployeeComments { get; set; }
        public string ManagerComments { get; set; }
    }
}