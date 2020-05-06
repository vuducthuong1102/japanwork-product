using System;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Newtonsoft.Json;
using SingleSignOn.DB.Sql.Entities;
using SingleSignOn.DB.Sql.Stores;
using SingleSignOn.Helpers;
using SingleSignOn.Logging;
using SingleSignOn.Models;
using SingleSignOn.Resources;
using SingleSignOn.Services;
using SingleSignOn.Settings;

namespace SingleSignOn.Controllers
{
    /// <summary>
    /// This class will act as a base class which other Web API controllers will inherit from, 
    /// for now it will contain three basic methods
    /// </summary>

    public class BaseApiController : ApiController
    {
        class EmptyController : ControllerBase
        {
            protected override void ExecuteCore() { }
        }

        private readonly ILog logger = LogProvider.For<BaseApiController>();

        private IStoreDocumentApi _documentStore;
        public BaseApiController()
        {
            _documentStore = GlobalContainer.IocContainer.Resolve<IStoreDocumentApi>();
        }

        protected IHttpActionResult CreateBadRequest(string strErrorCode, string strErrorMessage)
        {
            var strError = strErrorCode + "-" + strErrorMessage;
            return CreateBadRequest(strError);
        }

        protected IHttpActionResult CreateBadRequest(string strErrorMessage)
        {
            logger.ErrorFormat("Return BadRequest: {0}", strErrorMessage);
            return BadRequest(strErrorMessage);
        }

        public void CreateDocumentApi(object ob)
        {
            if (SystemSettings.IsLogParamater)
            {
                string linkUrl = HttpContext.Current.Request.Url.AbsolutePath;

                string data = JsonConvert.SerializeObject(ob);
                _documentStore.Insert(linkUrl, data);
            }
        }

        public static string RenderViewToString(string controllerName,
                                        string viewName,
                                        object viewData)
        {
            var context = HttpContext.Current;
            var contextBase = new HttpContextWrapper(context);
            var routeData = new RouteData();
            routeData.Values.Add("controller", controllerName);

            var controllerContext = new ControllerContext(contextBase,
                                                          routeData,
                                                          new EmptyController());

            var razorViewEngine = new RazorViewEngine();
            var razorViewResult = razorViewEngine.FindView(controllerContext,
                                                           viewName,
                                                           "",
                                                           false);

            var writer = new StringWriter();
            var viewContext = new ViewContext(controllerContext,
                                              razorViewResult.View,
                                              new ViewDataDictionary(viewData),
                                              new TempDataDictionary(),
                                              writer);
            razorViewResult.View.Render(viewContext, writer);

            return writer.ToString();
        }

        public static string RenderPartialViewAsString(string partialName, object model)
        {
            var sw = new StringWriter();
            var httpContext = new HttpContextWrapper(HttpContext.Current);

            // point to an empty controller
            var routeData = new RouteData();
            routeData.Values.Add("controller", "EmptyController");

            var controllerContext = new ControllerContext(new RequestContext(httpContext, routeData), new EmptyController());

            var view = ViewEngines.Engines.FindPartialView(controllerContext, partialName).View;

            view.Render(new ViewContext(controllerContext, view, new ViewDataDictionary { Model = model }, new TempDataDictionary(), sw), sw);

            return sw.ToString();
        }

        protected string GenereateHashingForNewAccount(string userName, string email)
        {
            var recoverData = string.Format("{0}|{1}", userName, email);
            var dataEncrypt = Utility.EncryptText(recoverData, SystemSettings.EncryptKey);

            var rawData = string.Format("{0}.{1}.{2}", Utility.Md5HashingData(DateTime.Now.ToString(Constant.DATEFORMAT_yyyyMMddHHmmss)), dataEncrypt, Utility.Md5HashingData(userName));

            return rawData;
        }

        protected int SendEmailToActiveAccount(string email, string hashingData, int userId = 0)
        {
            var result = EnumCommonCode.Success;
            try
            {
                var hasTimeout = false;
                var emailModel = new EmailInputModel();
                emailModel.body = EmailHelper.GetEmailTemplate("ActiveAccount");

                var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
                var activeLink = string.Format("{0}/{1}/{2}?token={3}", SystemSettings.FrontendURL, "WebAuth", "ActiveAccount", hashingData);
                if (!string.IsNullOrEmpty(emailModel.body))
                {
                    emailModel.body = emailModel.body.Replace(EmailTemplateConst.Receiver, email);
                    emailModel.body = emailModel.body.Replace(EmailTemplateConst.ActiveAccountLink, activeLink);
                }
                emailModel.sendto = email;
                emailModel.subject = EmailResource.SUBJECT_VERIVY_REGISTER;

                var emailStore = GlobalContainer.IocContainer.Resolve<IStoreSystemEmail>();
                var emailInfo = new IdentitySystemEmail();
                emailInfo.Subject = emailModel.subject;
                emailInfo.Body = emailModel.body;
                emailInfo.Sender = string.Empty;
                emailInfo.Receiver = email;
                emailInfo.ReceiverId = userId;
                emailInfo.Action = ActionType.ActiveAccount;

                //Create new email
                emailStore.Insert(emailInfo);

                EmailService.Send(emailModel, ref hasTimeout);
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to Send Email to user [0] due to {1}", email, ex.Message);
                logger.Error(strError);
                result = EnumCommonCode.Error;
            }

            return result;
        }

        protected int SendEmailToActiveAccount_WithoutAmazon_old(string email, string hashingData)
        {
            var result = EnumCommonCode.Success;
            //Begin sending email
            var emailModel = new EmailModel
            {
                Sender = SystemSettings.EmailSender,
                SenderPwd = SystemSettings.EmailSenderPwd,
                Receiver = email,
                Subject = EmailResource.SUBJECT_VERIVY_REGISTER
            };

            emailModel.Body = EmailHelper.GetEmailTemplate("ActiveAccount");

            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            baseUrl = baseUrl.Replace("http://", "https://");

            var activeLink = string.Format("{0}/{1}/{2}?token={3}", SystemSettings.Email_ActiveLink, "WebAuth", "ActiveAccount", hashingData);
            if (!string.IsNullOrEmpty(emailModel.Body))
            {
                emailModel.Body = emailModel.Body.Replace(EmailTemplateConst.Receiver, emailModel.Receiver);
                emailModel.Body = emailModel.Body.Replace(EmailTemplateConst.ActiveAccountLink, activeLink);
            }

            var sendEmailResult = EmailHelper.SendEmail(emailModel);
            if (!string.IsNullOrEmpty(sendEmailResult))
            {
                //Sending email was failed
                logger.Error(sendEmailResult);
                result = EnumCommonCode.Error;
            }

            return result;
        }

        protected int SendEmailToActiveAccount_WithoutAmazon(string email, string hashingData)
        {
            var result = EnumCommonCode.Success;
            //Begin sending email
            var emailModel = new EmailModel
            {
                Sender = SystemSettings.EmailSender,
                SenderPwd = SystemSettings.EmailSenderPwd,
                Receiver = email,
                Subject = EmailResource.SUBJECT_VERIVY_REGISTER
            };

            emailModel.ActiveLink = string.Format("{0}/{1}?token={2}", SystemSettings.Email_ActiveLink, "Verify", hashingData);

            logger.Debug(string.Format("Active link: {0}", emailModel.ActiveLink));

            emailModel.Body = RenderPartialViewAsString("../EmailTemplates/ActiveAccount", emailModel);

            var sendEmailResult = EmailHelper.SendEmail(emailModel);
            if (!string.IsNullOrEmpty(sendEmailResult))
            {
                //Sending email was failed
                logger.Error(sendEmailResult);
                result = EnumCommonCode.Error;
            }

            return result;
        }

        protected int BeginSendEmailToRecoverPwd(int userId, string email, string pwdType)
        {
            var result = EnumCommonCode.Success;
            try
            {
                var userStore = GlobalContainer.IocContainer.Resolve<IStoreUser>();

                var recoverData = string.Format("{0}|{1}", userId, pwdType);
                var dataEncrypt = Utility.EncryptText(recoverData, SystemSettings.EncryptKey);
                var hashingData = string.Format("{0}.{1}.{2}", Utility.Md5HashingData(DateTime.Now.ToString(Constant.DATEFORMAT_yyyyMMddHHmmss)), dataEncrypt, Utility.Md5HashingData(userId.ToString()));

                //Begin sending email
                var emailModel = new EmailModel
                {
                    Sender = SystemSettings.EmailSender,
                    SenderPwd = SystemSettings.EmailSenderPwd,
                    Receiver = email,
                    Subject = EmailResource.SUBJECT_RECOVER_PASSWORD
                };

                emailModel.ActiveLink = string.Format("{0}/{1}/{2}?token={3}", SystemSettings.Email_ActiveLink, "WebAuth", "RecoverPassword", hashingData);

                emailModel.Body = RenderPartialViewAsString("../EmailTemplates/RecoverPassword", emailModel);

                //var emailStore = GlobalContainer.IocContainer.Resolve<IStoreSystemEmail>();
                //var emailInfo = new IdentitySystemEmail();
                //emailInfo.Subject = EmailResource.SUBJECT_RECOVER_PASSWORD;
                //emailInfo.Body = emailModel.Body;
                //emailInfo.Sender = string.Empty;
                //emailInfo.Receiver = email;
                //emailInfo.ReceiverId = userId;
                //emailInfo.Action = (pwdType == PasswordLevelType.Level1) ? ActionType.RecoverPassword1 : ActionType.RecoverPassword2;

                ////Create new email
                //emailStore.Insert(emailInfo);

                userStore.SendEmailRecoverPassword(new IdentityUser
                {
                    UserName = email
                }, hashingData);


                var sendEmailResult = EmailHelper.SendEmail(emailModel);
                if (!string.IsNullOrEmpty(sendEmailResult))
                {
                    //Sending email was failed
                    logger.Error(sendEmailResult);
                    result = EnumCommonCode.Error;
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to Send Email to user [0] due to {1}", email, ex.Message);
                logger.Error(strError);
                result = EnumCommonCode.Error;
            }

            return result;
        }
    }

    class EmptyController : ControllerBase
    {
        protected override void ExecuteCore() { }
    }
}