using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Pictures.AzurePictureService.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Pictures.AzurePictureService.Controllers
{
    public class AzurePictureServiceController : BasePluginController
    {
        private readonly ISettingService _settingService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;

        private readonly AzurePictureServiceSettings _azurePictureServiceSettings;
        private readonly ILocalizationService _localizationService;

        public AzurePictureServiceController(ISettingService settingService, 
            IPaymentService paymentService, IOrderService orderService, 
            IOrderProcessingService orderProcessingService,
             ILocalizationService localizationService,
            AzurePictureServiceSettings azurePictureServiceSettings,
            PaymentSettings paymentSettings)
        {
            this._settingService = settingService;
            this._paymentService = paymentService;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._azurePictureServiceSettings = azurePictureServiceSettings;
            this._localizationService = localizationService;
        }
        

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel();
            model.ConnectionString = _azurePictureServiceSettings.ConnectionString;
            model.CollectionName = _azurePictureServiceSettings.ContainerName;
            model.IsEnabled= _azurePictureServiceSettings.IsEnabled;

            return View("~/Plugins/Pictures.AzurePictureService/Views/AzurePictureService/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _azurePictureServiceSettings.ConnectionString = model.ConnectionString;
            _azurePictureServiceSettings.ContainerName = model.CollectionName;
            _azurePictureServiceSettings.IsEnabled = model.IsEnabled;
            _settingService.SaveSetting(_azurePictureServiceSettings);
            
            //return View("Nop.Plugin.Payments.Payu.Views.PaymentPayu.Configure", model);
            //return View("~/Plugins/Payments.Payu/Views/PaymentPayu/Configure.cshtml", model);

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }
        
    }
}