using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Settings;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.MsSqlStores;
using Manager.WebApp.Resources;
using Microsoft.AspNet.Identity;
using Manager.WebApp.Models;
using Manager.WebApp.Caching;
using ApiJobMarket.DB.Sql.Entities;
using Manager.WebApp.Services;
using Newtonsoft.Json;

namespace Manager.WebApp.Controllers
{
    public class FooterController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<FooterController>();

        public FooterController()
        {

        }

        [AccessRoleChecker]
        public ActionResult Index(ManageFooterModel model)
        {
            try
            {
                var apiResult = FooterServices.GetFooterAsync().Result;
                if(apiResult != null)
                {
                    if(apiResult.value != null)
                    {
                        model.FooterInfo = JsonConvert.DeserializeObject<IdentityFooter>(apiResult.value.ToString());

                        if (model.FooterInfo != null)
                        {
                            model.BodyContent = model.FooterInfo.BodyContent;
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to display Footer Manage page because: " + ex.ToString());

                return View(model);
            }

            return View(model);
        }
        
        [HttpPost]
        [ActionName("Index")]
        public ActionResult Index_Post(ManageFooterModel model)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage + x.Exception));
                this.AddNotification(messages, NotificationType.ERROR);
                return View(model);
            }

            try
            {
                var apiFooterModel = new ApiFooterModel();
                apiFooterModel.BodyContent = model.BodyContent;
                apiFooterModel.LanguageCode = UserCookieManager.GetCurrentLanguageOrDefault();

                //_mainStore.Update(info);
                var result = FooterServices.UpdateAsync(apiFooterModel).Result;

                //Clear cache
                //FrontendCachingHelpers.ClearFooterCached();

                this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Update Footer request: " + ex.ToString());

                return View(model);
            }

            return View(model);
        }       
    }
}