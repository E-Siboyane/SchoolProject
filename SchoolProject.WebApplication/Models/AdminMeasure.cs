using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolProject.WebApplication.Models.Abstract;
using SchoolProject.WebApplication.Models.Interface;

namespace SchoolProject.WebApplication.Models {
    public class AdminMeasure : Audit, IStatus {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long MeasureId { get; set; }
        [Required]
        public string MeasureName { get; set; }
        [Required]
        public decimal DefaultWeight { get; set; }
        [Required]
        public string DefaultSourceOfInformation { get; set; }
        [Required]
        public string DefaultSubjectMatterExpert { get; set; }
        [ForeignKey("Term"), Required]        
        public int TermId { get; set; }
        public virtual AdminTerm Term { get; set; }
        [ForeignKey("Status"), Required]
        public int StatusId { get; set; }
        public virtual AdminStatus Status { get; set; }
    }
}
