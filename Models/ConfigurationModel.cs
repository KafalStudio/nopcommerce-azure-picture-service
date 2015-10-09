using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Pictures.AzurePictureService.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Pictures.AzurePictureService.ConnectionString")]
        public string ConnectionString { get; set; }

        [NopResourceDisplayName("Plugins.Pictures.AzurePictureService.ContainerName")]
        public string CollectionName { get; set; }

        [NopResourceDisplayName("Plugins.Pictures.AzurePictureService.IsEnabled")]
        public bool IsEnabled { get; set; }

    }
}