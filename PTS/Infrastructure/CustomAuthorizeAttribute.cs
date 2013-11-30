using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core;
using Core.Domains;

namespace PTS.Infrastructure
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly UserRoles[] _userRoles;

        public CustomAuthorizeAttribute(params UserRoles[] userRoles)
        {
            _userRoles = userRoles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            if (httpContext.Session == null)
                return false;

            if (httpContext.User == null || !httpContext.User.Identity.IsAuthenticated)
                return false;

            var curUser = httpContext.User as CustomPrincipal;
            if (curUser == null)
                return false;

            if (curUser.Role == default(UserRoles))
                return false;

            if (curUser.IsFirstLogin)
            {
                var urlHelper = new UrlHelper(httpContext.Request.RequestContext);
                httpContext.Response.Redirect(urlHelper.Action("Index", "Home") ?? "/");
                return false;
            }

            if (_userRoles.Length > 0 && !curUser.IsInRole(_userRoles))
            {
                var urlHelper = new UrlHelper(httpContext.Request.RequestContext);
                httpContext.Response.Redirect(urlHelper.Action("Index", "Home") ?? "/");
                return false;
            }

            //if (curUser.IsInRole(RoleEnum.CustomerUser))
            //{
            //    var id = (httpContext.Request.RequestContext.RouteData.Values["id"])
            //             ??
            //             (httpContext.Request["id"]);

            //    if (id != null && Convert.ToInt64(id) != curUser.CustomerId)
            //        return false;
            //}

            return true;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any())
                return;

            if (!AuthorizeCore(filterContext.HttpContext))
            {
                if (filterContext.HttpContext.Response.RedirectLocation != null)
                    return;

                var urlHelper = new UrlHelper(filterContext.RequestContext);
                var returnUrl = filterContext.HttpContext.Request.RawUrl;
                if (returnUrl == "/")
                    returnUrl = "";

                filterContext.HttpContext.Response.Redirect(urlHelper.Action("Login", "Home", new { returnUrl }) ?? "/");
            }
        }
    }
}