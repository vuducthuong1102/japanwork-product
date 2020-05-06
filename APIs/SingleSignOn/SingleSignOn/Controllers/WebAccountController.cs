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

namespace SingleSignOn.Controllers
{    
    public class WebAccountController : BaseAuthenticatedController
    {
        private readonly ILog logger = LogProvider.For<WebAccountController>();
        private IWebAuthRedisService _redisService;
        private readonly IStoreUser _userStore;

        public WebAccountController(IWebAuthRedisService redisService, IStoreUser userStore)
        {
            _redisService = redisService;
            _userStore = userStore;
        }

        public ActionResult UpdateProfile()
        {
            var returnModel = new WebAccountUpdateProfileModel();
            if (UserCookieManager.IsAuthenticated())
            {
                var userLogin = UserCookieManager.GetCookie<UserCookieModel>(SingleSignOnSettings.SSOCommonUserKey);
                if (userLogin != null)
                {
                   var userId = userLogin.UserId;
                   var userInfo = _userStore.GetUserById(userId);
                   if(userInfo != null)
                    {
                        returnModel.UserId = userInfo.Id;
                        returnModel.TokenKey = userInfo.TokenKey;
                        returnModel.Address = userInfo.Address;
                        returnModel.Full_Name = userInfo.FullName;
                        returnModel.Display_Name = userInfo.DisplayName;
                        returnModel.Birthday = userInfo.Birthday?.ToString(Constant.DATE_FORMAT_ddMMyyyy);
                        returnModel.Sex = userInfo.Sex;
                        returnModel.Note = userInfo.Note;
                        returnModel.Avatar = userInfo.Avatar;
                    }
                }
            }
            return View(returnModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("UpdateProfile")]
        public ActionResult UpdateProfile_Post(WebAccountUpdateProfileModel model)
        {
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

            var validateMsg = string.Empty;
            var currentDomain = Request.Url.Host;
            var errorCode = ValidateUpdateProfileData(model, ref validateMsg);
            var isValid = (errorCode == EnumCommonCode.Success || errorCode == 0);

            logger.Debug("Begin UPDATEPROFILE-WEB request");
            logger.DebugFormat("UPDATEPROFILE-WEB Model encrypted = {0}", JsonConvert.SerializeObject(model));
            var birthday = (model.Birthday == null) ? DateTime.Now : DateTime.ParseExact(model.Birthday, Constant.DATE_FORMAT_ddMMyyyy, null);

            var sex = UserSex.Male;
            try
            {
                sex = Convert.ToInt32(model.Sex);
            }
            catch
            {
            }
            try
            {
                var message = string.Empty;
                if (isValid)
                {                    
                    var result = _userStore.UpdateProfile(
                        new IdentityUser
                        {
                            Id = model.UserId,
                            TokenKey = model.TokenKey,
                            FullName = model.Full_Name,
                            DisplayName = model.Display_Name,
                            Birthday = birthday,
                            Sex = sex,
                            Address = model.Address,
                            Note = model.Note,
                            Avatar = model.Avatar
                        }
                    );
                    
                    if (result == EnumCommonCode.Success)
                    {
                        //Write log successfully
                        var logInfo = GetDataForLogging(model);
                        logInfo.Domain = currentDomain;
                        logInfo.ActionType = ActionType.UpdateProfile;
                        logInfo.ActionDesc = string.Format("User [{0}] updated profile successfully", model.UserId);

                        //Write log 
                        _userStore.WriteUserLog(logInfo, SingleSignOnSettings.AllowToTraceLoginRequest);
                    }

                    message = GetUpdateProfleResponseMessage(result);

                    if (result == EnumCommonCode.Success)
                    {
                        this.AddNotification(message, NotificationType.SUCCESS);
                    }
                    else
                    {
                        this.AddNotification(message, NotificationType.ERROR);
                    }
                }
               
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to UPDATEPROFILE due to {0}", ex.Message);
                logger.Error(strError);
                this.AddNotification(strError, NotificationType.ERROR);
            }
            finally
            {
                logger.Debug("Ended UPDATEPROFILE-WEB request");
            }

            return View(model);
        }

        public ActionResult ViewProfile()
        {
            return View();
        }

        public ActionResult ChangePassword1(WebAccountChangePasswordModel model)
        {
            ModelState.Clear();
            if (UserCookieManager.IsAuthenticated())
            {
                try
                {
                    var userLogin = UserCookieManager.GetCookie<UserCookieModel>(SingleSignOnSettings.SSOCommonUserKey);
                    if (userLogin != null)
                    {
                        model.UserId = userLogin.UserId;
                        model.TokenKey = userLogin.TokenKey;
                    }
                }
                catch (Exception ex)
                {
                    var strError = string.Format("Failed to GetUserInformation due to {0}", ex.Message);
                    this.AddNotification(strError, NotificationType.ERROR);
                }   
            }

            return View(model);
        }

        [HttpPost]
        [ActionName("ChangePassword1")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmChangePassword1(WebAccountChangePasswordModel model)
        {
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

            logger.Debug("Begin CHANGEPWD1-WEB request");
            logger.DebugFormat("CHANGEPWD1-WEB Model encrypted = {0}", JsonConvert.SerializeObject(model));

            var newPwd = Utility.Md5HashingData(model.NewPassword);
            var oldPwd = Utility.Md5HashingData(model.OldPassword);
            try
            {
                var result = _userStore.ChangePassword(new IdentityUser { Id = model.UserId, NewPassword = newPwd, PasswordHash = oldPwd, TokenKey = model.TokenKey }, UserPasswordLevel.Level1);
                var msg = GetChangePwdResponseMessage(result);
                if (result == EnumCommonCode.Success)
                {
                    var currentDomain = Request.Url.Host;
                    ClearLoginDataCached(currentDomain);

                    this.AddNotification(msg, NotificationType.SUCCESS);
                }
                else
                {
                    this.AddNotification(msg, NotificationType.ERROR);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to ChangePassword1 due to {0}", ex.Message);
                logger.Error(strError);
                this.AddNotification(strError, NotificationType.ERROR);
            }
            finally
            {
                logger.Debug("Ended CHANGEPWD1-WEB request");
            }

            return View(model);
        }

        public ActionResult ChangePassword2(WebAccountChangePasswordModel model)
        {
            ModelState.Clear();
            if (UserCookieManager.IsAuthenticated())
            {
                try
                {
                    var userLogin = UserCookieManager.GetCookie<UserCookieModel>(SingleSignOnSettings.SSOCommonUserKey);
                    if (userLogin != null)
                    {
                        model.UserId = userLogin.UserId;
                        model.TokenKey = userLogin.TokenKey;
                    }
                }
                catch (Exception ex)
                {
                    var strError = string.Format("Failed to GetUserInformation due to {0}", ex.Message);
                    this.AddNotification(strError, NotificationType.ERROR);
                }
            }

            return View(model);
        }

        [HttpPost]
        [ActionName("ChangePassword2")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmChangePassword2(WebAccountChangePasswordModel model)
        {
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

            logger.Debug("Begin CHANGEPWD2-WEB request");
            logger.DebugFormat("CHANGEPWD2-WEB Model encrypted = {0}", JsonConvert.SerializeObject(model));

            var newPwd = Utility.Md5HashingData(model.NewPassword);
            var oldPwd = Utility.Md5HashingData(model.OldPassword);           
            try
            {
                var userIdentity = _userStore.GetUserById(model.UserId);
                if (userIdentity != null)
                {
                    var result = _userStore.ChangePassword(new IdentityUser { Id = model.UserId, NewPassword = newPwd, PasswordHash2 = oldPwd, TokenKey = model.TokenKey }, UserPasswordLevel.Level2);
                    var msg = GetChangePwdResponseMessage(result, UserPasswordLevel.Level2);
                    if (result == EnumCommonCode.Success)
                    {
                        this.AddNotification(msg, NotificationType.SUCCESS);

                        var otpCode = Utility.GenerateOTPCode();
                        var createOTPStatus = EnumCommonCode.Success;
                        var tranId = CreateNewOTPForAction(model.UserId, model.TokenKey, otpCode, newPwd, ActionType.ChangePassword2, ref createOTPStatus);
                        if (createOTPStatus == EnumCommonCode.Success)
                        {
                            if (!string.IsNullOrEmpty(tranId))
                            {
                                SendOTPCodeToUser(userIdentity.PhoneNumber, otpCode, tranId, userIdentity.OTPType);
                            }

                            this.AddNotification(UserWebResource.COMMON_SUCCESS_OTPCODE_SENT, NotificationType.SUCCESS);
                            return RedirectToAction("VerifyOTP", new WebVerifyOTPModel { UserId = model.UserId, TokenKey = model.TokenKey, OTPType = userIdentity.OTPType, ActionType = ActionType.ChangePassword2 });
                        }
                        else
                        {
                            this.AddNotification(tranId, NotificationType.ERROR);
                            return View(model);
                        }
                        
                    }
                    else
                    {
                        this.AddNotification(msg, NotificationType.ERROR);
                        return View(model);
                    }                            
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to ChangePassword2 due to {0}", ex.Message);
                logger.Error(strError);
                this.AddNotification(strError, NotificationType.ERROR);
            }
            finally
            {
                logger.Debug("Ended CHANGEPWD2-WEB request");
            }

            return View(model);
        }

        public ActionResult VerifyOTP(WebVerifyOTPModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("VerifyOTP")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmVerifyOTP(WebVerifyOTPModel model)
        {
            var currentDomain = Request.Url.Host;
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

            logger.Debug("Begin VERIFYOTP-WEB request for " + model.ActionType);
            logger.DebugFormat("VERIFYOTP-WEB Model encrypted = {0}", JsonConvert.SerializeObject(model));
            var message = string.Empty;
            var result = _userStore.VerifyOTPCode(
                new UserCodeIdentity
                {
                    UserId = model.UserId,
                    Code = model.OTPCode,
                    CodeType = model.OTPType,
                    CreatedDate = DateTime.Now,
                    TokenKey = model.TokenKey,
                    Action = model.ActionType
                });

            if (result == EnumCommonCode.Success)
            {
                //Write log
                var logInfo = GetDataForLogging(model);
                logInfo.ActionDesc = string.Format("The OTPCode [{0}] of User [{1}] was verified successfully for {2}", model.OTPCode, model.UserId, model.ActionType);
                logInfo.Domain = currentDomain;
                logInfo.ActionType = model.ActionType;

                _userStore.WriteUserLog(logInfo, SingleSignOnSettings.AllowToTraceLoginRequest);

                message = GetOTPResponseMessage(result, "verify", model.ActionType);
                this.AddNotification(message, NotificationType.SUCCESS);
            }
            else
            {
                message = GetOTPResponseMessage(result, "verify", model.ActionType);
                this.AddNotification(message, NotificationType.ERROR);
            }

            logger.Debug("Ended VERIFYOTP-WEB request for " + model.ActionType);

            return View(model);
        }

        public ActionResult ChangeAuthMethod(WebChangeAuthMethodModel model)
        {
            var userId = string.Empty;
            model.AuthMethod = OTPType.OTPSMS;
            if (UserCookieManager.IsAuthenticated())
            {
                var userLogin = UserCookieManager.GetCookie<UserCookieModel>(SingleSignOnSettings.SSOCommonUserKey);
                if (userLogin != null)
                {
                    model.UserId = userLogin.UserId;
                    model.TokenKey = userLogin.TokenKey;
                    model.AuthMethod = userLogin.OTPType;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ActionName("ChangeAuthMethod")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeAuthMethod_Post(WebChangeAuthMethodModel model)
        {
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

            logger.Debug("Begin CHANGE AUTH METHOD-WEB request");
            logger.DebugFormat("CHANGE AUTH METHOD-WEB Model encrypted = {0}", JsonConvert.SerializeObject(model));

            try
            {
                var userIdentity = _userStore.GetUserById(model.UserId);
                if (userIdentity != null)
                {
                    if(userIdentity.OTPType == model.AuthMethod)
                    {
                        this.AddNotification(UserWebResource.COMMON_ERROR_AUTH_METHOD_SELECTED, NotificationType.ERROR);
                        return View(model);
                    }
                    else
                    {                      
                        var otpCode = Utility.GenerateOTPCode();
                        var createOTPStatus = EnumCommonCode.Success;
                        var tranId = CreateNewOTPForAction(model.UserId, model.TokenKey, otpCode, model.AuthMethod, ActionType.ChangeAuthMethod, ref createOTPStatus, userIdentity.OTPType);
                        if (createOTPStatus == EnumCommonCode.Success)
                        {
                            if (!string.IsNullOrEmpty(tranId))
                            {
                                //Sent otp code to old method
                                SendOTPCodeToUser(userIdentity.PhoneNumber, otpCode, tranId, userIdentity.OTPType);
                            }

                            this.AddNotification(UserWebResource.COMMON_SUCCESS_OTPCODE_SENT, NotificationType.SUCCESS);
                            return RedirectToAction("VerifyOTP", new WebVerifyOTPModel { UserId = model.UserId, TokenKey = model.TokenKey, OTPType = userIdentity.OTPType, ActionType = ActionType.ChangeAuthMethod });
                        }
                        else
                        {
                            this.AddNotification(tranId, NotificationType.ERROR);
                            return View(model);
                        }                        
                    }                   
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to ChangeAuthMethod due to {0}", ex.Message);
                logger.Error(strError);
                this.AddNotification(strError, NotificationType.ERROR);
            }
            finally
            {
                logger.Debug("Ended CHANGE AUTH METHOD-WEB request");
            }

            return View(model);
        }

        #region Helpers
        private string GetChangePwdResponseMessage(int statusCode, int level = 1)
        {
            var message = string.Empty;
            if (statusCode == EnumCommonCode.Success)
            {
                if (level == UserPasswordLevel.Level2)
                {
                    message = UserWebResource.COMMON_SUCCESS_OTPCODE_SENT;
                }
                else
                {
                    message = UserWebResource.COMMON_CHANGEPWD_SUCCESS;
                }
            }
            else if (statusCode == EnumCommonCode.Error_OldPwd1NotCorrect)
            {
                message = UserWebResource.COMMON_ERROR_OLDPWD1_NOTCORRECT;
            }
            else if (statusCode == EnumCommonCode.Error_OldPwd2NotCorrect)
            {
                message = UserWebResource.COMMON_ERROR_OLDPWD2_NOTCORRECT;
            }
            else if (statusCode == EnumCommonCode.Error_NewPwdEqualOldPwd)
            {
                message = UserWebResource.COMMON_ERROR_OLDPWD_EQUAL_NEWPWD;
            }
            else if (statusCode == EnumCommonCode.Error_UserOrTokenNotFound)
            {
                message = UserWebResource.COMMON_ERROR_USERTOKEN_NOTFOUND;
            }

            return message;
        }

        private void ClearLoginDataCached(string domain)
        {
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

        private string CreateNewOTPForAction(int userId, string userToken, string otpCode, string targetData, string actionType, ref int status, string codeType = "")
        {
            var tranId = Utility.GenTranIdWithPrefix("ID");            
            try
            {
                var result = _userStore.CreateOTPCode(
                    new UserCodeIdentity
                    {
                        Id = tranId,
                        UserId = userId,
                        Code = otpCode,
                        CreatedDate = DateTime.Now,
                        TokenKey = userToken,
                        TargetData = targetData,
                        Action = actionType,
                        CodeType = codeType
                    });
                status = result;
                if (result == EnumCommonCode.Success)
                {
                    return tranId;
                }
                else
                {
                    return GetOTPResponseMessage(result);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to CreateOTPCode and send SMS to user due to {0}", ex.Message);
                logger.Error(strError);
            }

            return string.Empty;
        }

        private string SendOTPCodeToUser(string mobile, string otpCode, string tranId, string otpType = "")
        {
            string msg = string.Format(UserApiResource.COMMON_OTPCODE_FORMAT, otpCode);

            try
            {
                msg += " .TranId: " + tranId;
                if (otpType == OTPType.OTPSMS)
                {
                    var hasTimeout = false;
                    var smsModel = new SMSInputModel();
                    smsModel.message = msg;
                    smsModel.phoneNumber = SMSService.ReformatPhoneNumber(mobile);

                    SMSService.Send(smsModel, ref hasTimeout);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to Send SMS to user due to {0}", ex.Message);
                logger.Error(strError);
            }

            return msg;
        }

        private string GetOTPResponseMessage(int statusCode, string type = "create", string action = "")
        {
            var message = string.Empty;
            if (statusCode == EnumCommonCode.Success)
            {
                if (type == "create")
                {
                    message = UserWebResource.OTP_SUCCESS_CREATE;
                }
                else
                {
                    if (action == ActionType.ChangePassword2)
                    {
                        message = UserWebResource.COMMON_SUCCESS_CHANGEPWD2;
                    }
                    else
                    {
                        message = UserWebResource.OTP_SUCCESS_VERIFY;
                    }
                }
            }
            else if (statusCode == EnumCreateAndVerifyOTPCodeStatus.Error_OTPCodeNotExistsOrUsed)
            {
                message = UserWebResource.OTP_ERROR_OTPCODE_NOTEXISTS_ORUSED;
            }
            else if (statusCode == EnumCommonCode.Error_UserOrTokenNotFound)
            {
                message = UserWebResource.COMMON_ERROR_USERTOKEN_NOTFOUND;
            }
            else if (statusCode == EnumCreateAndVerifyOTPCodeStatus.Error_OTPCodeExpired)
            {
                message = UserWebResource.OTP_ERROR_OTPCODE_EXPIRED;
            }

            return message;
        }

        private string GetUpdateProfleResponseMessage(int statusCode)
        {
            var message = string.Empty;
            if (statusCode == EnumCommonCode.Success)
            {
                message = UserWebResource.UPDATE_PROFILE_SUCCESS;
            }
            else if (statusCode == EnumCommonCode.Error_UserOrTokenNotFound)
            {
                message = UserWebResource.COMMON_ERROR_USERTOKEN_NOTFOUND;
            }

            return message;
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

        private int ValidateUpdateProfileData(WebAccountUpdateProfileModel model, ref string message)
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

        #endregion
    }
}
