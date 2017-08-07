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
        [Required, Range(0.01, 100.00, ErrorMessage = "Weight should be between 0.01 to 100")]
        public decimal ObjectiveWeight { get; set; }
        [Required, ForeignKey("Status")]
        public int StatusId { get; set; }
        public virtual AdminStatus Status { get; set; }
    }
}
