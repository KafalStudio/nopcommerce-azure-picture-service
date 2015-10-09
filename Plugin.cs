using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;

namespace Nop.Plugin.Pictures.AzurePictureService
{
    public class Plugin:BasePlugin, IMiscPlugin
    {
        private readonly ISettingService _settingService;

        public Plugin(ISettingService settingService)
        {
            _settingService = settingService;
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "AzurePictureService";
            routeValues = new RouteValueDictionary()
            {
                { "Namespaces", "Nop.Plugin.Pictures.AzurePictureService.Controllers" }, { "area", null }
            };
        }

        public override void Install()
        {
            var settings = new AzurePictureServiceSettings()
            {
                ConnectionString = "UseDevelopmentStorage=true",
                ContainerName = "AzureImages"
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Pictures.AzurePictureService.ContainerName", "Container Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Pictures.AzurePictureService.ContainerName.Hint", "Container Name to use for storing images.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Pictures.AzurePictureService.ConnectionString", "Connection String");
            this.AddOrUpdatePluginLocaleResource("Plugins.Pictures.AzurePictureService.ConnectionString.Hint", "Connection string to be used for connecting to Azure Blob Storage.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Pictures.AzurePictureService.IsEnabled", "Enable");
            this.AddOrUpdatePluginLocaleResource("Plugins.Pictures.AzurePictureService.IsEnabled.Hint", "Enable.");
            base.Install();
        }
    }
}
