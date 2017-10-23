using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SchoolProject.WebApplication.Models;
using SchoolProject.WebApplication.ViewModels;
using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Net;

namespace SchoolProject.WebApplication.Controllers
{   
    public class AccountController : Controller
    {
        private ApplicationDatabaseContext _dbContext;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public AccountController() {
            _dbContext = new ApplicationDatabaseContext();
        }

        public ApplicationSignInManager SignInManager {
            get {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager {
            get {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set {
                _userManager = value;
            }
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager) {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        [HttpGet]
        public ActionResult Login() {
            var loginModel = new LoginViewModel();
            loginModel.Username = "E947470";
            return View(loginModel);
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel loginModel) {
            if (!ModelState.IsValid) {
                return View(loginModel);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(loginModel.Username, loginModel.Password, loginModel.RememberMe, shouldLockout: false);
            switch (result) {
                case SignInStatus.Success:
                    return RedirectToAction("");
                case SignInStatus.LockedOut:
                    return View("Lockout");
               case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(loginModel);
            }
        }

        [HttpGet]
        public ActionResult ManageUsers()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_dbContext));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_dbContext));
            var employes = _dbContext.StructureEmployee.Where(x => x.DateDeleted == null).ToList();
            var users = UserManager.Users.ToList();
            var results = users.Join(employes, u => u.UserName, e => e.NetworkUsername, (usersList, employeesList) => new { usersList, employeesList }).ToList();
            return View();
        }

        [HttpGet]
        public ActionResult RegisterUser() {
            if (TempData["viewModelRegisterUser"] != null) {
                return View((RegisterUserViewModel)TempData["viewModelRegisterUser"]);
            }
            else {
                var registerUserModel = new RegisterUserViewModel() {
                    Employees = GetEmployees()
                };
                return View(registerUserModel);
            }
        }

        [HttpPost]
        public async Task<ActionResult> RegisterUser(RegisterUserViewModel registerUserViewModel) {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_dbContext));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_dbContext));
            if (ModelState.IsValid) {
                var user = new ApplicationUser() {
                    UserName = registerUserViewModel.Username,
                    Email = registerUserViewModel.EmailAddress,
                    EmailConfirmed = true
                };
                var createUser = await UserManager.CreateAsync(user, registerUserViewModel.Password);
                if (createUser.Succeeded) {
                    var registerNewUserModel = new RegisterUserViewModel() {
                        Employees = GetEmployees(),
                        ProcessingStatusMessage = string.Format("Successfully added account : {0}", GetEmployees().FirstOrDefault(x => string.Compare( x.ValueText,registerUserViewModel.Username,true) == 0).DisplayText),
                        ProcessingStatus = true
                    };
                    TempData["viewModelRegisterUser"] = registerNewUserModel;
                    return RedirectToAction("RegisterUser");
                }
                else {

                }
            }
            return View(registerUserViewModel);
        }

        public List<SelectionOptions> GetEmployees() {
            var employees = new List<SelectionOptions>();
            var retrieveInfo = _dbContext.StructureEmployee.Where(x => x.StatusId == 1 && x.DateDeleted == null);
            retrieveInfo.ToList().ForEach(user => {
                employees.Add(new SelectionOptions() {
                    DisplayText = string.Format("{0} {1}", user.Name, user.Surname),
                    ValueText = user.NetworkUsername
                });
            });
            return (employees);
        }
    }
}