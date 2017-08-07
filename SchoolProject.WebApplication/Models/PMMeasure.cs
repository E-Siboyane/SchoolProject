﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolProject.WebApplication.Models.Abstract;

namespace SchoolProject.WebApplication.Models {
    public class PMMeasure : Audit {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PMMeasureId { get; set; }
        [Required, ForeignKey("PMObjective")]
        public long PMObjectiveId { get; set; }        
        public virtual PMObjective PMObjective { get; set; }
        [Required]
        public string MeasureName { get; set; }
        [StringLength(150)]
        public string SourceOfInformation { get; set; }
        [Required, StringLength(150)]
        public string SubjectMatterExpert { get; set; }
        [Required, ForeignKey("Term")]
        public int TermId { get; set; }
        public virtual AdminTerm Term { get; set; }
        [Required, Range(0.01, 100.00, ErrorMessage ="Weight should be between 0.01 to 100")]
        public decimal MeasureWeight { get; set; }
        [Range(0.01, 100.00, ErrorMessage = "Weight should be between 0.01 to 100")]
        public decimal EmployeeScore { get; set; }
        [Range(0.01, 100.00, ErrorMessage = "Weight should be between 0.01 to 100")]
        public decimal LineManagerScore { get; set; }
        [Range(0.01, 100.00, ErrorMessage = "Weight should be between 0.01 to 100")]
        public decimal AuditScore { get; set; }
        public string EmployeeComments { get; set; }
        public string LineManagerComments { get; set; }
        public string AuditComments { get; set; }
        [Required, ForeignKey("Status")]
        public int StatusId { get; set; }
        public virtual AdminStatus Status { get; set; }
    }
}
