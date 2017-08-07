using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolProject.WebApplication.Models.Abstract;
using SchoolProject.WebApplication.Models.Interface;

namespace SchoolProject.WebApplication.Models {
    [Table("Team")]
    public class StructureTeam : Audit, IStatus {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TeamId { get; set; }
        [Required, StringLength(50)]
        public string TeamCode { get; set; }
        [Required, StringLength(256)]       
        public string TeamName{ get; set; }
        [Required, ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public virtual StructureDepartment Department { get; set; }
        [Required, ForeignKey("Status")]
        public int StatusId { get; set; }
        public virtual AdminStatus Status { get; set; }
    }
}
