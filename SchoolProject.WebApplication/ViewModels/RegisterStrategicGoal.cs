using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolProject.WebApplication.ViewModels {
    public class RegisterStrategicGoal {
        public int StrategicGoalId { get; set; }
        [Required]
        public string StrategicGoalCode { get; set; }
        [Required]
        public string StrategicGoalName { get; set; }
        [Required]
        public FormModeOption FormMode { get; set; }
        public string ProcessingStatusMessage { get; set; }
        public bool ProcessingStatus { get; set; }
    }
}