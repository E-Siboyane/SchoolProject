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
    [Table("Employee")]
    public class StructureEmployee : Audit, IStatus {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeRecordId { get; set; }
        [Required]
        public string EmployeeCode { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string NetworkUsername { get; set; }
        [ForeignKey("Team")]
        public int TeamId { get; set; }
        public virtual StructureTeam Team { get; set; }
        [ForeignKey("Status"), Required]
        public int StatusId { get; set; }
        public virtual AdminStatus Status { get; set; }
    }
}