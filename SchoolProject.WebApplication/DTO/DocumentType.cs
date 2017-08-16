using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolProject.WebApplication.DTO {
    public class DocumentType {
        public int DocumentTypeId { get; set; }
        public string DocumentTypeCode { get; set; }
        public string DocumentTypeName { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public DateTime? DeleteDate { get; set; }
    }
}
