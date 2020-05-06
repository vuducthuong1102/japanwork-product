using System;
using System.Web.Mvc;
using Manager.WebApp.Helpers;
using Manager.WebApp.Services;
using MsSql.AspNet.Identity;
using Manager.WebApp.Settings;
using Manager.WebApp.Models;
using System.Net;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Resources;
using Newtonsoft.Json;
using System.Collections.Generic;
using Manager.SharedLibs;

namespace Manager.WebApp.Controllers
{
    public class FrontEndSystemController : BaseAuthedController
    {        
        IFrontEndSettingsService _frontSettingService;
        private readonly ILog logger = LogProvider.For<SystemController>();

        public FrontEndSystemController(IFrontEndSettingsService settingService)
        {
            _frontSettingService = settingService;
        }

        [AccessRoleChecker(AdminRequired = true)]
        public ActionResult FrontEndSettings()
        {
            ModelState.Clear();

            var settings = new SiteFrontEndSettings();
            settings.Load(_frontSettingService, false);

            var model = new FrontEndSettingsViewModel()
            {
                CurrentFrontEndSettingsType = settings.General.GetType().Name,
                SystemSestings = settings,
            }
            ;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FrontEndSettings(FrontEndSettingsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var logoImg = string.Empty;
                    if (model.SiteLogoUpload != null)
                    {
                        if (model.SiteLogoUpload[0] != null)
                        {
                            var uploadedUrls = UploadImages("images");
                            if (uploadedUrls.HasData())
                            {
                                logoImg = uploadedUrls[0];
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(logoImg))
                        logoImg = model.CurrentLogo;

                    logoImg = RemoveServerUrl(logoImg);

                    model.SystemSestings.General.SiteLogo = logoImg;
                    model.SystemSestings.Save(_frontSettingService);

                    this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
                }
                else
                {
                    this.AddNotificationModelStateErrors(ModelState);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to save FrontEndSettings because: " + ex.ToString());
            }
            return View(model);
        }
     
    }
}