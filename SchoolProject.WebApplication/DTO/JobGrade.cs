using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolProject.WebApplication.DTO {
    public class JobGrade {
        public int JodGradeId { get; set; }
        public string JobGradeName { get; set; }
        public string JobGradeCode { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public DateTime? DeleteDate { get; set; }
    }
}
