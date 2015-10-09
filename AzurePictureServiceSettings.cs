using Nop.Core.Configuration;

namespace Nop.Plugin.Pictures.AzurePictureService
{
    public class AzurePictureServiceSettings : ISettings
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
        public bool IsEnabled { get; set; }
        
    }
}
