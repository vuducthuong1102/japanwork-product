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

namespace Manager.WebApp.Controllers
{
    public class FooterController : BaseAuthedController
    {
        private readonly IStoreFooter _mainStore;
        private readonly ILog logger = LogProvider.For<FooterController>();

        public FooterController(IStoreFooter mainStore)
        {
            _mainStore = mainStore;
        }

        [AccessRoleChecker]
        public ActionResult Index(ManageFooterModel model)
        {
            try
            {
                model.FooterInfo = _mainStore.GetByLangCode(UserCookieManager.GetCurrentLanguageOrDefault());        
                if(model.FooterInfo != null)
                {
                    model.BodyContent = model.FooterInfo.BodyContent;
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
                //Begin db transaction
                IdentityFooter info = new IdentityFooter();
                info.BodyContent = model.BodyContent;
                info.LangCode = UserCookieManager.GetCurrentLanguageOrDefault();

                _mainStore.Update(info);

                //Clear cache
                FrontendCachingHelpers.ClearFooterCached();

                this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Create Footer request: " + ex.ToString());

                return View(model);
            }

            return View(model);
        }       
    }
}