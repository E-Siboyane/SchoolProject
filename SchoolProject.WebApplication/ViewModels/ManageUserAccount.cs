using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolProject.WebApplication.ViewModels {
    public class ManageUserAccount {
        public string EmployeeCode { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}