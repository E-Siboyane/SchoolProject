using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolProject.WebApplication.ViewModels {
    public class PerformanceReeviewContent {
        public int StrategicGoalID { get; set; }
        public string StrategicGoalName { get; set; }
        public string ObjectiveName { get; set; }
        public long MeasureId { get; set; }
        public string MeasureName { get; set; }
        public decimal MeasureWeight { get; set; }
    }
}