using System.Web.Mvc;
using PTS.Infrastructure;
using Core.Web.Filters;

namespace PTS
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AuthorizeAttribute());
            filters.Add(new HandleErrorAttribute());
            filters.Add(new NoCacheFilter());
            filters.Add(new LoginValidate());
        }
    }
}