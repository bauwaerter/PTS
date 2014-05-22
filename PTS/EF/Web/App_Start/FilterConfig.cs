using System.Web;
using System.Web.Mvc;
using Blizzard.Filters;

namespace Blizzard {
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CacheFilter());
        }
    }
}