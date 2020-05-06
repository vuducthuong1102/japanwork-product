using System;
using System.Web.Mvc;

using SingleSignOn.Logging;
using SingleSignOn.DB.Sql.Stores;
using SingleSignOn.Models;
using System.Threading.Tasks;
using SingleSignOn.Services;
using SingleSignOn.DB.Sql.Entities;
using Newtonsoft.Json;
using SingleSignOn.Helpers;
using CaptchaMvc.HtmlHelpers;
using SingleSignOn.Settings;
using SingleSignOn.Extensions;
using SingleSignOn.Resources;
using SingleSignOn.Caching;
using System.Linq;
using CaptchaMvc.Attributes;
using SingleSignOn.ShareLibs;

namespace SingleSignOn.Controllers
{    
    public class WebAuthController : BaseController
    {
        private readonly ILog logger = LogProvider.For<WebAuthController>();
        private IWebAuthRedisService _redisService;
        private readonly IStoreUser _userStore;

        public WebAuthController(IWebAuthRedisService redisService, IStoreUser userStore)
        {
            _redisService = redisService;
            _userStore = userStore;
        }

        /// <summary>
        /// GET: /WebAuth/Login
        /// </summary>
        [AllowAnonymous]
        public ActionResult Login(string urlReturn)
        {
            if (UserCookieManager.IsAuthenticated())
            {
                ResponseWebAuthLoginModel redirectModel = new ResponseWebAuthLoginModel();
                redirectModel.URLReturn = urlReturn;
                try
                {
                    var userIdentity = UserCookieManager.GetCookie<UserCookieModel>(SingleSignOnSettings.SSOCommonUserKey);
                    if (userIdentity != null)
                    {
                        redirectModel.Data.Id = userIdentity.UserId;
                        redirectModel.TokenKey = userIdentity.TokenKey;
                        return RedirectToLocal(redirectModel);
                    }
                }
                catch(Exception ex)
                {
                    var strError = string.Format("Failed when trying to Login due to: {0}", ex.ToString());
                    logger.Error(strError);
                    this.AddNotification(UserWebResource.COMMON_EXCEPTION_NOTIF, NotificationType.ERROR);
                }
            }

            WebAuthLoginViewModel model = new WebAuthLoginViewModel();
            model.URLReturn = urlReturn;
            model.NumberOfFailedLogins = Convert.ToInt32(Session["LoginTimesFailed"]);
            return View(model);
        }


        /// <summary>
        /// POST: /WebAuth/Login
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(WebAuthLoginViewModel model, string urlReturn)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            model.NumberOfFailedLogins = Convert.ToInt32(Session["LoginTimesFailed"]);
            if (model.NumberOfFailedLogins >= SingleSignOnSettings.NumberOfFailedLoginsToShowCaptcha)
            {
                var captchaMessage = UserWebResource.COMMON_ERROR_CAPTCHA_INVALID;
                if (!this.IsCaptchaValid(captchaMessage))
                {
                    //ModelState.AddModelError("", captchaMessage);
                    this.AddNotification(captchaMessage, NotificationType.ERROR);
                    return View(model);
                }
            }

            model = ReFormatLoginModel(model);
            var currentDomain = Request.Url.Host;
            var returnModel = new ResponseWebAuthLoginModel();
            try
            {
                logger.Debug("Begin LOGIN-WEB request");

                //Extract data from cache first
                var redisKey = model.UserName + model.Password;
                var cachedData = await _redisService.GetWebAuthLoginModelAsync(redisKey, currentDomain);
                if (cachedData == null)
                {
                    //Get from database
                    var webLoginIdentity = _userStore.WebLogin(new IdentityUser { UserName = model.UserName, PasswordHash = model.Password, Domain = currentDomain });
                    if (webLoginIdentity != null)
                    {

                        var needCreateToken = ((string.IsNullOrEmpty(webLoginIdentity.TokenKey) || (!string.IsNullOrEmpty(webLoginIdentity.TokenKey) && webLoginIdentity.TokenExpiredDate != null && webLoginIdentity.TokenExpiredDate < DateTime.Now)));
                        //If token doesn't exists
                        if (needCreateToken)
                        {
                            //Generate tokenkey when the input data is valid
                            var newTokenKey = GenerateTokenKey(model.UserName);
                            returnModel.TokenKey = newTokenKey;
                            webLoginIdentity.TokenKey = newTokenKey;

                            var gentokenResult = _userStore.ProvideTokenKeyForUser(new UserTokenIdentity { UserId = webLoginIdentity.Id, TokenKey = newTokenKey, Domain = currentDomain, Method = ActionMethod.Web });
                            if (gentokenResult)
                            {
                                //Login successfully
                                var logInfo = GetDataForLogging(model);
                                logInfo.ActionDesc = string.Format("User [{0}] logged in successfully with token [{1}]", webLoginIdentity.Id, returnModel.TokenKey);
                                logInfo.Domain = currentDomain;
                                logInfo.ActionType = ActionType.Login;

                                //Write log after gentoken successfully
                                _userStore.WriteUserLog(logInfo, SingleSignOnSettings.AllowToTraceLoginRequest);

                                var dtNow = DateTime.Now;
                                webLoginIdentity.TokenCreatedDate = dtNow;
                                if(webLoginIdentity.LoginDurations > 0)
                                {
                                    webLoginIdentity.TokenExpiredDate = dtNow.AddMinutes(webLoginIdentity.LoginDurations);
                                }
                                else
                                {
                                    webLoginIdentity.TokenExpiredDate = dtNow.AddMinutes(SingleSignOnSettings.DefaultCachedTimeout);
                                }
                                
                            }
                        }
                        else
                        {
                            returnModel.TokenKey = webLoginIdentity.TokenKey;
                        }

                        returnModel.Data = webLoginIdentity;                        
                        returnModel.Code = EnumCommonCode.Success;
                        returnModel.Msg = UserWebResource.LOGIN_SUCCESS;

                        //Save to cache after login successfully
                        await _redisService.SetResponseWebAuthLoginModelAsync(returnModel, redisKey, currentDomain);
                    }
                    else
                    {
                        returnModel.Code = EnumLoginStatus.LoginErrorUserInfoIncorrect;
                        returnModel.Msg = UserWebResource.LOGIN_ERROR_INFO_INVALID;
                    }

                    //Save to cache
                    //await _redisService.SetResponseWebAuthLoginModelAsync(returnModel, redisKey, currentDomain);
                }
                else
                {
                    returnModel = cachedData;                   
                }

                logger.DebugFormat("LOGIN-WEB Returned Model encrypted = {0}", JsonConvert.SerializeObject(returnModel));

                if (returnModel.Code == EnumCommonCode.Success)
                {
                    returnModel.URLReturn = urlReturn;
                    Session["LoginTimesFailed"] = 0;

                    //Save user datato session
                    //Session[Constant.WEB_USER_LOGIN_SESSION_KEY] = returnModel.Data;
                    var cookieModel = new UserCookieModel { UserId = returnModel.Data.Id, TokenKey = returnModel.Data.TokenKey, OTPType = returnModel.Data.OTPType };
                    UserCookieManager.SetCookie(SingleSignOnSettings.SSOCommonUserKey, cookieModel, returnModel.Data.LoginDurations);

                    return RedirectToLocal(returnModel);
                }
                else
                {
                    model.NumberOfFailedLogins++;
                    Session["LoginTimesFailed"] = model.NumberOfFailedLogins;
                    //ModelState.AddModelError("", returnModel.Msg);
                    this.AddNotification(returnModel.Msg, NotificationType.ERROR);
                    return View(model);
                } 
            }
            catch (Exception ex)
            {
                var rawModel = JsonConvert.SerializeObject(model);
                var strError = string.Format("Failed for LOGIN-WEB request: {0} because: {1}", rawModel, ex.Message);
                logger.Error(strError);
                //ModelState.AddModelError("", string.Format("Failed for LOGIN-WEB request because: {0}", ex.Message));
                this.AddNotification(UserWebResource.COMMON_EXCEPTION_NOTIF, NotificationType.ERROR);
                return View(model);
            }
            finally
            {
                logger.Debug("Ended LOGIN-WEB request");
            }             
        }

        public ActionResult Logout()
        {
            var currentDomain = Request.Url.Host;
            //ClearLoginDataCached(currentDomain);
            //UserSessionManager.ClearSessionData();
            UserCookieManager.ClearCookie(SingleSignOnSettings.SSOCommonUserKey);

            //var currentRefer = string.Empty;
            //if(Request.UrlReferrer != null)
            //{
            //    //currentRefer = Request.UrlReferrer.ToString();
            //    currentRefer = String.Format("{0}{1}{2}{3}", Request.UrlReferrer.Scheme,
            //        Uri.SchemeDelimiter, Request.UrlReferrer.Authority, Request.UrlReferrer.AbsolutePath);
            //    return Redirect(currentRefer);
            //}

            return RedirectToAction("Login", "WebAuth");
        }

        public ActionResult Register()
        {
            return View();
        }

        [ActionName("Register")]
        [HttpPost]
        public ActionResult Register(WebAccountRegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage + x.Exception));
                //this.AddNotification(messages, NotificationType.ERROR);
                return View(model);
            }

            var captchaMessage = UserWebResource.COMMON_ERROR_CAPTCHA_INVALID;
            if (!this.IsCaptchaValid(captchaMessage))
            {
                //ModelState.AddModelError("", captchaMessage);
                this.AddNotification(captchaMessage, NotificationType.ERROR);
                return View(model);
            }

            model = ReFormatRegisterModel(model);
            var validateMsg = string.Empty;
            var currentDomain = Request.Url.Host;
            var errorCode = ValidateRegisterData(model, ref validateMsg);

            var isValid = (errorCode == EnumCommonCode.Success || errorCode == 0);
            try
            {
                logger.Debug("Begin REGISTER-WEB request");
                logger.DebugFormat("REGISTER-WEB Model encrypted = {0}", JsonConvert.SerializeObject(model));
                if (isValid)
                {
                    var birthday = (model.Birthday == null)? DateTime.Now : DateTime.ParseExact(model.Birthday, Constant.DATE_FORMAT_ddMMyyyy, null);
                    var sex = UserSex.Male;
                    try
                    {
                        sex = Convert.ToInt32(model.Sex);
                    }
                    catch
                    {
                    }

                    var code = EnumCommonCode.Success;

                    var userIdentity = new IdentityUser
                    {
                        UserName = model.UserName,
                        PasswordHash = model.Password,
                        Email = model.Email,
                        Birthday = birthday,
                        Sex = sex,
                        Address = model.Address,
                        FullName = model.Full_Name,
                        DisplayName = model.Display_Name,
                        IDCard = model.CMTND,
                        PhoneNumber = model.Phone,
                        Note = model.Note,
                        OTPType = OTPType.OTPSMS
                    };

                    //Check and register from database
                    var result = _userStore.WebRegister(userIdentity, ref code);

                    if (result > 0)
                    {
                        //Generate tokenkey when user was created successfully
                        //var newTokenKey = GenerateTokenKey(model.UserName);

                        //var gentokenResult = _userStore.ProvideTokenKeyForUser(new UserTokenIdentity { UserId = newUserId, TokenKey = newTokenKey, Domain = currentDomain, Method = ActionMethod.Web });
                        //if (gentokenResult)
                        {
                            //Write log successfully
                            var logInfo = GetDataForLogging(model);
                            logInfo.Domain = currentDomain;
                            logInfo.ActionType = ActionType.Register;
                            logInfo.ActionDesc = string.Format("User [{0}] was registered successfully", result);

                            //Write log 
                            _userStore.WriteUserLog(logInfo, SingleSignOnSettings.AllowToTraceLoginRequest);
                        }

                        this.AddNotification(UserWebResource.REGISTER_SUCCESS, NotificationType.SUCCESS);
                        ModelState.Clear();

                        return View(new WebAccountRegisterModel());
                    }
                    else
                    {
                        this.AddNotification(GetRegisterResponseMessage(code), NotificationType.ERROR);
                    }
                }
                else
                {
                    this.AddNotification(validateMsg, NotificationType.ERROR);
                }         
            }
            catch (Exception ex)
            {
                string strError = "Failed for REGISTER-WEB request: " + ex.Message;
                logger.ErrorException(strError, ex);
                this.AddNotification(UserWebResource.COMMON_EXCEPTION_NOTIF, NotificationType.ERROR);
            }
            finally
            {
                logger.Debug("Ended REGISTER-WEB request");
            }

            return View(model);
        }

        public ActionResult RecoverPassword()
        {
            var model = new WebRecoverPasswordModel();
            return View(model);
        }

        [ActionName("RecoverPassword")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecoverPassword_Post(WebRecoverPasswordModel model)
        {
            var result = string.Empty;
            var isSuccess = false;
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage + x.Exception));
                this.AddNotification(messages, NotificationType.ERROR);
                return View(model);
            }

            var captchaMessage = UserWebResource.COMMON_ERROR_CAPTCHA_INVALID;
            if (!this.IsCaptchaValid(captchaMessage))
            {
                ModelState.AddModelError("", captchaMessage);
                return View(model);
            }

            try
            {
                IdentityUser userIdentity = null;
                if(model.RecoverMethod == RecoverPasswordMethod.EMAIL)
                {
                    if (string.IsNullOrEmpty(model.Answer))
                    {
                        this.AddNotification(UserWebResource.COMMON_ERROR_EMAIL_NULL, NotificationType.ERROR);
                        return View(model);
                    }
                        
                    userIdentity = _userStore.GetUserByEmail(model.Answer);
                    if (userIdentity != null)
                    {
                        _userStore.RecoverPasswordStep1(userIdentity.Id, Utility.Md5HashingData(model.NewPassword));

                        var sendMailStatus = SendEmailToRecoverPwd(userIdentity.Id, userIdentity.Email, model.PasswordType);
                        if (sendMailStatus == EnumCommonCode.Success)
                        {
                            isSuccess = true;
                            result = string.Format(EmailResource.NOTIF_EMAIL_SENT_FORMAT, userIdentity.Email);
                        }
                        else
                        {
                            result = EmailResource.ERROR_SENDING_EMAIL_FAILED;
                        }
                    }
                    else
                    {
                        result = UserWebResource.COMMON_ERROR_EMAIL_INVALID;
                    }
                }

                if (isSuccess)
                {
                    this.AddNotification(result, NotificationType.SUCCESS);

                    ClearLoginDataCached();
                    UserCookieManager.ClearCookie(SingleSignOnSettings.SSOCommonUserKey);                    

                    ModelState.Clear();
                }
                else
                    this.AddNotification(result, NotificationType.ERROR);
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying to RecoverPassword due to: {0}", ex.ToString());
                logger.Error(strError);
                this.AddNotification(UserWebResource.COMMON_EXCEPTION_NOTIF, NotificationType.ERROR);
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Test(string token)
        {

            var recoverData = string.Format("{0}|{1}", "ae96d260-7c1b-4c36-9b15-530ca0080b9b", "level1");
            var dataEncrypt = Utility.EncryptText(recoverData, "VId123#@!");

            var plainText = Utility.DecryptText(dataEncrypt, "VId123#@!");

            //var rawData = Utility.EncryptText(SystemSettings.EncryptKey, token);
            //var userData = rawData.Split('|');
            var result = "Encrypted: " + dataEncrypt + "<br />" + "Plain text: " + recoverData + "<br />";

            if (!string.IsNullOrEmpty(token))
            {
                var myRawData = Utility.DecryptText(token, SystemSettings.EncryptKey);
                result += "Token: " + token + "----> Result: " + myRawData;

                UserCookieManager.ClearCookie(SingleSignOnSettings.SSOCommonUserKey);
                ClearLoginDataCached();
            }


            return Content("Encrypted: " + dataEncrypt + "<br />" + "Plain text: " + recoverData);
        }

        [HttpGet]
        public ActionResult Password_Reset(string token)
        {
            var result = EnumCommonCode.Success;
            if (string.IsNullOrEmpty(token))
            {
                this.AddNotification(UserWebResource.COMMON_ERROR_DATA_INVALID, NotificationType.ERROR);
            }

            try
            {
                var dataArr = token.Split('.');
                if(dataArr.Count() < 3)
                {
                    this.AddNotification(UserWebResource.COMMON_ERROR_DATA_INVALID, NotificationType.ERROR);
                    return View();
                }

                //var rawData = Utility.TripleDESDecrypt(SystemSettings.EncryptKey, dataArr[1]);
                var rawData = Utility.DecryptText(dataArr[1], SystemSettings.EncryptKey);

                var userData = rawData.Split('|');
                if (userData.Count() >= 2)
                {
                   result = _userStore.RecoverPasswordStep2(Utils.ConvertToInt32(userData[0]), userData[1]);
                }

                if(result != EnumCommonCode.Success)
                {
                    if(result == EnumCommonCode.Error_Info_NotFound)
                    {
                        this.AddNotification(UserWebResource.COMMON_ERROR_INFO_NOTFOUND, NotificationType.ERROR);
                    }
                    else
                    {
                        this.AddNotification(UserWebResource.COMMON_ERROR_DATA_INVALID, NotificationType.ERROR);
                    }

                    return View();
                }
                else
                {
                    //Login successfully
                    var logInfo = GetDataForLogging(token);
                    logInfo.ActionDesc = string.Format("User [{0}] was recovered password ({1}) successfully", userData[0], userData[1]);
                    logInfo.Domain = Request.Url.Host;
                    logInfo.ActionType = (userData[1] == "level1" ? ActionType.RecoverPassword1 : ActionType.RecoverPassword2);

                    //Write log
                    _userStore.WriteUserLog(logInfo, SingleSignOnSettings.AllowToTraceLoginRequest);

                    ClearLoginDataCached();
                    UserCookieManager.ClearCookie(SingleSignOnSettings.SSOCommonUserKey);                    

                    this.AddNotification(UserWebResource.COMMON_CHANGEPWD_SUCCESS, NotificationType.SUCCESS);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying to ResetPassword due to: {0}", ex.ToString());
                logger.Error(strError);
                this.AddNotification(UserWebResource.COMMON_ERROR_DATA_INVALID, NotificationType.ERROR);
            }
            return View();
        }

        #region Helpers
        private WebAuthLoginViewModel ReFormatLoginModel(WebAuthLoginViewModel model)
        {
            if (model != null)
            {
                model.UserName = model.UserName.Trim();
                model.Password = Utility.Md5HashingData(model.Password);
            }

            return model;
        }

        public UserLogIdentity GetDataForLogging(object model)
        {
            var loginIdentity = new UserLogIdentity();
            loginIdentity.UserIp = Request.UserHostAddress;
            loginIdentity.UserAgent = Request.UserAgent;
            loginIdentity.Method = ActionMethod.Web;
            try
            {
                loginIdentity.RawData = model.ToJson();
            }
            catch (Exception ex)
            {
                loginIdentity.RawData = string.Format("Could not parse the data to Json: {0}", ex.Message);
            }

            return loginIdentity;
        }

        public string GenerateTokenKey(string userName)
        {
            var tokenKey = string.Empty;
            var clientIp = Request.UserHostAddress;
            var dtNow = DateTime.Now.ToString(Constant.DATEFORMAT_yyyyMMddHHmmss);

            //Rule: MD5(< username >|< DateTime.Now.ToString(yyyyMMddHHmmsss) >|< clientIP >
            var rawTokenStr = string.Format("{0}|{1}|{2}", userName, dtNow, clientIp);

            tokenKey = Utility.Md5HashingData(rawTokenStr);

            return tokenKey;
        }

        private ActionResult RedirectToLocal(ResponseWebAuthLoginModel model)
        {            
            var parmsList = "?UserId={0}&Token={1}";
            if(model.Data != null)
            {
                parmsList = string.Format(parmsList, model.Data.Id, model.TokenKey);
            }

            if (!string.IsNullOrEmpty(model.URLReturn))
            {
                try
                {
                    if (!Url.IsLocalUrl(model.URLReturn))
                    {
                        Uri uri = new Uri(model.URLReturn);
                        model.URLReturn = uri.GetLeftPart(UriPartial.Path);
                    }                   
                }
                catch
                {
                }
                model.URLReturn = model.URLReturn + parmsList;
                return Redirect(model.URLReturn);
            }
            return RedirectToAction("Index", "Home");
        }

        private WebAccountRegisterModel ReFormatRegisterModel(WebAccountRegisterModel model)
        {
            if (model != null)
            {
                model.UserName = model.UserName.Trim();
                model.Email = model.Email.Trim();
                if (model.CMTND != null) model.CMTND = model.CMTND.Trim();
                model.Password = Utility.Md5HashingData(model.Password);
                //model.ConfirmPassword = Utility.Md5HashingData(model.Password);
            }

            return model;
        }

        private int ValidateRegisterData(WebAccountRegisterModel model, ref string message)
        {
            var errorCode = 0;
            if (errorCode == 0)
            {
                if (!string.IsNullOrEmpty(model.Birthday))
                {
                    try
                    {
                        var dt = DateTime.ParseExact(model.Birthday, Constant.DATE_FORMAT_ddMMyyyy, null);
                    }
                    catch
                    {
                        errorCode = EnumCommonCode.ErrorTimeInvalid;
                        message = UserWebResource.REGISTER_ERROR_BIRTHDAY_INVALID;
                        ModelState.AddModelError("Birthday", message);
                    }
                }
            }
            return errorCode;
        }

        private string GetRegisterResponseMessage(int statusCode)
        {
            var message = string.Empty;
            if (statusCode == EnumCommonCode.Success)
            {
                message = UserWebResource.REGISTER_SUCCESS;
            }
            else if (statusCode == EnumCommonCode.Error_UserNameAlreadyUsed)
            {
                message = UserWebResource.REGISTER_ERROR_USERNAME_USED;
                ModelState.AddModelError("UserName", message);
            }
            else if (statusCode == EnumCommonCode.Error_EmailAlreadyUsed)
            {
                message = UserWebResource.REGISTER_ERROR_EMAIL_USED;
                ModelState.AddModelError("Email", message);
            }
            else if (statusCode == EnumCommonCode.Error_PhoneAlreadyUsed)
            {
                message = UserWebResource.REGISTER_ERROR_PHONE_USED;
                ModelState.AddModelError("Phone", message);
            }
            else if (statusCode == EnumCommonCode.Error_IDCardAlreadyUsed)
            {
                message = UserWebResource.REGISTER_ERROR_IDCARD_USED;
                ModelState.AddModelError("IDCard", message);
            }

            return message;
        }

        private void ClearLoginDataCached(string domain = "")
        {
            if(string.IsNullOrEmpty(domain)) domain = Request.Url.Host;
            if (UserCookieManager.IsAuthenticated())
            {
                try
                {
                    var userLogin = UserCookieManager.GetCookie<UserCookieModel>(SingleSignOnSettings.SSOCommonUserKey);
                    var currentUser = _userStore.GetUserById(userLogin.UserId);

                    if (currentUser != null)
                    {
                        _redisService.ClearCachedDataByKeyAndDomain(currentUser.UserName + currentUser.PasswordHash, domain);
                        _redisService.ClearCachedDataByKeyAndDomain(currentUser.Email + currentUser.PasswordHash, domain);
                        _redisService.ClearCachedDataByKeyAndDomain(currentUser.PhoneNumber + currentUser.PasswordHash, domain);
                    }
                }
                catch (Exception ex)
                {
                    var strError = "Could not ClearLoginCacheData in web due to: {0}";
                    strError = string.Format(ex.Message);
                }
            }
            
        }

        private int SendEmailToRecoverPwd(int userId, string email, string pwdType)
        {
            var result = EnumCommonCode.Success;
            //Begin sending email
            var emailModel = new EmailModel
            {
                Sender = SystemSettings.EmailSender,
                SenderPwd = SystemSettings.EmailSenderPwd,
                Receiver = email,
                Subject = EmailResource.SUBJECT_RECOVER_PASSWORD
            };
            emailModel.Body = EmailHelper.GetEmailTemplate("RecoverPassword");

            var recoverData = string.Format("{0}|{1}", userId, pwdType);
            //var dataEncrypt = Utility.TripleDESEncrypt(SystemSettings.EncryptKey, recoverData);
            var dataEncrypt = Utility.EncryptText(recoverData, SystemSettings.EncryptKey);

            var rawData = string.Format("{0}.{1}.{2}", Utility.Md5HashingData(DateTime.Now.ToString(Constant.DATEFORMAT_yyyyMMddHHmmss)), dataEncrypt, Utility.Md5HashingData(userId.ToString()));

            var baseUrl = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);
            var recoverLink = string.Format("{0}/{1}/{2}?token={3}", baseUrl, "WebAuth", "Password_Reset", rawData);
            if (!string.IsNullOrEmpty(emailModel.Body))
            {
                emailModel.Body = emailModel.Body.Replace(EmailTemplateConst.Receiver, emailModel.Receiver);
                emailModel.Body = emailModel.Body.Replace(EmailTemplateConst.RecoverPwdLink, recoverLink);
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

        #endregion
    }
}
