using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolProject.WebApplication.Models.Abstract;
using SchoolProject.WebApplication.Models.Interface;

namespace SchoolProject.WebApplication.Models {
    public class PMReview : Audit, IStatus {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PMReviewId { get; set; }
        [ForeignKey("ReportingStructure")]
        public int ReviewReportingStructureId { get; set; }
        public virtual PMReviewReportingStructure ReportingStructure { get; set; }
        [Required,ForeignKey("PMReviewPeriod")]
        public int PMReviewPeriodId { get; set; }
        public virtual PMReviewPeriod PMReviewPeriod { get; set; }
        public string OverallEmployeeComments { get; set; }
        public string OverallLineManagerComments { get; set; }
        public string OverallAuditComments { get; set; }
        [Required, ForeignKey("Status")]
        public int StatusId { get; set; }
        public virtual AdminStatus Status { get; set; }
    }
}
