using System;
using System.Net;
using System.Web.Mvc;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Helpers;
using Manager.WebApp.Models;
using Manager.WebApp.Resources;
using Manager.WebApp.Settings;
using Manager.SharedLibs;
using System.Collections.Generic;
using System.Linq;
using Manager.WebApp.Services;
using Newtonsoft.Json;
using System.Threading.Tasks;
using ApiJobMarket.DB.Sql.Entities;
using Autofac;
using System.Dynamic;
using MsSql.AspNet.Identity.MsSqlStores;
using System.Web;

namespace Manager.WebApp.Controllers
{
    public class AgencyController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<AgencyController>();
        
        public AgencyController()
        {
           
        }
        
        [AllowAnonymous]
        public async Task<ActionResult> ActiveAccount(string token)
        {
            var result = EnumCommonCode.Success;
            var returnModel = new ResponseApiModel();

            CommonNotificationModel viewModel = new CommonNotificationModel();
            viewModel.IsSuccess = false;

            if (string.IsNullOrEmpty(token))
            {
                viewModel.Message = ManagerResource.COMMON_ERROR_DATA_INVALID;

                return View(viewModel);
            }

            try
            {
                var dataArr = token.Split('.');
                if (dataArr.Count() < 3)
                {
                    viewModel.Message = ManagerResource.COMMON_ERROR_DATA_INVALID;

                    return View(viewModel);
                }

                var rawData = Utility.DecryptText(dataArr[1], SystemSettings.EncryptKey);

                var userData = rawData.Split('|');
                if (userData.Count() >= 2)
                {
                    var info = new IdentityActiveAccount();
                    info.UserName = userData[0];
                    info.HashingData = token;

                    //var code = _userStore.ActiveAccountByEmail(info);
                    var store = GlobalContainer.IocContainer.Resolve<IStoreAgency>();

                    var code = store.ActiveAccountByEmail(info);
                    await Task.FromResult(code);

                    returnModel.Code = code;
                }

                if (returnModel.Code == 1)
                {
                    ModelState.Clear();
                    viewModel.IsSuccess = true;
                    viewModel.Message = ManagerResource.SUCCESS_ACCOUNT_ACTIVATED;
                    return View(viewModel);
                }
                else
                {
                    ModelState.Clear();
                    viewModel.IsSuccess = true;
                    viewModel.Message = ManagerResource.SUCCESS_ACCOUNT_ACTIVATED;
                    return View(viewModel);

                    ////Thong bao that bai
                    //viewModel.Message = ManagerResource.COMMON_ERROR_DATA_INVALID;
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying to ActiveAccount due to: {0}", ex.ToString());
                logger.Error(strError);

                viewModel.Message = ManagerResource.COMMON_ERROR_DATA_INVALID;
            }

            return View(viewModel);
        }

        [AllowAnonymous]
        public async Task<ActionResult> Verify(string token)
        {
            var result = EnumCommonCode.Success;
            var returnModel = new ResponseApiModel();

            CommonNotificationModel viewModel = new CommonNotificationModel();
            viewModel.IsSuccess = false;

            if (string.IsNullOrEmpty(token))
            {
                viewModel.Message = ManagerResource.COMMON_ERROR_DATA_INVALID;

                return View(viewModel);
            }

            try
            {
                var dataArr = token.Split('.');
                if (dataArr.Count() < 3)
                {
                    viewModel.Message = ManagerResource.COMMON_ERROR_DATA_INVALID;

                    return View(viewModel);
                }
                var encodeData = Server.UrlDecode(dataArr[1]);

                var rawData = Utility.DecryptText(dataArr[1], SystemSettings.EncryptKey);

                var userData = rawData.Split('|');
                if (userData.Count() >= 2)
                {
                    var info = new IdentityActiveAccount();
                    info.UserName = userData[0];
                    info.HashingData = token;

                    //var code = _userStore.ActiveAccountByEmail(info);
                    var store = GlobalContainer.IocContainer.Resolve<IStoreAgency>();

                    var code = store.ActiveAccountByEmail(info);
                    await Task.FromResult(code);

                    returnModel.Code = code;
                }

                if (returnModel.Code == 1)
                {
                    ModelState.Clear();
                    viewModel.IsSuccess = true;
                    viewModel.Message = ManagerResource.SUCCESS_ACCOUNT_ACTIVATED;
                    return View(viewModel);
                }
                else
                {
                    ModelState.Clear();
                    viewModel.IsSuccess = true;
                    viewModel.Message = ManagerResource.SUCCESS_ACCOUNT_ACTIVATED;
                    return View(viewModel);

                    ////Thong bao that bai
                    //viewModel.Message = ManagerResource.COMMON_ERROR_DATA_INVALID;
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying to ActiveAccount due to: {0}", ex.ToString());
                logger.Error(strError);

                viewModel.Message = ManagerResource.COMMON_ERROR_DATA_INVALID;
            }

            return View(viewModel);
        }

        [AllowAnonymous]
        public async Task<ActionResult> RecoverPassword(string token)
        {
            AccountRecoverPasswordModel model = new AccountRecoverPasswordModel();
            model.IsSuccess = false;
            model.Token = token;

            if (string.IsNullOrEmpty(token))
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_DATA_INVALID, NotificationType.ERROR);
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var dataArr = token.Split('.');
                if (dataArr.Count() < 3)
                {
                    this.AddNotification(ManagerResource.COMMON_ERROR_DATA_INVALID, NotificationType.ERROR);
                    return RedirectToAction("Login", "Account");
                }

                var rawData = Utility.DecryptText(dataArr[1], SystemSettings.EncryptKey);

                var userData = rawData.Split('|');
                if (userData.Count() >= 2)
                {
                    var info = new MsSql.AspNet.Identity.IdentityUser();
                    info.UserName = userData[0];
                    info.HashingData = token;

                    var store = GlobalContainer.IocContainer.Resolve<IStoreAgency>();

                    var userInfo = store.GetByUserName(info.UserName);
                    await Task.FromResult(userInfo);

                    if(userInfo != null)
                    {
                        if (string.IsNullOrEmpty(userInfo.SecurityStamp))
                        {
                            this.AddNotification(ManagerResource.COMMON_ERROR_DATA_INVALID, NotificationType.ERROR);
                            return RedirectToAction("Login", "Account");
                        }

                        if(token != userInfo.SecurityStamp)
                        {
                            this.AddNotification(ManagerResource.COMMON_ERROR_DATA_INVALID, NotificationType.ERROR);
                            return RedirectToAction("Login", "Account");
                        }

                        return View(model);
                    }
                    else
                    {
                        this.AddNotification(ManagerResource.COMMON_ERROR_DATA_INVALID, NotificationType.ERROR);
                        return RedirectToAction("Login", "Account");
                    }                   
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying to Display RecoverPassword Page due to: {0}", ex.ToString());
                logger.Error(strError);

                this.AddNotification(ManagerResource.COMMON_ERROR_DATA_INVALID, NotificationType.ERROR);
                return RedirectToAction("Login", "Account");
            }

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ActionName("RecoverPassword")]
        public async Task<ActionResult> RecoverPassword_Confirm(AccountRecoverPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage + x.Exception));

                this.AddNotification(messages, NotificationType.ERROR);
                return View(model);
            }

            if (string.IsNullOrEmpty(model.Token))
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_DATA_INVALID, NotificationType.ERROR);
                return View(model);
            }

            try
            {
                var dataArr = model.Token.Split('.');
                if (dataArr.Count() < 3)
                {
                    this.AddNotification(ManagerResource.COMMON_ERROR_DATA_INVALID, NotificationType.ERROR);
                    return View(model);
                }

                var rawData = Utility.DecryptText(dataArr[1], SystemSettings.EncryptKey);

                var userData = rawData.Split('|');
                if (userData.Count() >= 2)
                {
                    var info = new MsSql.AspNet.Identity.IdentityUser();
                    info.UserName = userData[0];
                    info.HashingData = model.Token;
                    info.PasswordHash = Utility.Md5HashingData(model.NewPassword);

                    var store = GlobalContainer.IocContainer.Resolve<IStoreAgency>();

                    var userInfo = store.GetByUserName(info.UserName);
                    await Task.FromResult(userInfo);

                    if (userInfo != null)
                    {
                        if (string.IsNullOrEmpty(userInfo.SecurityStamp))
                        {
                            this.AddNotification(ManagerResource.COMMON_ERROR_DATA_INVALID, NotificationType.ERROR);
                            return View(model);
                        }

                        if (info.HashingData != userInfo.SecurityStamp)
                        {
                            this.AddNotification(ManagerResource.COMMON_ERROR_DATA_INVALID, NotificationType.ERROR);
                            return View(model);
                        }

                        var result = store.ChangePassword(info);

                        this.AddNotification(ManagerResource.LB_CHANGE_PWD_SUCCESS, NotificationType.SUCCESS);
                        ViewBag.IsSuccess = true;

                        ModelState.Clear();

                        return View();
                    }
                    else
                    {
                        this.AddNotification(ManagerResource.COMMON_ERROR_DATA_INVALID, NotificationType.ERROR);
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying to RecoverPassword due to: {0}", ex.ToString());
                logger.Error(strError);

                this.AddNotification(ManagerResource.COMMON_ERROR_DATA_INVALID, NotificationType.ERROR);
                return View(model);
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            AgencyRegisterModel model = new AgencyRegisterModel();
            return View(model);
        }


        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Register")]
        public async Task<ActionResult> Register(AgencyRegisterModel model)
        {
            var message = string.Empty;
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
                var store = GlobalContainer.IocContainer.Resolve<IStoreAgency>();
                var hashingData = GenereateHashingForNewAccount(model.Email, model.Email);
                model.Password = Utility.Md5HashingData(model.Password);
                var user = new MsSql.AspNet.Identity.IdentityUser {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.Name,
                    PasswordHash = model.Password,
                    SecurityStamp = hashingData
                };

                var newId = store.CreateAccount(user);

                await Task.FromResult(newId);

                if(newId <= 0)
                {
                    this.AddNotification(ManagerResource.ERROR_ACCOUNT_DUPLICATED, NotificationType.ERROR);
                    return View(model);
                }
                else
                {
                    var apiModel = new ApiAgencyCreateModel();
                    apiModel.agency_id = newId;
                    apiModel.agency = model.Name;
                    apiModel.email = model.Email;
                    apiModel.company_name = model.Company;
                    apiModel.phone = model.Phone;
                    apiModel.website = model.Website;
                    apiModel.address = model.Address;

                    var apiResult = AgencyServices.CreateAsync(apiModel).Result;
                    if (apiResult != null)
                    {
                        if (apiResult.status == (int)HttpStatusCode.OK)
                        {                            
                            var sendMailStatus = SendEmailToActiveAccount(model, hashingData);

                            if (sendMailStatus == EnumCommonCode.Success)
                            {
                                message = string.Format(EmailResource.NOTIF_EMAIL_SENT_FORMAT, model.Email);
                                this.AddNotification(message, NotificationType.SUCCESS);

                                if (!Request.IsAuthenticated)
                                {
                                    return RedirectToAction("Login", "Account");
                                }
                                else
                                {
                                    return RedirectToAction("Register", "Agency");
                                }
                            }
                            else
                            {
                                message = EmailResource.ERROR_SENDING_EMAIL_FAILED;
                                this.AddNotification(message, NotificationType.ERROR);
                            }
                        }
                        else
                        {
                            if (apiResult.error != null && !string.IsNullOrEmpty(apiResult.error.error_code))
                            {
                                message = apiResult.error.message;

                                this.AddNotification(message, NotificationType.ERROR);
                            }                            
                        }
                    }                    
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to Register because: {0}", ex.ToString()));
            }

            return View(model);            
        }

        [AllowAnonymous]
        public ActionResult ResendEmailActive()
        {
            AgencyRequestSendEmailActiveModel model = new AgencyRequestSendEmailActiveModel();
            if (Request["email"] != null)
                model.Email = Request["email"].ToString();

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ActionName("ResendEmailActive")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResendEmailActive_Confirm(AgencyRequestSendEmailActiveModel model)
        {
            var message = string.Empty;
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
                var store = GlobalContainer.IocContainer.Resolve<IStoreAgency>();
                var userInfo = store.GetByUserName(model.Email);

                await Task.FromResult(userInfo);
                if(userInfo == null)
                {
                    this.AddNotification(ManagerResource.COMMON_ERROR_ACCOUNT_BY_EMAIL_NOT_FOUND, NotificationType.ERROR);
                }
                else
                {
                    if (userInfo.EmailConfirmed)
                    {
                        this.AddNotification(ManagerResource.LB_THIS_ACCOUNT_ALREADY_ACTIVED, NotificationType.ERROR);
                    }
                    else
                    {
                        var hashingData = GenereateHashingForNewAccount(model.Email, model.Email);
                        var sendMailStatus = ReSendEmailToActiveAccount(model, hashingData);

                        if (sendMailStatus == EnumCommonCode.Success)
                        {
                            store.ResendEmailActive(new IdentityActiveAccount
                            {
                                UserName = model.Email,
                                HashingData = hashingData
                            });

                            message = string.Format(EmailResource.NOTIF_EMAIL_SENT_FORMAT, model.Email);
                            this.AddNotification(message, NotificationType.SUCCESS);

                            return RedirectToAction("Login", "Account", new { UserName = model.Email });
                        }
                        else
                        {
                            message = EmailResource.ERROR_SENDING_EMAIL_FAILED;
                            this.AddNotification(message, NotificationType.ERROR);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to ResendEmailActive because: {0}", ex.ToString()));
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            AgencyRequestForgotPwdModel model = new AgencyRequestForgotPwdModel();
            if (Request["email"] != null)
                model.Email = Request["email"].ToString();

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ActionName("ForgotPassword")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword_Confirm(AgencyRequestForgotPwdModel model)
        {
            var message = string.Empty;
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
                var store = GlobalContainer.IocContainer.Resolve<IStoreAgency>();
                var userInfo = store.GetByUserName(model.Email);

                await Task.FromResult(userInfo);
                if (userInfo == null)
                {
                    this.AddNotification(ManagerResource.COMMON_ERROR_ACCOUNT_BY_EMAIL_NOT_FOUND, NotificationType.ERROR);
                    return View(model);
                }
                else
                {
                    if (!userInfo.EmailConfirmed)
                    {
                        this.AddNotification(ManagerResource.COMMON_ERROR_INACTIVE_ACCOUNT, NotificationType.ERROR);

                        return RedirectToAction("ActiveAccount", "Agency");
                    }

                    var hashingData = GenereateHashingForNewAccount(model.Email, model.Email);
                    var sendMailStatus = SendEmailForgotPassword(model, hashingData);

                    if (sendMailStatus == EnumCommonCode.Success)
                    {
                        dynamic info = new ExpandoObject();
                        info.UserName = model.Email;
                        info.HashingData = hashingData;

                        store.SendEmailRecover(info);

                        message = string.Format(EmailResource.NOTIF_EMAIL_FORGOT_PWD_SENT_FORMAT, model.Email);

                        this.AddNotification(message, NotificationType.SUCCESS);
                        ModelState.Clear();

                        return View();
                    }
                    else
                    {
                        message = EmailResource.ERROR_SENDING_EMAIL_FAILED;
                        this.AddNotification(message, NotificationType.ERROR);
                    }
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to SendEmailRecoverPassword because: {0}", ex.ToString()));
            }

            return View(model);
        }

        [AccessRoleChecker]
        public ActionResult Index(ManageAgencyModel model)
        {
            int currentPage = 1;
            int pageSize = SystemSettings.DefaultPageSize;

            if (string.IsNullOrEmpty(model.SearchExec))
            {
                model.SearchExec = "Y";
                if (!ModelState.IsValid)
                {
                    ModelState.Clear();
                }
            }

            if (Request["Page"] != null)
            {
                currentPage = Utils.ConvertToInt32(Request["Page"], 1);
            }

            var filter = GetFilterConfig();

            try
            {
                //model.SearchResults = _mainStore.GetByPage(filter);
                if (model.SearchResults.HasData())
                {
                    model.TotalCount = model.SearchResults[0].total_count;
                    model.CurrentPage = currentPage;
                    model.PageSize = pageSize;

                    List<int> listIds = new List<int>();
                    listIds = model.SearchResults.Select(x => x.agency_id).ToList();
                    var apiUserInfoModel = new ApiListUserInfoModel();
                    apiUserInfoModel.ListUserId = listIds;

                    var listUserReturned = CustomerAccountServices.GetListUserProfileAsync(apiUserInfoModel).Result;
                    if (listUserReturned.Data != null)
                    {
                        model.ListUsers = JsonConvert.DeserializeObject<List<dynamic>>(listUserReturned.Data.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed to get data because: " + ex.ToString());

                return View(model);
            }

            return View(model);
        }

        public ActionResult Create()
        {
            ModelState.Clear();
            var model = new AgencyEditModel();

            return View(model);
        }

        [AccessRoleChecker]
        public ActionResult UpdateProfile()
        {
            ModelState.Clear();
            var model = new AgencyEditModel();

            try
            {
                var apiResult = AgencyServices.GetDetailAsync(new ApiAgencyModel { agency_id = GetCurrentAgencyId() }).Result;
                IdentityAgency info = null;

                if(apiResult != null && apiResult.value != null)
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

        [HttpPost, ActionName("UpdateProfile")]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile_Confirm(AgencyEditModel model)
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

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error("Failed to UpdateProfile because: " + ex.ToString());
            }

            return View(model);
        }

        //Show popup confirm delete        
        //[AccessRoleChecker]
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return PartialView("_PopupDelete", id);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete_Confirm(int id)
        {
            var strError = string.Empty;
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                //var info = _mainStore.GetById(id, UserCookieManager.GetCurrentLanguageOrDefault());

                //if (info != null)
                //{
                //    _mainStore.Delete(id);
                //}
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete Agency because: " + ex.ToString());

                return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = "location.reload()" });
        }

        #region Helpers

        private string GenereateHashingForNewAccount(string userName, string email)
        {
            var recoverData = string.Format("{0}|{1}", userName, email);
            var dataEncrypt = Utility.EncryptText(recoverData, SystemSettings.EncryptKey);
            dataEncrypt = Server.UrlEncode(dataEncrypt);

            var rawData = string.Format("{0}.{1}.{2}",  Utility.Md5HashingData(DateTime.Now.ToString(Constant.DATEFORMAT_yyyyMMddHHmmss)), dataEncrypt, Utility.Md5HashingData(userName));

            return rawData;
        }

        private int SendEmailToActiveAccount(AgencyRegisterModel model, string hashingData)
        {
            var result = EnumCommonCode.Success;
            //Begin sending email
            var emailModel = new EmailModel
            {
                Sender = SystemSettings.EmailSender,
                SenderPwd = SystemSettings.EmailSenderPwd,
                Receiver = model.Email,
                Subject = EmailResource.SUBJECT_VERIFY_REGISTER
            };

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            var activeLink = string.Format("{0}/{1}/{2}?token={3}", baseUrl, "Agency", "Verify", hashingData);
            //var activeLink = string.Format("{0}/{1}/{2}?token={3}", SystemSettings.Email_ActiveLink, "Agency", "ActiveAccount", hashingData);

            model.ActiveLink = activeLink;

            emailModel.Body = PartialViewAsString("../EmailTemplates/ActiveAccount", model);
            emailModel.Attachments = new List<System.Net.Mail.Attachment>();
            var attFile = "~/Content/Docs/registration.docx";
            var attFilePath = System.Web.HttpContext.Current.Server.MapPath(attFile);

            if (System.IO.File.Exists(attFilePath))
            {
                emailModel.Attachments.Add(new System.Net.Mail.Attachment(attFilePath));
            }
            
            //if (!string.IsNullOrEmpty(emailModel.Body))
            //{
            //    emailModel.Body = emailModel.Body.Replace(EmailTemplateConst.Receiver, emailModel.Receiver);
            //    emailModel.Body = emailModel.Body.Replace(EmailTemplateConst.ActiveAccountLink, activeLink);
            //}

            var sendEmailResult = EmailHelpers.SendEmail(emailModel);
            if (!string.IsNullOrEmpty(sendEmailResult))
            {
                //Sending email was failed
                logger.Error(sendEmailResult);
                result = EnumCommonCode.Error;
            }

            return result;
        }

        private int ReSendEmailToActiveAccount(AgencyRequestSendEmailActiveModel model, string hashingData)
        {
            var result = EnumCommonCode.Success;
            //Begin sending email
            var emailModel = new EmailModel
            {
                Sender = SystemSettings.EmailSender,
                SenderPwd = SystemSettings.EmailSenderPwd,
                Receiver = model.Email,
                Subject = EmailResource.SUBJECT_VERIFY_REGISTER
            };

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            var activeLink = string.Format("{0}/{1}/{2}?token={3}", baseUrl, "Agency", "Verify", hashingData);
            //var activeLink = string.Format("{0}/{1}/{2}?token={3}", SystemSettings.Email_ActiveLink, "Agency", "ActiveAccount", hashingData);

            model.ActiveLink = activeLink;

            emailModel.Body = PartialViewAsString("../EmailTemplates/ResendActiveAccount", model);

            //if (!string.IsNullOrEmpty(emailModel.Body))
            //{
            //    emailModel.Body = emailModel.Body.Replace(EmailTemplateConst.Receiver, emailModel.Receiver);
            //    emailModel.Body = emailModel.Body.Replace(EmailTemplateConst.ActiveAccountLink, activeLink);
            //}

            var sendEmailResult = EmailHelpers.SendEmail(emailModel);
            if (!string.IsNullOrEmpty(sendEmailResult))
            {
                //Sending email was failed
                logger.Error(sendEmailResult);
                result = EnumCommonCode.Error;
            }

            return result;
        }

        private int SendEmailForgotPassword(AgencyRequestForgotPwdModel model, string hashingData)
        {
            var result = EnumCommonCode.Success;
            //Begin sending email
            var emailModel = new EmailModel
            {
                Sender = SystemSettings.EmailSender,
                SenderPwd = SystemSettings.EmailSenderPwd,
                Receiver = model.Email,
                Subject = EmailResource.SUBJECT_RECOVER_PASSWORD
            };

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            var activeLink = string.Format("{0}/{1}/{2}?token={3}", baseUrl, "Agency", "RecoverPassword", hashingData);
            
            model.ActiveLink = activeLink;

            emailModel.Body = PartialViewAsString("../EmailTemplates/RecoverPassword", model);

            var sendEmailResult = EmailHelpers.SendEmail(emailModel);
            if (!string.IsNullOrEmpty(sendEmailResult))
            {
                //Sending email was failed
                logger.Error(sendEmailResult);
                result = EnumCommonCode.Error;
            }

            return result;
        }

        #endregion

    }
}