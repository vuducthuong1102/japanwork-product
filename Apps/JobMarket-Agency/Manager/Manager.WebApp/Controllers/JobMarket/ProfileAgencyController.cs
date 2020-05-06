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
    public class ProfileAgencyController : BaseAuthedController
    {
        private readonly IStoreEmailServer _mainStore;
        private readonly ILog logger = LogProvider.For<ProfileAgencyController>();

        public ProfileAgencyController(IStoreEmailServer mainStore)
        {
            _mainStore = mainStore;
        }

        [AccessRoleChecker(AgencyRequired = true)]
        public ActionResult Index()
        {
            ModelState.Clear();
            var model = new AgencyEditModel();

            try
            {
                var apiResult = AgencyServices.GetDetailAsync(new ApiAgencyModel { agency_id = GetCurrentAgencyId() }).Result;
                IdentityAgency info = null;

                if (apiResult != null && apiResult.value != null)
                {
                    info = JsonConvert.DeserializeObject<IdentityAgency>(apiResult.value.ToString());

                    if (info != null)
                    {
                        model.Name = info.agency;
                        model.Company = info.company_name;
                        model.Email = info.email;
                        model.Phone = info.phone;
                        model.Website = info.website;
                        model.Address = info.address;
                        model.LogoPath = info.logo_path;
                        model.LogoFullPath = info.logo_full_path;
                    }
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to display UpdateProfile because: {0}", ex.ToString()));
            }

            return View(model);
        }

        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        [AccessRoleChecker]
        public ActionResult UpdateProfileAgency(AgencyEditModel model)
        {
            try
            {
                var apiModel = new ApiAgencyModel();
                apiModel.agency_id = GetCurrentAgencyId();
                apiModel.agency = model.Name;
                apiModel.company_name = model.Company;
                //apiModel.email = model.Email;
                apiModel.phone = model.Phone;
                apiModel.website = model.Website;
                apiModel.address = model.Address;
                apiModel.logo_path = model.LogoPath;

                if (model.image_file_upload != null)
                {
                    var apiUploadReturned = AgencyServices.UploadLogoAsync(apiModel, model.image_file_upload).Result;

                    if (apiUploadReturned != null)
                    {
                        if (apiUploadReturned.value != null)
                        {
                            List<FileUploadResponseModel> images = JsonConvert.DeserializeObject<List<FileUploadResponseModel>>(apiUploadReturned.value.ToString());
                            if (images.HasData())
                            {
                                apiModel.logo_path = images[0].Path;
                            }
                        }
                    }
                }

                var apiReturned = AgencyServices.UpdateProfileAsync(apiModel).Result;

                var message = string.Empty;

                if (apiReturned.status == (int)HttpStatusCode.OK)
                {
                    if (apiReturned.error != null && !string.IsNullOrEmpty(apiReturned.error.error_code))
                    {
                        message = apiReturned.error.message;
                        logger.Error("Failed to UpdateProfile because: " + message);
                        this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);
                    }

                    this.AddNotification(ManagerResource.LB_UPDATE_SUCCESS, NotificationType.SUCCESS);

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed to UpdateProfile because: " + ex.ToString());
            }

            return View(model);
        }

        #region Helpers

        private IdentityEmailServer ExtractEditFormData(EmailServerUpdateModel formData)
        {
            var myIdetity = new IdentityEmailServer();
            myIdetity.Id = formData.Id;

            myIdetity.Name = formData.Name;
            var smtpConfig = new IdentityEmailServerSMTPConfig();
            var popConfig = new IdentityEmailServerPOPConfig();

            if (formData.SendingConfig != null)
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

            if (smtpConfig != null)
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