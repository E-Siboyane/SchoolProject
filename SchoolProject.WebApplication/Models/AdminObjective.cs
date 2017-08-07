using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolProject.WebApplication.Models.Abstract;
using SchoolProject.WebApplication.Models.Interface;

namespace SchoolProject.WebApplication.Models {
    public class AdminObjective : Audit, IStatus {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ObjectivesId { get; set; }
        [Required]
        public string ObjectiveName { get; set; }
        [Required]
        public decimal ObjectiveOverallWeight { get; set; }
        [Required, ForeignKey("Status")]
        public int StatusId { get; set; }
        public virtual AdminStatus Status { get; set; }
    }
}
