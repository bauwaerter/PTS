using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using ClassLibrary1.Helpers;
using Service.Services;

namespace PTS.Infrastructure {
    /// <summary>
    /// Validates login sessions
    /// </summary>
    public class LoginValidate : ActionFilterAttribute {
        #region Overridden Methods
        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext) 
        {
            if (!string.IsNullOrEmpty(SessionDataHelper.SessionId)) 
            {
                // Create login service
                var login = new LoginService();

                if (SessionDataHelper.UserId != 1) {
                    if (!login.CheckLogin(SessionDataHelper.UserId, SessionDataHelper.SessionId)) {
                        FormsAuthentication.SignOut();
                        SessionHelper.Abandon();

                        FormsAuthentication.RedirectToLoginPage();
                        // Redirect to login page.
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Account" }, { "action", "Login" } });
                    }
                }
            }

            base.OnActionExecuting(filterContext);
        }


        #endregion
    }
}
