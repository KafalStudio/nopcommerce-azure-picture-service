using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Pictures.AzurePictureService
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Pictures.AzurePictureService.Configure",
                 "Plugins/AzurePictureService/Configure",
                 new { controller = "AzurePictureService", action = "Configure" },
                 new[] { "Nop.Plugin.Pictures.AzurePictureService.Controllers" }
            );
        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
