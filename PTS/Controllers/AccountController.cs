using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Core;
using Core.Helpers.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using Newtonsoft.Json.Serialization;
using WebMatrix.WebData;
using PTS.Filters;
using PTS.Models;
using Service.Interfaces;
using Core.Domains;
using PTS.Infrastructure;

namespace PTS.Controllers
{
    //[Authorize]
    //[InitializeSimpleMembership]
    public class AccountController : BaseController
    {

        #region fields

        //private readonly IBaseService<StudentUser> _studentUserService;
        private readonly IUserService _userService;
        //private readonly IStudentUserService _studentUserService;
        private readonly IBaseService<StudentUser> _studentUserService;
        private readonly IBaseService<Class> _classService;
        private readonly IBaseService<Location> _locationService;

        #endregion


        public AccountController(IUserService userService, IBaseService<StudentUser> studentUserService, IBaseService<Class> classService, IBaseService<Location> locationService)
        {

            _userService = userService;
            _studentUserService = studentUserService;
            _classService = classService;
            _locationService = locationService;

        }
        //
        // GET: /Account/Login

        //[AllowAnonymous]
        //public ActionResult Login(string returnUrl)
        //{
        //    ViewBag.ReturnUrl = returnUrl;
        //    return View();
        //}

        public ActionResult SaveUser(User user)
        {
            try
            {
                _userService.Update(user);
                return Json(new
                {
                    Result="OK"
                });
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }


        public ActionResult ManageAccount()
        {
            var model = _userService.GetById(15);
            var user = new AccountUser();
            var loc = new LocationVM();
            
            if (model.LocationId.HasValue)
            {
                int locid = model.LocationId.Value;
                var getlocation = _locationService.GetById(locid);
                loc = new LocationVM 
                {
                    Address =getlocation.Address,
                    City = getlocation.City,
                    Country = getlocation.Country,
                    id = getlocation.Id,
                    State = getlocation.State,
                    ZipCode = getlocation.ZipCode
                };
                //loc = _locationService.GetById(locid);
                
            }
            

            if(model.Role== UserRole.Student)
            {
                var student= _studentUserService.GetById(15);
                user = new AccountUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Education = student.Education,
                    Email = model.Email,
                    Id = model.Id,
                    Major = student.Major,
                    PassWord = model.PassWord,
                    Location = loc
                };
            }

            return View(user);
        }

       
        //
        // POST: /Account/Login

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult Login(LoginModel model, string returnUrl)
        //{
        //    if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
        //    {
        //        return RedirectToLocal(returnUrl);
        //    }

        //    // If we got this far, something failed, redisplay form
        //    ModelState.AddModelError("", "The user name or password provided is incorrect.");
        //    return View(model);
        //}

        //
        // POST: /Account/LogOff

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LogOff()
        //{
        //    WebSecurity.Logout();

        //    return RedirectToAction("Index", "Home");
        //}

        //
        // GET: /Account/Register

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(User user, string confirmPassword) {
            if (!CommonHelper.IsValidEmail(user.Email)) {
                throw new Exception("Username must be a valid email address");
            }
            
            try {
                if (confirmPassword.Equals(user.PassWord, StringComparison.Ordinal)) {
                    var salt = "";
                    var hashedPassword = SecurityHelper.HashPassword(user.PassWord, ref salt);
                    user.PassWord = hashedPassword;
                    user.PasswordSalt = salt;

                    _userService.Insert(user);

                    return RedirectToAction("Index", "Home");
                }
            throw new Exception("Passwords do not match");
            } catch (Exception e) {
                Error(e.Message);
                return View(user);
            }
        }

        // GET: /Account/RoleSelect
        [AllowAnonymous]
        [HttpGet]
        public ActionResult RoleSelect(User user) {
            return View(user);
        }

        //
        // POST: /Account/Disassociate

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Disassociate(string provider, string providerUserId)
        //{
        //    string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
        //    ManageMessageId? message = null;

        //    // Only disassociate the account if the currently logged in user is the owner
        //    if (ownerAccount == User.Identity.Name)
        //    {
        //        // Use a transaction to prevent the user from deleting their last login credential
        //        using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
        //        {
        //            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        //            if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
        //            {
        //                OAuthWebSecurity.DeleteAccount(provider, providerUserId);
        //                scope.Complete();
        //                message = ManageMessageId.RemoveLoginSuccess;
        //            }
        //        }
        //    }

        //    return RedirectToAction("Manage", new { Message = message });
        //}

        //
        // GET: /Account/Manage

        //public ActionResult Manage(ManageMessageId? message)
        //{
        //    ViewBag.StatusMessage =
        //        message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
        //        : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
        //        : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
        //        : "";
        //    ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        //    ViewBag.ReturnUrl = Url.Action("Manage");
        //    return View();
        //}

        //
        // POST: /Account/Manage

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Manage(LocalPasswordModel model)
        //{
        //    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        //    ViewBag.HasLocalPassword = hasLocalAccount;
        //    ViewBag.ReturnUrl = Url.Action("Manage");
        //    if (hasLocalAccount)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            // ChangePassword will throw an exception rather than return false in certain failure scenarios.
        //            bool changePasswordSucceeded;
        //            try
        //            {
        //                changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
        //            }
        //            catch (Exception)
        //            {
        //                changePasswordSucceeded = false;
        //            }

        //            if (changePasswordSucceeded)
        //            {
        //                return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // User does not have a local password so remove any validation errors caused by a missing
        //        // OldPassword field
        //        ModelState state = ModelState["OldPassword"];
        //        if (state != null)
        //        {
        //            state.Errors.Clear();
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            try
        //            {
        //                WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
        //                return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
        //            }
        //            catch (Exception)
        //            {
        //                ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
        //            }
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //
        // POST: /Account/ExternalLogin

    //    [HttpPost]
    //    [AllowAnonymous]
    //    [ValidateAntiForgeryToken]
    //    public ActionResult ExternalLogin(string provider, string returnUrl)
    //    {
    //        return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
    //    }

    //    //
    //    // GET: /Account/ExternalLoginCallback

    //    [AllowAnonymous]
    //    public ActionResult ExternalLoginCallback(string returnUrl)
    //    {
    //        AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
    //        if (!result.IsSuccessful)
    //        {
    //            return RedirectToAction("ExternalLoginFailure");
    //        }

    //        if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
    //        {
    //            return RedirectToLocal(returnUrl);
    //        }

    //        if (User.Identity.IsAuthenticated)
    //        {
    //            // If the current user is logged in add the new account
    //            OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
    //            return RedirectToLocal(returnUrl);
    //        }
    //        else
    //        {
    //            // User is new, ask for their desired membership name
    //            string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
    //            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
    //            ViewBag.ReturnUrl = returnUrl;
    //            return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
    //        }
    //    }

    //    //
    //    // POST: /Account/ExternalLoginConfirmation

    //    [HttpPost]
    //    [AllowAnonymous]
    //    [ValidateAntiForgeryToken]
    //    public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
    //    {
    //        string provider = null;
    //        string providerUserId = null;

    //        if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
    //        {
    //            return RedirectToAction("Manage");
    //        }

    //        if (ModelState.IsValid)
    //        {
    //            // Insert a new user into the database
    //            using (UsersContext db = new UsersContext())
    //            {
    //                UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
    //                // Check if user already exists
    //                if (user == null)
    //                {
    //                    // Insert name into the profile table
    //                    db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
    //                    db.SaveChanges();

    //                    OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
    //                    OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

    //                    return RedirectToLocal(returnUrl);
    //                }
    //                else
    //                {
    //                    ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
    //                }
    //            }
    //        }

    //        ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
    //        ViewBag.ReturnUrl = returnUrl;
    //        return View(model);
    //    }

    //    //
    //    // GET: /Account/ExternalLoginFailure

    //    [AllowAnonymous]
    //    public ActionResult ExternalLoginFailure()
    //    {
    //        return View();
    //    }

    //    [AllowAnonymous]
    //    [ChildActionOnly]
    //    public ActionResult ExternalLoginsList(string returnUrl)
    //    {
    //        ViewBag.ReturnUrl = returnUrl;
    //        return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
    //    }

    //    [ChildActionOnly]
    //    public ActionResult RemoveExternalLogins()
    //    {
    //        ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
    //        List<ExternalLogin> externalLogins = new List<ExternalLogin>();
    //        foreach (OAuthAccount account in accounts)
    //        {
    //            AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

    //            externalLogins.Add(new ExternalLogin
    //            {
    //                Provider = account.Provider,
    //                ProviderDisplayName = clientData.DisplayName,
    //                ProviderUserId = account.ProviderUserId,
    //            });
    //        }

    //        ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
    //        return PartialView("_RemoveExternalLoginsPartial", externalLogins);
    //    }

    //    #region Helpers
    //    private ActionResult RedirectToLocal(string returnUrl)
    //    {
    //        if (Url.IsLocalUrl(returnUrl))
    //        {
    //            return Redirect(returnUrl);
    //        }
    //        else
    //        {
    //            return RedirectToAction("Index", "Home");
    //        }
    //    }

    //    public enum ManageMessageId
    //    {
    //        ChangePasswordSuccess,
    //        SetPasswordSuccess,
    //        RemoveLoginSuccess,
    //    }

    //    internal class ExternalLoginResult : ActionResult
    //    {
    //        public ExternalLoginResult(string provider, string returnUrl)
    //        {
    //            Provider = provider;
    //            ReturnUrl = returnUrl;
    //        }

    //        public string Provider { get; private set; }
    //        public string ReturnUrl { get; private set; }

    //        public override void ExecuteResult(ControllerContext context)
    //        {
    //            OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
    //        }
    //    }

    //    private static string ErrorCodeToString(MembershipCreateStatus createStatus)
    //    {
    //        // See http://go.microsoft.com/fwlink/?LinkID=177550 for
    //        // a full list of status codes.
    //        switch (createStatus)
    //        {
    //            case MembershipCreateStatus.DuplicateUserName:
    //                return "User name already exists. Please enter a different user name.";

    //            case MembershipCreateStatus.DuplicateEmail:
    //                return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

    //            case MembershipCreateStatus.InvalidPassword:
    //                return "The password provided is invalid. Please enter a valid password value.";

    //            case MembershipCreateStatus.InvalidEmail:
    //                return "The e-mail address provided is invalid. Please check the value and try again.";

    //            case MembershipCreateStatus.InvalidAnswer:
    //                return "The password retrieval answer provided is invalid. Please check the value and try again.";

    //            case MembershipCreateStatus.InvalidQuestion:
    //                return "The password retrieval question provided is invalid. Please check the value and try again.";

    //            case MembershipCreateStatus.InvalidUserName:
    //                return "The user name provided is invalid. Please check the value and try again.";

    //            case MembershipCreateStatus.ProviderError:
    //                return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

    //            case MembershipCreateStatus.UserRejected:
    //                return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

    //            default:
    //                return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
    //        }
    //    }
    //    #endregion
    }
}
