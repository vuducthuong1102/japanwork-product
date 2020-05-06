using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Settings;
using MsSql.AspNet.Identity.Entities;
using Manager.WebApp.Resources;
using MsSql.AspNet.Identity.MsSqlStores;
using Manager.WebApp.Models;
using Manager.WebApp.Services;
using Manager.SharedLibs;
using MsSql.AspNet.Identity.Stores;
using Newtonsoft.Json;
using System.Collections.Generic;
using ApiJobMarket.DB.Sql.Entities;

namespace Manager.WebApp.Controllers
{
    public class SettingsController : BaseAuthedController
    {
        private readonly IStoreEmailServer _mainStore;
        private readonly ILog logger = LogProvider.For<SettingsController>();

        public SettingsController(IStoreEmailServer mainStore)
        {
            _mainStore = mainStore;
        }
        
        public ActionResult Email(ManageEmailSettingsModel model)
        {            
            try
            {
                var agencyId = GetCurrentAgencyId();
                var staffId = GetCurrentStaffId();

                model.EmailServers = CommonHelpers.GetListEmailServers(agencyId);
                model.InComing = new ManageEmailSettingItemModel();
                model.OutGoing = new ManageEmailSettingItemModel();

                var listEmails = CommonHelpers.GetListEmailSettings(agencyId, staffId);
                if (listEmails.HasData())
                {
                    foreach (var item in listEmails)
                    {
                        if(item.EmailType == (int)EnumEmailSettingTypes.InComing)
                        {                            
                            model.InComing.Email = item.Email;
                            model.InComing.EmailPassword = "xxxxxx";
                            model.InComing.EmailServerId = item.EmailServerId;
                            model.InComing.EmailType = (int)EnumEmailSettingTypes.InComing;

                            continue;
                        }

                        if (item.EmailType == (int)EnumEmailSettingTypes.OutGoing)
                        {
                            model.OutGoing.Email = item.Email;
                            model.OutGoing.EmailPassword = "xxxxxx";
                            model.OutGoing.EmailServerId = item.EmailServerId;
                            model.OutGoing.EmailType = (int)EnumEmailSettingTypes.OutGoing;

                            continue;
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed to display Email Settings because: " + ex.ToString());

                return View(model);
            }

            return View(model);
        }

        [HttpPost]
        [ActionName("Email")]
        public ActionResult Email_Post(ManageEmailSettingsModel model)
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
                //Extract data
                var settings = new List<IdentityEmailSetting>();
                var agencyId = GetCurrentAgencyId();
                var staffId = GetCurrentStaffId();

                if(model.InComing != null)
                {
                    var inComming = new IdentityEmailSetting();
                    inComming.Email = model.InComing.Email;

                    if (model.InComing.PasswordChanged)
                    {
                        var myEncryptedPwd = Utility.TripleDESEncrypt(model.InComing.Email, model.InComing.EmailPassword);
                        inComming.EmailPasswordHash = myEncryptedPwd;
                    }                        

                    inComming.PasswordChanged = model.InComing.PasswordChanged;

                    inComming.EmailServerId = model.InComing.EmailServerId;
                    inComming.EmailType = (int)EnumEmailSettingTypes.InComing;
                    inComming.AgencyId = agencyId;
                    inComming.StaffId = staffId;
                    inComming.TestingSuccessed = model.InComing.TestingSuccessed;

                    settings.Add(inComming);
                }

                if (model.OutGoing != null)
                {
                    var outGoing = new IdentityEmailSetting();
                    outGoing.Email = model.OutGoing.Email;

                    if (model.OutGoing.PasswordChanged)
                    {
                        var myEncryptedPwd = Utility.TripleDESEncrypt(model.OutGoing.Email, model.OutGoing.EmailPassword);
                        outGoing.EmailPasswordHash = myEncryptedPwd;
                    }

                    outGoing.PasswordChanged = model.OutGoing.PasswordChanged;

                    outGoing.EmailServerId = model.OutGoing.EmailServerId;
                    outGoing.EmailType = (int)EnumEmailSettingTypes.OutGoing;

                    outGoing.AgencyId = agencyId;
                    outGoing.StaffId = staffId;
                    outGoing.TestingSuccessed = model.OutGoing.TestingSuccessed;

                    settings.Add(outGoing);
                }

                var isSuccess = _mainStore.UpdateEmailSettings(settings);

                //Clear cache
                CachingHelpers.ClearCacheByKey(string.Format(EnumFormatInfoCacheKeys.EmailSettings, agencyId, staffId));

                this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed for update Email Settings: " + ex.ToString());

                //return View(model);
            }

            return RedirectToAction("Email");
        }

        #region Helpers

        private IdentityEmailServer ExtractEditFormData(EmailServerUpdateModel formData)
        {
            var myIdetity = new IdentityEmailServer();
            myIdetity.Id = formData.Id;

            myIdetity.Name = formData.Name;
            var smtpConfig = new IdentityEmailServerSMTPConfig();
            var popConfig = new IdentityEmailServerPOPConfig();

            if(formData.SendingConfig != null)
            {
                smtpConfig.SMTPServer = formData.SendingConfig.SMTPServer;
                smtpConfig.Port = formData.SendingConfig.Port;
                smtpConfig.SSLRequired = formData.SendingConfig.SSLRequired;
            }

            if (formData.ReceivingConfig != null)
            {
                popConfig.POPServer = formData.ReceivingConfig.POPServer;
                popConfig.Port = formData.ReceivingConfig.Port;
                popConfig.SSLRequired = formData.ReceivingConfig.SSLRequired;
            }

            myIdetity.SMTPConfig = JsonConvert.SerializeObject(smtpConfig);
            myIdetity.POPConfig = JsonConvert.SerializeObject(popConfig);
            myIdetity.TestingSuccessed = formData.TestingSuccessed;

            myIdetity.Status = formData.Status;
            myIdetity.AgencyId = GetCurrentAgencyId();
            myIdetity.StaffId = GetCurrentStaffId();

            return myIdetity;
        }

        private EmailServerUpdateModel RenderEditModel(IdentityEmailServer identity)
        {
            var editModel = new EmailServerUpdateModel();

            editModel.Id = identity.Id;
            editModel.Name = identity.Name;
            editModel.TestingSuccessed = identity.TestingSuccessed;
            var smtpConfig = JsonConvert.DeserializeObject<IdentityEmailServerSMTPConfig>(identity.SMTPConfig);
            var popConfig = JsonConvert.DeserializeObject<IdentityEmailServerPOPConfig>(identity.POPConfig);

            if(smtpConfig != null)
            {
                editModel.SendingConfig = new EmailServerSMTPModel();
                editModel.SendingConfig.SMTPServer = smtpConfig.SMTPServer;
                editModel.SendingConfig.Port = smtpConfig.Port;
                editModel.SendingConfig.SSLRequired = smtpConfig.SSLRequired;
            }

            if (popConfig != null)
            {
                editModel.ReceivingConfig = new EmailServerPOPModel();
                editModel.ReceivingConfig.POPServer = popConfig.POPServer;
                editModel.ReceivingConfig.Port = popConfig.Port;
                editModel.ReceivingConfig.SSLRequired = popConfig.SSLRequired;
            }

            editModel.Status = identity.Status;

            return editModel;
        }

        #endregion

    }
}