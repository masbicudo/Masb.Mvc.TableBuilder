using System.Web.Mvc;

namespace Masb.Mvc.TableBuilder.Sample
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}