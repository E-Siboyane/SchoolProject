using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SchoolProject.WebApplication.Models;

namespace SchoolProject.WebApplication.Controllers
{
    public class _BaseController : Controller
    {
        public string FullName { get; private set; }
        public string Role { get; set; }
        private readonly ApplicationDatabaseContext _dbContext;

        public _BaseController() {
            _dbContext = new ApplicationDatabaseContext();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext) {
            var user = _dbContext.Users.Include(r => r.Roles).FirstOrDefault(X => string.Compare(X.UserName, 
                                                                                  User.Identity.Name, true) == 0);
            var employee = _dbContext.StructureEmployee.FirstOrDefault(X => string.Compare(X.NetworkUsername,
                                                                                User.Identity.Name, true) == 0);
            if (user != null) {
                if (user.Roles.Count > 0) {
                    var roleId = user.Roles.FirstOrDefault().RoleId;
                    Role = _dbContext.Roles.FirstOrDefault(x => string.Compare(x.Id, roleId, true) == 0).Name;
                }
                else {
                    Role = "Employee";
                }
            }

            if (employee != null) {
                FullName = string.Format("{0} {1}", employee.Name, employee.Surname);
            }

            base.OnActionExecuting(filterContext);
        }


    }
}