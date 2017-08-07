using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProject.WebApplication.Models.Interface {
    public interface IAudit {
        [Required]
        DateTime DateCreated { get; set; }
        [Required, StringLength(100)]
        string CreatedBy { get; set; }
        DateTime? DateModified { get; set; }
        [StringLength(100)]
        string ModifiedBy { get; set; }
        DateTime? DateDeleted { get; set; }
        [StringLength(100)]
        string DeletedBy { get; set; }
    }
}
