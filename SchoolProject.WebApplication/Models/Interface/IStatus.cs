using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProject.WebApplication.Models.Interface {
    public interface IStatus {
        int StatusId { get; set; }
        AdminStatus Status { get; set; }
    }
}
