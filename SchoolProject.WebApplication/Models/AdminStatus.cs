using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolProject.WebApplication.Models.Abstract;

namespace SchoolProject.WebApplication.Models {
    public class AdminStatus : Audit {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StatusId { get; set; }
        [Required, StringLength(50)]
        public string StatusName { get; set; }
        [Required, StringLength(50)]
        public string StatusCode { get; set; }
    }
}
