using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolProject.WebApplication.Models.Abstract;
using SchoolProject.WebApplication.Models.Interface;

namespace SchoolProject.WebApplication.Models {
    public class PMStrategicGoal : Audit, IStatus {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PMStrategicGoalId { get; set; }
        [ForeignKey("PMReview")]
        public int PMReviewId { get; set; }
        public virtual PMReview PMReview { get; set; }
        [Required, ForeignKey("StrategicGoal")]
        public int StrategicGoalId { get; set; }
        public virtual AdminStrategicGoal StrategicGoal { get; set; }
        public decimal? StrategicGoalWeight { get; set; }
        [Required, ForeignKey("Status")]
        public int StatusId { get; set; }
        public virtual AdminStatus Status { get; set; }
    }
}
