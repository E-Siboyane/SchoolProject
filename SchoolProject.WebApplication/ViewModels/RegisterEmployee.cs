using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolProject.WebApplication.ViewModels {
    public class RegisterEmployee {
        public int EmployeeRecordId { get; set; }
        [Required]
        public string EmployeeCode { get; set; }
        [Required]
        public string EmployeeName { get; set; }
        [Required]
        public string EmployeeSurname { get; set; }
        [Required]
        public string NetworkUsername { get; set; }
        [Required]
        public int TeamId { get; set; }
        [Required]
        public int JobGradeId { get; set; }
        [Required]
        public FormModeOption FormMode { get; set; }
        public string ProcessingStatusMessage { get; set; }
        public bool ProcessingStatus { get; set; }
        public List<SelectionOptions> Team { get; set; }
        public List<SelectionOptions> JobGrade { get; set; }
    }
}