using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolProject.WebApplication.Models.Abstract {
    public abstract class Audit {
        [Required]
        public DateTime DateCreated { get; set; }
        [Required, StringLength(100)]
        public string CreatedBy { get; set; }
        public DateTime? DateModified { get; set; }
        [StringLength(100)]
        public string ModifiedBy { get; set; }
        public DateTime? DateDeleted { get; set; }
        [StringLength(100)]
        public string DeletedBy { get; set; }
    }
}
