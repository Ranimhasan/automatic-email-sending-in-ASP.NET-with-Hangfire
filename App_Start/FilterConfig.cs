using System.Web;
using System.Web.Mvc;

namespace Sozlesme_Takip_Sistemi1
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
