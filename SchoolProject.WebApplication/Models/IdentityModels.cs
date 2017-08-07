using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SchoolProject.WebApplication.Models {
    public class IdentityModels {
    }
    public class ApplicationUser : IdentityUser {
        //You can extend this class by adding additional fields like Birthday
    }
}
