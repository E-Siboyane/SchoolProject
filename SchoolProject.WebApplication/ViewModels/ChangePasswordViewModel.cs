using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolProject.WebApplication.ViewModels {
    public class ChangePasswordViewModel {
        [Required]
        public string FullName { get; set; }
        public string Role { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        public string ProcessingStatusMessage { get; set; }
        public bool ProcessingStatus { get; set; }
    }
}