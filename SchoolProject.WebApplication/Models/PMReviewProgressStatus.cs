using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolProject.WebApplication.Models.Abstract;
using SchoolProject.WebApplication.Models.Interface;

namespace SchoolProject.WebApplication.Models {
    public class PMReviewProgressStatus : Audit, IStatus {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PMReviewProgressStatusId { get; set; }
        [Required, ForeignKey("PMReview")]
        public int PMReviewId { get; set; }
        public virtual PMReview PMReview { get; set; }
        [Required, ForeignKey("ProcessStage")]
        public int ProcessStageId { get; set; }
        public virtual PMProcessStage ProcessStage { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Required, ForeignKey("Status")]
        public int StatusId { get; set; }
        public virtual AdminStatus Status { get; set; }
    }
}
