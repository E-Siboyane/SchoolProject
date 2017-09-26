using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolProject.WebApplication.Models.Abstract;
using SchoolProject.WebApplication.Models.Interface;

namespace SchoolProject.WebApplication.Models {
    public class PMObjective : Audit, IStatus {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PMObjectiveId { get; set; }
        [Required, ForeignKey("PMStrategicGoal")]
        public int PMStrategicGoalId { get; set; }
        public virtual PMStrategicGoal PMStrategicGoal { get; set; }
        public string ObjectiveName { get; set; }
        [Range(0.00, 100.00, ErrorMessage = "Weight should range from 0.00 to 100.00")]
        public decimal ObjectiveWeight { get; set; }
        [Required, ForeignKey("Status")]
        public int StatusId { get; set; }
        public virtual AdminStatus Status { get; set; }
    }
}
