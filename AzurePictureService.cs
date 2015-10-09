#region using

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Media;

#endregion

namespace Nop.Plugin.Pictures.AzurePictureService
{
    public class AzurePictureService : PictureService
    {
        private const string AccountCacheKey = "AzurePictureService.BlobAccount";
        private CloudBlobClient _blobClient;
        private CloudBlobContainer _container;
        private readonly ICacheManager _cacheManager;
        private readonly AzurePictureServiceSettings _settings;
        private readonly ISettingService _settingService;

        public AzurePictureService(ICacheManager cacheManager_,
            AzurePictureServiceSettings settings_,
            IRepository<Picture> pictureRepository, IRepository<ProductPicture> productPictureRepository,
            ISettingService settingService, IWebHelper webHelper, ILogger logger, IDbContext dbContext, IEventPublisher eventPublisher,
            MediaSettings mediaSettings)
            : base(
                pictureRepository, productPictureRepository, settingService, webHelper, logger, dbContext, eventPublisher,
                mediaSettings)
        {
            _cacheManager = cacheManager_;
            _settings = settings_;
            _settingService = settingService;
        }

        private CloudStorageAccount BlobAccount
        {
            get
            {
                return _cacheManager.Get(AccountCacheKey, () => CloudStorageAccount.Parse(_settings.ConnectionString));
            }
        }

        public CloudBlobClient BlobClient
        {
            get { return _blobClient ?? (_blobClient = BlobAccount.CreateCloudBlobClient()); }
        }

        public CloudBlobContainer Container
        {
            get
            {
                if (_container == null)
                {
                    _container = BlobClient.GetContainerReference(_settings.ContainerName.ToLower());
                    _container.CreateIfNotExists();
                }
                return _container;
            }
        }

        public override bool StoreInDb
        {
            get { return base.StoreInDb; }
            set
            {
                var oldValue = StoreInDb;

                try
                {
                    base.StoreInDb = value;
                }
                catch (Exception ex)
                {
                    _settingService.SetSetting("Media.Images.StoreInDB", oldValue);
                }
            }
        }

        protected override byte[] LoadPictureFromFile(int pictureId, string mimeType)
        {
            if (!_settings.IsEnabled)
            {
                return base.LoadPictureFromFile(pictureId, mimeType);
            }

            return _cacheManager.Get(GetCacheIdForPicture(pictureId),
                () => LoadPictureFromBlobAsync(pictureId, mimeType).Result);
        }

        
        protected override void DeletePictureOnFileSystem(Picture picture)
        {
            if (!_settings.IsEnabled)
            {
                base.DeletePictureOnFileSystem(picture);
                return;
            }

            if (picture == null)
                throw new ArgumentNullException("picture");

            var lastPart = GetFileExtensionFromMimeType(picture.MimeType);
            var fileName = string.Format("{0}_0.{1}", picture.Id.ToString("0000000"), lastPart);

            Container.GetBlockBlobReference(fileName).DeleteIfExists();
            _cacheManager.Remove(GetCacheIdForPicture(picture.Id));
        }

        protected override void SavePictureInFile(int pictureId, byte[] pictureBinary, string mimeType)
        {
            if (!_settings.IsEnabled)
            {
                base.SavePictureInFile(pictureId, pictureBinary, mimeType);
                return;
            }

            var lastPart = GetFileExtensionFromMimeType(mimeType);
            var fileName = string.Format("{0}_0.{1}", pictureId.ToString("0000000"), lastPart);

            var blob = Container.GetBlockBlobReference(fileName);
            blob.Properties.ContentType = mimeType;
            blob.UploadFromByteArray(pictureBinary, 0, pictureBinary.Length);
            _cacheManager.Set(GetCacheIdForPicture(pictureId), pictureBinary, 30);
        }

        private string GetCacheIdForPicture(int pictureId)
        {
            return "AzurePictureService" + pictureId;
        }

        private async Task<byte[]> LoadPictureFromBlobAsync(int pictureId, string mimeType)
        {
            var lastPart = GetFileExtensionFromMimeType(mimeType);
            var fileName = string.Format("{0}_0.{1}", pictureId.ToString("0000000"), lastPart);
            using (var stream = new MemoryStream())
            {
                await Container.GetBlockBlobReference(fileName).DownloadToStreamAsync(stream).ConfigureAwait(false);
                return stream.ToArray();
            }
        }

    }
}