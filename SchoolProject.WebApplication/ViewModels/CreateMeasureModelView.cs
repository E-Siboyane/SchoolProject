using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolProject.WebApplication.ViewModels {
    public class CreateMeasureModelView {
        [Required]
        public string Username { get; set; }
        [Required]
        public string ManagerUsername { get; set; }
        [Required]
        public ReviewProcessStatus CurrentReviewStatus { get; set; }
        [Required]
        public int PerformanceReviewId { get; set; }
        [Required]
        public int StrategicGoalId { get; set; }
        [Required]
        public string ObjectiveName { get; set; }
        public long MeasureId { get; set; }
        [Required]
        public string MeasureName { get; set; }
        [Required]
        [Range(1, 100, ErrorMessage = "Measure Weight Must be between 1 to 100")]
        public decimal MeasureWeight { get; set; }
        [Required]
        public List<SelectionOptions> StrategicGoals { get; set; }
        public FormModeOption FormProcessingMode { get; set; }
        public string ProcessingStatusMessage { get; set; }
        public bool ProcessingStatus { get; set; }
        public List<PerformanceReeviewContent> CreatedMeasures { get; set; }
    }
}