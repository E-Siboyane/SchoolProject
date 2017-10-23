using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolProject.WebApplication.ViewModels {
    public class RegisterUserViewModel {
        [Required]
        [DisplayName("Email Address")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        [Required]
        [DisplayName("Employee Code")]
        public string Username { get; set; }
        public List<SelectionOptions> Employees { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string ProcessingStatusMessage { get; set; }
        public bool ProcessingStatus { get; set; }
    }
}