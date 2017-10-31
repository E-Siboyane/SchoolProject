using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolProject.WebApplication.ViewModels {
    public class ScoreReviewViewModel {
        [Required]
        public int PerformanceReviewId { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string ManagerUsername { get; set; }
        [Required]
        public ReviewProcessStatus CurrentReviewStatus { get; set; }
        [Required]
        [Range(1, 100, ErrorMessage = "Measure Score Must be between 1 to 100")]
        public decimal Score { get; set; }
        [Required]
        public string StrategicGoal { get; set; }
        [Required]
        public string Objective { get; set; }
        [Required]
        public long MeasureId { get; set; }
        [Required]
        public string MeasureName { get; set; }
        [Required]
        public decimal MeasureWeight { get; set; }
        [Required]
        public string CaptureComments { get; set; }
        [Required]
        public bool IsLineManager { get; set; }
        public decimal MaximumScore { get; set; }
        public List<PerformanceReviewScoringContent> ReviewContents { get; set; }
        public string ProcessingStatusMessage { get; set; }
        public bool ProcessingStatus { get; set; }
    }
}