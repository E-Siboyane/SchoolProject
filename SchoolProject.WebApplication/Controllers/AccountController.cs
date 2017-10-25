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
using System.Data.Entity;

namespace SchoolProject.WebApplication.Controllers
{   
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationDatabaseContext _dbContext;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public AccountController() {
            _dbContext = new ApplicationDatabaseContext();
        }

        [AllowAnonymous]
        public ActionResult AccessDenied() {
            return View();
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
        [AllowAnonymous]
        public ActionResult Login(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;
            var loginModel = new LoginViewModel();
            return View(loginModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel loginModel, string returnUrl) {
            if (!ModelState.IsValid) {
                return View(loginModel);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(loginModel.Username, loginModel.Password, loginModel.RememberMe, shouldLockout: false);
            switch (result) {
                case SignInStatus.Success:
                    ViewBag.FullName = "Elias Seboyane";
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
               case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(loginModel);
            }
        }

        [HttpGet]
        public ActionResult LogOff() {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        private IAuthenticationManager AuthenticationManager {
            get {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private ActionResult RedirectToLocal(string returnUrl) {
            if (Url.IsLocalUrl(returnUrl)) {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult ManageUsers()
        {
            ViewBag.Message = null;
            ViewBag.MessageType = false;
            if (TempData["Message"] != null)
                ViewBag.Message = (string)TempData["Message"];
            if (TempData["MessageType"] != null)
                ViewBag.MessageType = (bool)ViewBag.MessageType;

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_dbContext));
            var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
            role.Name = "Admin";
            if (!roleManager.RoleExists(role.Name)) {
                roleManager.Create(role);
           }
            //var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_dbContext));
            var userAccounts = GetEmployeeLoginDetails();
            return View(userAccounts);
        }

        [HttpGet]
        public ActionResult AssignUserAdminRole(string userId) {
            var fullName = GetEmployeeLoginDetails().FirstOrDefault(x => string.Compare(x.UserId, userId, false) == 0).FullName;
            var assignRole = UserManager.AddToRole(userId, "Admin");
            if (assignRole.Succeeded) {
                 TempData["Message"] = string.Format("Successfully Assigned {0} to Admin role", fullName);
                TempData["MessageType"] = true;
            }
            else {
                var row = 1;
                var errorMessage = string.Empty;
                foreach (var error in assignRole.Errors) {
                    if (assignRole.Errors.Count() > 1) {
                        if (row == 1)
                            errorMessage = errorMessage + error;
                        else
                            errorMessage = errorMessage + "<br>" + error;
                    }
                    else {
                        errorMessage = errorMessage + error;
                    }
                    row++;
                }
                ViewBag.Message = errorMessage;
                ViewBag.MessageType = false;
            }
            return RedirectToAction("ManageUsers");

        }

        public List<ManageUserAccount> GetEmployeeLoginDetails() {
            var userAccounts = new List<ManageUserAccount>();
            var employes = _dbContext.StructureEmployee.Where(x => x.DateDeleted == null).ToList();
            var users = UserManager.Users.Include(x => x.Roles).ToList();
            var results = users.Join(employes, u => u.UserName, e => e.NetworkUsername, (usersList, employeesList) => new { usersList, employeesList }).ToList();
            foreach (var user in results) {
                userAccounts.Add(new ManageUserAccount() {
                    Username = user.usersList.UserName,
                    UserId = user.usersList.Id,
                    Email = user.usersList.Email,
                    FullName = string.Format("{0} - {1} {2}", user.employeesList.EmployeeCode,user.employeesList.Surname, user.employeesList.Name),
                    EmployeeCode = user.employeesList.EmployeeCode,
                    Role = user.usersList.Roles.Count > 0 ? GetUserRole(user.usersList.Roles.FirstOrDefault().RoleId) : "Employee"
                });
            }
            return (userAccounts);
        }

        public string GetUserRole(string roleId) {
            var role = _dbContext.Roles.FirstOrDefault(x => string.Compare(x.Id, roleId, true) == 0);
            if (role != null)
                return role.Name;
            return string.Empty;
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
            // var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_dbContext));
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
                        ProcessingStatusMessage = string.Format("Successfully added account : {0}", GetEmployees().FirstOrDefault(x => string.Compare(x.ValueText, registerUserViewModel.Username, true) == 0).DisplayText),
                        ProcessingStatus = true
                    };
                    TempData["viewModelRegisterUser"] = registerNewUserModel;
                    return RedirectToAction("RegisterUser");
                }
                else {
                    registerUserViewModel.ProcessingStatus = false;
                    var row = 1;
                    foreach (var error in createUser.Errors) {
                        if (createUser.Errors.Count() > 1) {
                            if (row == 1)
                            registerUserViewModel.ProcessingStatusMessage = registerUserViewModel.ProcessingStatusMessage + error;
                            else
                                registerUserViewModel.ProcessingStatusMessage = registerUserViewModel.ProcessingStatusMessage + "<br>" + error;
                        }
                        else {
                            registerUserViewModel.ProcessingStatusMessage = registerUserViewModel.ProcessingStatusMessage + error;
                        }
                        row++;
                    }
                }
            }
            return View(registerUserViewModel);
        }

        [HttpGet]
        public ActionResult ChangePassword(string userId) {
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("ManageUsers");
            var userDetails = GetEmployeeLoginDetails().FirstOrDefault(x => string.Compare(x.UserId, userId, false) == 0);

            if (TempData["AddedUser"] != null) {
                return View((ChangePasswordViewModel)TempData["AddedUser"]);
            }
            else {
                TempData["AddedUser"] = null;
                var changePasswordModel = new ChangePasswordViewModel() {
                    UserId = userId,
                    FullName = userDetails.FullName
                };
                return View(changePasswordModel);
            }
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel changePasswordModel) {
            if (ModelState.IsValid) {
                var passordResetToken = await UserManager.GeneratePasswordResetTokenAsync(changePasswordModel.UserId);
                var result = await UserManager.ResetPasswordAsync(changePasswordModel.UserId, passordResetToken,changePasswordModel.Password);
                if (result.Succeeded) {
                    changePasswordModel.ProcessingStatus = true;
                    changePasswordModel.Password = string.Empty;
                    changePasswordModel.ProcessingStatusMessage = string.Format("Successfully Changed Password for {0}", changePasswordModel.FullName);
                    TempData["AddedUser"] = changePasswordModel;
                    return RedirectToAction("ChangePassword", new { userId = changePasswordModel.UserId });
                }
                else {
                    changePasswordModel.ProcessingStatus = false;
                    var row = 1;
                    foreach (var error in result.Errors) {
                        if (result.Errors.Count() > 1) {
                            if (row == 1)
                                changePasswordModel.ProcessingStatusMessage = changePasswordModel.ProcessingStatusMessage + error;
                            else
                                changePasswordModel.ProcessingStatusMessage = changePasswordModel.ProcessingStatusMessage + "<br>" + error;
                        }
                        else {
                            changePasswordModel.ProcessingStatusMessage = changePasswordModel.ProcessingStatusMessage + error;
                        }
                        row++;
                    }
                }
            }
            return View(changePasswordModel);
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