using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolProject.WebApplication.Models.Abstract;
using SchoolProject.WebApplication.Models.Interface;

namespace SchoolProject.WebApplication.Models {
    public class PMReviewPeriod : Audit, IStatus {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PMReviewPeriodId { get; set; }
        [Required, ForeignKey("ReviewPeriod")]
        public int ReviewPeriodId { get; set; }
        public virtual AdminReviewPeriod ReviewPeriod { get; set; }
        [Required, ForeignKey("PerformanceYear")]
        public int PerformanceYearId { get; set; }
        public virtual AdminPerformanceYear PerformanceYear { get; set; }
        [Required, ForeignKey("Status")]
        public int StatusId { get; set; }
        public virtual AdminStatus Status { get; set; }
    }
}
