using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolProject.WebApplication.DTO {
    public class ReportingStructure {
        public int EmployeeRecordId { get; set; }
        public string EmployeeCode { get; set; }
        public string ManagerEployeeCode { get; set; }
        public string JobGradeName { get; set; }
        public string EmployeeName { get; set; }
        public string ManagerName { get; set; }
        public string DocumentTypeName { get; set; }
        public string StatusName { get; set; }
    }
}