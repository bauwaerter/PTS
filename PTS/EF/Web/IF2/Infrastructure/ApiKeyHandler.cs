using System.Net.Http;
using System.Security.Principal;
using System.Web;

namespace Web.Infrastructure
{
    /// <summary>
    /// Api Key Handler
    /// </summary>
    public class ApiKeyHandler : DelegatingHandler
    {
        /// <summary>
        /// Sets the principal to current HTTP context
        /// </summary>
        /// <param name="principal"></param>
        private void SetPrincipal(IPrincipal principal)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
        }
    } // class
} // namespace