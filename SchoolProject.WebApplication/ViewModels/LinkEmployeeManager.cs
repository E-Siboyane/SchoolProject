using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolProject.WebApplication.ViewModels {
    public class LinkEmployeeManager {
        public int EmployeeReportingRecordId { get; set; }
        [Required]
        public int EmployeeRecordId { get; set; }
        [Required]
        public int ManagerRecordId { get; set; }
        [Required]
        public int DocumentTypeId { get; set; }
        [Required]
        public FormModeOption FormMode { get; set; }
        public string ProcessingStatusMessage { get; set; }
        public bool ProcessingStatus { get; set; }
        public List<SelectionOptions> Employees { get; set; }
        public List<SelectionOptions> Managers { get; set; }
        public List<SelectionOptions> DocumentTypes { get; set; }
    }
}