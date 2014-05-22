using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Core;
using Core.Configuration;
using Core.Domains;
using Core.Helpers;
using Core.Models;
using Service.Interfaces;
using Web.Infrastructure;
using Core.Helpers.Security;

namespace Web.Controllers {
    public class UserController : BaseController {
        #region fields
        /// <summary>
        /// The _login service
        /// </summary>
        private readonly ILoginService _loginService;
        /// <summary>
        /// The _user service
        /// </summary>
        private readonly IUserService _userService;
        /// <summary>
        /// The _email service
        /// </summary>
        private readonly IEmailService _emailService;

        private readonly ISubscriberService _subscriberService;
        #endregion

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="loginService">The login service.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="emailService">The email service.</param>
        public UserController(ILoginService loginService, ISubscriberService subscriberService,
            IUserService userService, IEmailService emailService) {
            _loginService = loginService;
            _userService = userService;
            _emailService = emailService;
            _subscriberService = subscriberService;
            }
        #endregion

        #region methods
        /// <summary>
        /// Adds a link to the site on the user's desktop.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult AddLink() {
            string appUrl = SiteSettings.Settings().AppUrl;
            string title = "Prospect Tutoring";
            urlShortcutToDesktop(title, appUrl);
            return RedirectToAction("SignIn", "User");
        }

        /// <summary>
        /// GET: /User/Create
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Create() {
            var user = new User {
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            return View(user);
        }

        /// <summary>
        /// POST: /User/Create
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="confirmPassword">The confirm password.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Passwords do not match</exception>
        [HttpPost]
        public ActionResult Create(User user, string confirmPassword) {
            try {
                if (!CommonHelper.IsValidEmail(user.Username)) {
                    throw new Exception("Username must be an email address");
                }

                if (confirmPassword.Equals(user.PasswordHash, StringComparison.Ordinal)) {
                    if (!_userService.Check(user)) {
                        throw new Exception("There is already a User with this username, please change this username.");
                    }

                    var originalPassword = user.PasswordHash;
                    var salt = "";
                    var hashedPassword = SecurityHelper.HashPassword(user.PasswordHash, ref salt);
                    user.PasswordHash = hashedPassword;
                    user.PasswordSalt = salt;

                    // Set Date and CreatedBy properties
                    user.CreatedBy = SessionDataHelper.UserId;
                    user.CreatedDate = DateTime.Now;
                    user.ModifiedBy = SessionDataHelper.UserId;
                    user.ModifiedDate = DateTime.Now;

                    if (user.Role != Role.Subscriber || SessionDataHelper.SubscriberId != 0) {
                        user.SubscriberId = SessionDataHelper.SubscriberId;
                    }

                    Success("Record Saved.");
                    _userService.Insert(user);

                    _emailService.SendNewUserEmail(user, originalPassword);

                    return RedirectToAction("Index");
                }
                throw new Exception("Passwords do not match");
            } catch (Exception ex) {
                Error(ex.Message);
                return View(user);
            }
        }

        /// <summary>
        /// GET: /User/Delete/5
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public ActionResult Delete(int userId) {
            try {
                _userService.Delete(userId);
                return new EmptyResult();
            } catch (Exception ex) {
                Error(ex.Message);
                return new EmptyResult();
            }
        }

        /// <summary>
        /// GET: /User/Edit/5
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public ActionResult Edit(int userId) {
            var user = _userService.GetById(userId);
            return View(user);
        }

        /// <summary>
        /// POST: /User/Edit/5
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="confirmPassword"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(User user, string confirmPassword) {
            try {
                if (!CommonHelper.IsValidEmail(user.Username)) {
                    throw new Exception("Username must be an email address");
                }

                if (user.PasswordHash == null || confirmPassword == user.PasswordHash) {
                    if (!_userService.Check(user)) {
                        throw new Exception("There is already a User with this username, please change this username.");
                    }

                    var tmp = _userService.GetById(user.Id);

                    if (String.IsNullOrEmpty(user.PasswordHash)) {
                        user.PasswordHash = tmp.PasswordHash;
                        user.PasswordSalt = tmp.PasswordSalt;

                    } else {
                        var salt = tmp.PasswordSalt;
                        var hashedPassword = SecurityHelper.HashPassword(user.PasswordHash, ref salt);
                        user.PasswordHash = hashedPassword;
                        user.PasswordSalt = salt;
                    }

                    // Set Date and CreatedBy properties
                    user.CreatedBy = SessionDataHelper.UserId;
                    user.CreatedDate = DateTime.Now;
                    user.ModifiedBy = SessionDataHelper.UserId;
                    user.ModifiedDate = DateTime.Now;

                    if (user.Role != Role.Subscriber) {
                        user.SubscriberId = SessionDataHelper.SubscriberId;
                    }

                    Success("Record Saved.");
                    _userService.Update(user);
                    return RedirectToAction("Index");
                }
                throw new Exception("Passwords do not match");
            } catch (Exception ex) {
                Error(ex.Message);
                return View(user);
            }
        }

        /// <summary>
        /// POST: /User/SignIn
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResetPassword(UserSignInModel model, string returnUrl = "swag")
        {
            try
            {
                User user = _userService.GetUserByUsername(model.Username);
                if (user != null)
                {
                    var salt = "";
                    var tempPassword = CommonHelper.GenerateRandomString();
                    var hashedPassword = SecurityHelper.HashPassword(tempPassword, ref salt);
                    user.PasswordHash = hashedPassword;
                    user.PasswordSalt = salt;
                    user.IsLocked = true;

                    _userService.Update(user);
                    _emailService.SendResetPasswordEmail(user, tempPassword);
                }
                else
                {
                    throw new Exception("Invalid username.");
                }
            } catch (Exception ex){
                return RedirectToAction("SignIn",ex.Message);
            }
            return RedirectToAction("SignIn", "User");
        }

        /// <summary>
        /// GET : /user/index
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        public ActionResult Index(int page = 1, int pageSize = 25) {
            try {
                var users = _userService.GetAllUser(page - 1, pageSize, SessionDataHelper.UserRole);
                return View(users);
            } catch {
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// GET: /User/SignIn
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult SignIn(string returnUrl) {
            var model = new UserSignInModel();
            model.ReturnUrl = returnUrl;
            return View("SignIn", model);
        }

        /// <summary>
        /// GET: /User/SignIn
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangePassword()
        {
            var model = new UserSignInModel();
            model.Username = SessionDataHelper.Username;
            return View("_ChangePasswordPartial", model);
        }

        /// <summary>
        /// POST: /User/SignIn
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangePassword(UserSignInModel model, string newPassword, string confirmPassword)
        {
            try {
                if (_userService.ValidateLogin(model.Username, model.Password)) {
                    if (newPassword.Equals(confirmPassword, StringComparison.Ordinal)) {
                            var user = _userService.GetUserByUsername(model.Username);
                            var salt = "";
                            var hashedPassword = SecurityHelper.HashPassword(newPassword, ref salt);
                            user.PasswordHash = hashedPassword;
                            user.PasswordSalt = salt;
                            user.IsLocked = false;

                            Success("Record Saved.");
                            _userService.Update(user);
                            return RedirectToAction("Index", "Home");
                        }
                        throw new Exception("Passwords do not match");
                }
                throw new Exception("Current Password is incorrect");

            } catch (Exception ex) {
                Error(ex.Message);
                return RedirectToAction("ChangePassword");
            }
        }

        /// <summary>
        /// POST: /User/SignIn
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult SignIn(UserSignInModel model, string returnUrl) {
            try {
                if (_userService.ValidateLogin(model.Username, model.Password)) {
                    var user = _userService.GetUserByUsername(model.Username);

                    if (_userService.VerifyKeyCheck(user)) {

                        // Set the last login date
                        user.LastLogin = DateTime.Now;
                        user.PasswordFailuresSinceLastSuccess = 0;
                        _userService.Update(user);

                        FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);

                        // Store data in session
                        SessionDataHelper.Username = model.Username;
                        SessionDataHelper.UserRole = user.Role;
                        SessionDataHelper.UserId = user.Id;
                        SessionDataHelper.SessionId = System.Web.HttpContext.Current.Session.SessionID;

                        // Store the login entry
                        if (SessionDataHelper.UserId != 1)
                        {
                            _loginService.LogUser(SessionDataHelper.UserId, SessionDataHelper.SessionId);
                        }

                        SessionDataHelper.SubscriberId = user.SubscriberId ?? 0;

                        if (Url.IsLocalUrl(returnUrl)) {
                            return Redirect(returnUrl);
                        }
                        if (user.IsLocked)
                        {
                            UserSignInModel signInModel = new UserSignInModel();
                            return ChangePassword();
                        } 
                        return RedirectToAction("Index", "Home");
                    }

                    throw new Exception("Your subscription is out of date, please provide payment to renew.");
                }
                throw new Exception("Your username/password combination was incorrect");
            } catch (Exception ex) {
                ModelState.AddModelError("Error", ex.Message);
            }
            return View(model);

        }

        /// <summary>
        /// POST : /user/signout
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SignOut() {
            FormsAuthentication.SignOut();
            SessionHelper.Abandon();
            Session.Abandon();

            FormsAuthentication.RedirectToLoginPage();
            return RedirectToAction("SignIn", "User");
        }

        /// <summary>
        /// Users the list.
        /// </summary>
        /// <param name="jtStartIndex">Start index of the jt.</param>
        /// <param name="jtPageSize">Size of the jt page.</param>
        /// <param name="jtSorting">The jt sorting.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UserList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null) {
            try {
                var userCount = _userService.GetAll().Count(x => x.SubscriberId == SessionDataHelper.SubscriberId);

                // Get data from database
                var users = _userService.GetPagedUsers(SessionDataHelper.UserRole, SessionDataHelper.SubscriberId, jtStartIndex, jtPageSize, jtSorting);

                // Format some data
                var results = users.Select(p => new {
                    p.Id,
                    p.Username,
                    p.FullName,
                    Role = p.Role.ToString(),
                    p.IsActive
                }).ToArray();

                //Return result to jTable
                return Json(new { Result = "OK", Records = results, TotalRecordCount = userCount });
            } catch (Exception ex) {
                return Json(new { Result = "ERROR", ex.Message });
            }
        }
        #endregion

        #region admin methods
        /// <summary>
        /// Signs the in as.
        /// </summary>
        /// <param name="subscriberId">The subscriber id.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public JsonResult SignInAs(int? subscriberId, string name = "None") {
            if (SessionDataHelper.UserRole <= Role.SystemAdminSubscriber) {
                if (subscriberId != null && subscriberId.Value > 0) {
                    SessionDataHelper.SubscriberId = subscriberId.Value;
                    SessionDataHelper.UserRole = Role.SystemAdminSubscriber;
                } else {
                    SessionDataHelper.SubscriberId = 0;
                    SessionDataHelper.UserRole = Role.SystemAdmin;
                }
                SessionDataHelper.SubscriberName = name;
            }

            if (Request.UrlReferrer == null)
            {
                return Json(new { url = "/Home/Index" });
            }

            var url = Request.UrlReferrer.AbsolutePath;
            var urlSplit = url.Split('/');

            if (urlSplit[1] == "Subscriber" && SessionDataHelper.UserRole != Role.SystemAdmin) {
                return Json(new { url = "/Home/Index" });
            }

            if (urlSplit[1] == "User" && SessionDataHelper.UserRole == Role.SystemAdmin) {
                return Json(new { url = "/Home/Index" });
            }

            return Json(new { url = "/" + urlSplit[1] });
        }
        #endregion

        [AllowAnonymous]
        public bool CheckUsername(string username)
        {
            var test = _userService.GetUserByUsername(username);
            return (test == null) ? false : true;
        }

        /// <summary>
        /// Reloads the PDF table.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public PartialViewResult LoadForgotPassword(UserSignInModel model)
        {
            return PartialView("_ForgotPassword", model);
        }

        #region private methods
        /// <summary>
        /// URLs the shortcut to desktop.
        /// </summary>
        /// <param name="linkName">Name of the link.</param>
        /// <param name="linkUrl">The link URL.</param>
        private void urlShortcutToDesktop(string linkName, string linkUrl) {
            string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            using (StreamWriter writer = new StreamWriter(deskDir + "\\" + linkName + ".url")) {
                writer.WriteLine("[InternetShortcut]");
                writer.WriteLine("URL=" + linkUrl);
                writer.Flush();
            }
        }
        #endregion
    } // class
} // namespace