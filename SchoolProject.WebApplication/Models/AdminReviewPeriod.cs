using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolProject.WebApplication.Models.Abstract;
using SchoolProject.WebApplication.Models.Interface;

namespace SchoolProject.WebApplication.Models {
    public class AdminReviewPeriod : Audit, IStatus {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewPeriodId { get; set; }
        [Required, StringLength(100)]
        public string ReviewPeriodName { get; set; }
        [Required, StringLength(50)]
        public string ReviewPeriodCode { get; set; }
        [Required, ForeignKey("Status")]
        public int StatusId { get; set; }
        public virtual AdminStatus Status{ get; set; }
    }
}
