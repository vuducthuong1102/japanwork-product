using System;
using System.Net;
using System.Web.Http;
using System.Threading.Tasks;
using SingleSignOn.Logging;
using SingleSignOn.Services;
using SingleSignOn.ActionResults;
using SingleSignOn.Extensions;
using SingleSignOn.Helpers;
using SingleSignOn.Models;
using SingleSignOn.DB.Sql.Stores;
using SingleSignOn.DB.Sql.Entities;
using System.Web;
using SingleSignOn.Settings;
using SingleSignOn.Resources;
using SingleSignOn.Helpers.Validation;
using Newtonsoft.Json;
using System.Collections.Generic;
using Autofac;

namespace SingleSignOn.Controllers
{
    [Authorize]
    [RoutePrefix("api/user")]
    public class ApiAuthController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiAuthController>();
        private IApiAuthRedisService _redisService;
        private IStoreUser _userStore;


        public ApiAuthController(IApiAuthRedisService redisService, IStoreUser userStore)
        {
            _redisService = redisService;
            _userStore = userStore;
        }

        [Route("login")]
        [HttpPost]
        public async Task<IHttpActionResult> Login(ApiAuthLoginModel model)
        {
            CreateDocumentApi(model);
            var returnModel = new ResponseApiAuthLoginModel();
            var jsonString = string.Empty;
            var currentDomain = Request.RequestUri.Host;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            model = ReFormatLoginModel(model);
            var validateMsg = string.Empty;
            var errorCode = ValidateLoginData(model, ref validateMsg);

            var isValid = (errorCode == EnumCommonCode.Success || errorCode == 0);
            try
            {
                logger.Debug("Begin LOGIN-API request");

                if (!isValid)
                {
                    returnModel.Code = errorCode;
                    returnModel.Msg = validateMsg;
                }

                if (isValid)
                {
                    //Extract data from cache first    
                    var redisKey = model.UserName + model.Password;
                    var cachedData = await _redisService.GetApiAuthLoginModelAsync(redisKey, currentDomain);
                    if (cachedData == null)
                    {
                        var userIdentity = new IdentityUser { UserName = model.UserName, PasswordHash = model.Password, Domain = currentDomain };

                        //Get from database
                        var apiLoginIdentity = _userStore.ApiLogin(userIdentity);
                        if (apiLoginIdentity != null)
                        {                           
                            apiLoginIdentity.LoginDate = DateTime.Now;
                            if (apiLoginIdentity.EmailConfirmed == 0 && apiLoginIdentity.PhoneNumberConfirmed == 0)
                            {
                                //Account hasn't been activated
                                returnModel.Code = EnumCommonCode.Error_InactiveAccount;
                                returnModel.Msg = UserApiResource.COMMON_ERROR_INACTIVE_ACCOUNT;
                                returnModel.Data = new ApiAuthUserLoginIdentity { Id = apiLoginIdentity.Id };
                            }
                            else
                            {
                                var needCreateToken = ((string.IsNullOrEmpty(apiLoginIdentity.TokenKey) || (!string.IsNullOrEmpty(apiLoginIdentity.TokenKey) && apiLoginIdentity.TokenExpiredDate != null && apiLoginIdentity.TokenExpiredDate < DateTime.Now)));
                                //If token doesn't exists or was expired
                                if (needCreateToken)
                                {
                                    //Generate tokenkey when the input data is valid
                                    returnModel.TokenKey = GenerateTokenKey(model.UserName);

                                    var gentokenResult = _userStore.ProvideTokenKeyForUser(new UserTokenIdentity { UserId = apiLoginIdentity.Id, TokenKey = returnModel.TokenKey, Domain = currentDomain, Method = ActionMethod.Api });
                                    if (gentokenResult)
                                    {
                                        //Login successfully
                                        var logInfo = GetDataForLogging(model);
                                        logInfo.ActionDesc = string.Format("User [{0}] logged in successfully with token [{1}]", apiLoginIdentity.Id, returnModel.TokenKey);
                                        logInfo.Domain = currentDomain;
                                        logInfo.ActionType = ActionType.Login;

                                        //Write log after gentoken successfully
                                        _userStore.WriteUserLog(logInfo, SingleSignOnSettings.AllowToTraceLoginRequest);
                                        var dtNow = DateTime.Now;
                                        apiLoginIdentity.TokenCreatedDate = dtNow;
                                        if (apiLoginIdentity.LoginDurations > 0)
                                        {
                                            apiLoginIdentity.TokenExpiredDate = dtNow.AddMinutes(apiLoginIdentity.LoginDurations);
                                        }
                                        else
                                        {
                                            apiLoginIdentity.TokenExpiredDate = dtNow.AddMinutes(SingleSignOnSettings.DefaultCachedTimeout);
                                        }
                                    }

                                    apiLoginIdentity.TokenKey = returnModel.TokenKey;
                                }
                                else
                                {
                                    returnModel.TokenKey = apiLoginIdentity.TokenKey;
                                }

                                if (!string.IsNullOrEmpty(apiLoginIdentity.TokenKey))
                                    returnModel.TokenKey = apiLoginIdentity.TokenKey;

                                if (!string.IsNullOrEmpty(apiLoginIdentity.Avatar))
                                {
                                    apiLoginIdentity.Avatar = CdnHelper.GetFullImgPath(apiLoginIdentity.Avatar);
                                }
                                returnModel.Data = apiLoginIdentity;
                                returnModel.Code = EnumCommonCode.Success;
                                returnModel.Msg = UserApiResource.LOGIN_SUCCESS;

                                //Save to cache after login successfully
                                await _redisService.SetResponseApiAuthLoginModelAsync(returnModel, redisKey, currentDomain);
                            }
                        }
                        else
                        {
                            returnModel.Code = EnumLoginStatus.LoginErrorUserInfoIncorrect;
                            returnModel.Msg = UserApiResource.LOGIN_ERROR_INFO_INVALID;
                        }

                        //Save to cache
                        //await _redisService.SetResponseApiAuthLoginModelAsync(returnModel, redisKey, currentDomain);
                    }
                    else
                    {
                        returnModel = cachedData;

                        if (returnModel.Code == EnumCommonCode.Success)
                            returnModel.Msg = UserApiResource.LOGIN_SUCCESS;
                    }
                }

                jsonString = JsonConvert.SerializeObject(returnModel);

                logger.DebugFormat("LOGIN-API Returned Model encrypted = {0}", jsonString);
            }
            catch (Exception ex)
            {
                string strError = "Failed for LOGIN-API request: " + ex.Message;
                logger.ErrorException(strError, ex);
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = UserApiResource.COMMON_ERROR_SYSTEM;

                jsonString = JsonConvert.SerializeObject(returnModel);
                return new JsonActionResult(HttpStatusCode.BadRequest, jsonString);
            }
            finally
            {
                logger.Debug("Ended LOGIN-API request");
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [Route("loginwith")]
        [HttpPost]
        public async Task<IHttpActionResult> LoginWith(ApiAuthLoginWithModel model)
        {
            CreateDocumentApi(model);
            var returnModel = new ResponseApiAuthLoginModel();
            var jsonString = string.Empty;
            var currentDomain = Request.RequestUri.Host;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            model = ReFormatLoginWithModel(model);
            var validateMsg = string.Empty;
            var errorCode = 0;
            var isValid = (errorCode == EnumCommonCode.Success || errorCode == 0);
            try
            {
                logger.Debug("Begin LOGIN-WITH-API request");

                if (!isValid)
                {
                    returnModel.Code = errorCode;
                    returnModel.Msg = validateMsg;
                }

                if (isValid)
                {
                    //Extract data from cache first    
                    var redisKey = model.UserName + model.SocialProvider;
                    var cachedData = await _redisService.GetApiAuthLoginModelAsync(redisKey, currentDomain);
                    if (cachedData == null)
                    {
                        var userIdentity = new IdentityUser { UserName = model.UserName, DisplayName = model.DisplayName, Email = model.Email, SocialProvider = model.SocialProvider, Domain = currentDomain };

                        //Get from database
                        var apiLoginIdentity = _userStore.ApiLoginWith(userIdentity);
                        if (apiLoginIdentity != null)
                        {
                            if (apiLoginIdentity.IsNew == 1)
                            {
                                //Get avatar here
                                var avatar = LoginSocialServices.GetAvatarAsync(model.SocialProvider, model.UserName).Result;

                                if (!string.IsNullOrEmpty(avatar))
                                {
                                    apiLoginIdentity.Avatar = avatar;

                                    userIdentity.Id = apiLoginIdentity.Id;
                                    userIdentity.Avatar = avatar;

                                    //Update avatar here
                                    _userStore.UpdateAvatar(userIdentity);
                                }
                            }

                            apiLoginIdentity.LoginDate = DateTime.Now;
                            var needCreateToken = ((string.IsNullOrEmpty(apiLoginIdentity.TokenKey) || (!string.IsNullOrEmpty(apiLoginIdentity.TokenKey) && apiLoginIdentity.TokenExpiredDate != null && apiLoginIdentity.TokenExpiredDate < DateTime.Now)));
                            //If token doesn't exists or was expired
                            if (needCreateToken)
                            {
                                //Generate tokenkey when the input data is valid
                                returnModel.TokenKey = GenerateTokenKey(model.UserName);

                                var gentokenResult = _userStore.ProvideTokenKeyForUser(new UserTokenIdentity { UserId = apiLoginIdentity.Id, TokenKey = returnModel.TokenKey, Domain = currentDomain, Method = ActionMethod.Api });
                                if (gentokenResult)
                                {
                                    //Login successfully
                                    var logInfo = GetDataForLogging(model);
                                    logInfo.ActionDesc = string.Format("User [{0}] logged in successfully with token [{1}]", apiLoginIdentity.Id, returnModel.TokenKey);
                                    logInfo.Domain = currentDomain;
                                    logInfo.ActionType = ActionType.Login;

                                    //Write log after gentoken successfully
                                    _userStore.WriteUserLog(logInfo, SingleSignOnSettings.AllowToTraceLoginRequest);
                                    var dtNow = DateTime.Now;
                                    apiLoginIdentity.TokenCreatedDate = dtNow;
                                    if (apiLoginIdentity.LoginDurations > 0)
                                    {
                                        apiLoginIdentity.TokenExpiredDate = dtNow.AddMinutes(apiLoginIdentity.LoginDurations);
                                    }
                                    else
                                    {
                                        apiLoginIdentity.TokenExpiredDate = dtNow.AddMinutes(SingleSignOnSettings.DefaultCachedTimeout);
                                    }
                                }

                                apiLoginIdentity.TokenKey = returnModel.TokenKey;
                            }
                            else
                            {
                                returnModel.TokenKey = apiLoginIdentity.TokenKey;
                            }

                            if (!string.IsNullOrEmpty(apiLoginIdentity.TokenKey))
                                returnModel.TokenKey = apiLoginIdentity.TokenKey;

                            if (!string.IsNullOrEmpty(apiLoginIdentity.Avatar))
                            {
                                apiLoginIdentity.Avatar = CdnHelper.GetFullImgPath(apiLoginIdentity.Avatar);
                            }
                            returnModel.Data = apiLoginIdentity;
                            returnModel.Code = EnumCommonCode.Success;
                            returnModel.Msg = UserApiResource.LOGIN_SUCCESS;

                            //Save to cache after login successfully
                            await _redisService.SetResponseApiAuthLoginModelAsync(returnModel, redisKey, currentDomain);
                        }
                        else
                        {
                            returnModel.Code = EnumLoginStatus.LoginErrorUserInfoIncorrect;
                            returnModel.Msg = UserApiResource.LOGIN_ERROR_INFO_INVALID;
                        }

                        //Save to cache
                        //await _redisService.SetResponseApiAuthLoginModelAsync(returnModel, redisKey, currentDomain);
                    }
                    else
                    {
                        returnModel = cachedData;

                        if (returnModel.Code == EnumCommonCode.Success)
                            returnModel.Msg = UserApiResource.LOGIN_SUCCESS;
                    }
                }

                jsonString = JsonConvert.SerializeObject(returnModel);

                logger.DebugFormat("LOGIN-WITH-API Returned Model encrypted = {0}", jsonString);
            }
            catch (Exception ex)
            {
                string strError = "Failed for LOGIN-WITH-API request: " + ex.Message;
                logger.ErrorException(strError, ex);
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = UserApiResource.COMMON_ERROR_SYSTEM;

                jsonString = JsonConvert.SerializeObject(returnModel);
                return new JsonActionResult(HttpStatusCode.OK, jsonString);
            }
            finally
            {
                logger.Debug("Ended LOGIN-WITH-API request");
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }


        //[Route("refreshtoken")]
        //[HttpPost]
        //private async Task<IHttpActionResult> RefreshToken(RefreshTokenModel model)
        //{
        //    CreateDocumentApi(model);
        //    var returnModel = new ResponseApiModel();
        //    var jsonString = string.Empty;
        //    var currentDomain = Request.RequestUri.Host;

        //    if (!ModelState.IsValid)
        //    {
        //        return ModelValidation.ApiValidate(ModelState, returnModel);
        //    }

        //    var validateMsg = string.Empty;
        //    var errorCode = ValidateApiCallingTime(model.Time, ref validateMsg);

        //    var isValid = (errorCode == EnumCommonCode.Success || errorCode == 0);
        //    try
        //    {
        //        logger.Debug("Begin REFRESH TOKEN-API request");

        //        if (!isValid)
        //        {
        //            returnModel.Code = errorCode;
        //            returnModel.Msg = validateMsg;
        //        }

        //        if (isValid)
        //        {
        //            var dtRefresh = DateTime.Now;
        //            var newTokenKey = GenerateTokenKey(model.UserId.ToString());

        //            //Update token
        //            var affected_rows = _userStore.RefreshTokenKey(new UserTokenIdentity { UserId = model.UserId, TokenKey = model.Token, Domain = currentDomain, CreatedDate = dtRefresh });
        //            await Task.FromResult(affected_rows);

        //            if (affected_rows > 0)
        //            {
        //                //Write log
        //                var logInfo = GetDataForLogging(model);
        //                logInfo.ActionDesc = string.Format("User [{0}] token was renewed successfully. New token: [{1}]", model.UserId, newTokenKey);
        //                logInfo.Domain = currentDomain;
        //                logInfo.ActionType = ActionType.RefreshToken;

        //                _userStore.WriteUserLog(logInfo, SingleSignOnSettings.AllowToTraceLoginRequest);

        //                returnModel.Code = EnumCommonCode.Success;
        //                returnModel.Msg = UserApiResource.REFRESHTOKEN_SUCCESS;
        //            }
        //            else
        //            {
        //                returnModel.Code = EnumCommonCode.Error_UserOrTokenNotFound;
        //                returnModel.Msg = UserApiResource.COMMON_ERROR_USERTOKEN_NOTFOUND;
        //            }
        //        }

        //        returnModel.TokenKey = model.Token;
        //        jsonString = JsonConvert.SerializeObject(returnModel);

        //        logger.DebugFormat("REFRESH TOKEN-API Returned Model encrypted = {0}", jsonString);
        //    }
        //    catch (Exception ex)
        //    {
        //        string strError = "Failed for REFRESH TOKEN-API request: " + ex.Message;
        //        logger.ErrorException(strError, ex);
        //        returnModel.Code = EnumCommonCode.Error;
        //        returnModel.Msg = strError;

        //        jsonString = JsonConvert.SerializeObject(returnModel);
        //        return new JsonActionResult(HttpStatusCode.BadRequest, jsonString);
        //    }
        //    finally
        //    {
        //        logger.Debug("Ended REFRESH TOKEN-API request");
        //    }

        //    return new JsonActionResult(HttpStatusCode.OK, jsonString);
        //}

        [Route("refreshtoken")]
        [HttpPost]
        public async Task<IHttpActionResult> RefreshToken(RefreshTokenModel model)
        {
            CreateDocumentApi(model);
            var returnModel = new ResponseApiModel();
            var jsonString = string.Empty;
            var currentDomain = Request.RequestUri.Host;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            var validateMsg = string.Empty;
            var errorCode = ValidateApiCallingTime(model.Time, ref validateMsg);

            var isValid = (errorCode == EnumCommonCode.Success || errorCode == 0);
            try
            {
                logger.Debug("Begin REFRESH TOKEN-API request");

                if (!isValid)
                {
                    returnModel.Code = errorCode;
                    returnModel.Msg = validateMsg;
                }

                //Generate tokenkey when the input data is valid
                var newTokenKey = GenerateTokenKey(model.UserId.ToString());
                returnModel.TokenKey = newTokenKey;

                var gentokenResult = _userStore.ProvideTokenKeyForUser(new UserTokenIdentity { UserId = model.UserId, TokenKey = returnModel.TokenKey, Domain = currentDomain, Method = ActionMethod.Api });

                await Task.FromResult(gentokenResult);
                if (gentokenResult)
                {
                    returnModel.Code = EnumCommonCode.Success;
                    returnModel.Msg = UserApiResource.COMMON_SUCCESS;
                }

                jsonString = JsonConvert.SerializeObject(returnModel);

                logger.DebugFormat("REFRESH TOKEN-API Returned Model encrypted = {0}", jsonString);
            }
            catch (Exception ex)
            {
                string strError = "Failed for REFRESH TOKEN-API request: " + ex.Message;
                logger.ErrorException(strError, ex);
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = strError;

                jsonString = JsonConvert.SerializeObject(returnModel);
                return new JsonActionResult(HttpStatusCode.BadRequest, jsonString);
            }
            finally
            {
                logger.Debug("Ended REFRESH TOKEN-API request");
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [Route("resendotp")]
        [HttpPost]
        public async Task<IHttpActionResult> ResendOTP(CreateOTPModel model)
        {

            CreateDocumentApi(model);
            var returnModel = new ResponseApiModel();
            var jsonString = string.Empty;
            var currentDomain = Request.RequestUri.Host;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            var validateMsg = string.Empty;
            var errorCode = ValidateApiCallingTime(model.Time, ref validateMsg);

            var isValid = (errorCode == EnumCommonCode.Success || errorCode == 0);
            try
            {
                logger.Debug("Begin RESEND OTP-API request");

                if (!isValid)
                {
                    returnModel.Code = errorCode;
                    returnModel.Msg = validateMsg;
                }

                if (isValid)
                {
                    var dtCreate = DateTime.Now;
                    try
                    {
                        dtCreate = DateTime.ParseExact(model.Time, Constant.DATEFORMAT_yyyyMMddHHmmss, null);
                    }
                    catch
                    {
                    }

                    var currentOTP = _userStore.GetCurrentOTP(new UserCodeIdentity { UserId = model.UserId, Action = model.Action });
                    if(currentOTP == null)
                    {
                        returnModel.Code = EnumCommonCode.Error;
                        returnModel.Msg = UserApiResource.COMMON_INFO_NOTFOUND;

                        jsonString = JsonConvert.SerializeObject(returnModel);
                        return new JsonActionResult(HttpStatusCode.OK, jsonString);
                    }

                    var userInfo = _userStore.GetUserById(model.UserId);
                    
                    if (userInfo != null)
                    {
                        var otpCode = Utility.GenerateOTPCode();
                        var createOTPStatus = EnumCommonCode.Success;

                        var tranId = CreateNewOTPForAction(model.UserId, string.Empty, otpCode, currentOTP.TargetData, model.Action, ref createOTPStatus);
                        await Task.FromResult(tranId);

                        if (createOTPStatus == EnumCommonCode.Success)
                        {
                            if (!string.IsNullOrEmpty(tranId))
                            {
                                //Begin send OTP to user
                                SendOTPCodeToUser(userInfo.PhoneNumber, otpCode, tranId, OTPType.OTPSMS);

                                //Write log
                                var logInfo = GetDataForLogging(model);
                                logInfo.ActionDesc = string.Format("RESEND new OTP Code [{0}] for User [{1}] successfully", otpCode, model.UserId);
                                logInfo.Domain = currentDomain;
                                logInfo.ActionType = ActionType.CreateOTP;

                                _userStore.WriteUserLog(logInfo, SingleSignOnSettings.AllowToTraceLoginRequest);

                                returnModel.Code = EnumCommonCode.Success;
                                returnModel.Msg = GetOTPResponseMessage(createOTPStatus, "create");
                            }

                            returnModel.Msg = string.Format(UserApiResource.COMMON_SUCCESS_OTPCODE_SENT, userInfo.PhoneNumber);
                            returnModel.Code = EnumCommonCode.Success;
                        }
                        else
                        {
                            returnModel.Code = EnumCommonCode.Error;
                            returnModel.Msg = tranId;
                        }
                    }
                    else
                    {
                        returnModel.Code = EnumCommonCode.Error_UserOrTokenNotFound;
                        returnModel.Msg = UserApiResource.COMMON_ERROR_USERTOKEN_NOTFOUND;
                    }                                       
                }

                returnModel.TokenKey = string.Empty;
                jsonString = JsonConvert.SerializeObject(returnModel);

                logger.DebugFormat("RESEND OTP-API Returned Model encrypted = {0}", jsonString);
            }
            catch (Exception ex)
            {
                string strError = "Failed for RESEND OTP-API request: " + ex.Message;
                logger.ErrorException(strError, ex);
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = strError;

                jsonString = JsonConvert.SerializeObject(returnModel);
                return new JsonActionResult(HttpStatusCode.OK, jsonString);
            }
            finally
            {
                logger.Debug("Ended RESEND OTP-API request");
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [Route("resendemail")]
        [HttpPost]
        public async Task<IHttpActionResult> ResendEmail(CreateEmailModel model)
        {

            CreateDocumentApi(model);
            var returnModel = new ResponseApiModel();
            var jsonString = string.Empty;
            var currentDomain = Request.RequestUri.Host;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            var validateMsg = string.Empty;
            var errorCode = ValidateApiCallingTime(model.Time, ref validateMsg);

            var isValid = (errorCode == EnumCommonCode.Success || errorCode == 0);
            try
            {
                logger.Debug("Begin RESEND EMAIL-API request");

                if (!isValid)
                {
                    returnModel.Code = errorCode;
                    returnModel.Msg = validateMsg;
                }

                if (isValid)
                {
                    var emailStore = GlobalContainer.IocContainer.Resolve<IStoreSystemEmail>();
                    var currentEmail = emailStore.GetEmailToResend(model.Email, model.Action);

                    if (currentEmail == null)
                    {
                        returnModel.Code = EnumCommonCode.Error;
                        returnModel.Msg = UserApiResource.COMMON_INFO_NOTFOUND;

                        jsonString = JsonConvert.SerializeObject(returnModel);
                        return new JsonActionResult(HttpStatusCode.OK, jsonString);
                    }

                    var result = await Task.FromResult(BeginResendEmail(currentEmail));

                    returnModel.Code = result;
                    if (result == EnumCommonCode.Success)
                    {
                        //Write log
                        var logInfo = GetDataForLogging(model);
                        logInfo.ActionDesc = string.Format("RESEND EMAIL to [{0}] successfully", currentEmail.Receiver);
                        logInfo.Domain = currentDomain;
                        logInfo.ActionType = ActionType.ResendEmail;

                        _userStore.WriteUserLog(logInfo, SingleSignOnSettings.AllowToTraceLoginRequest);

                        returnModel.Msg = string.Format(EmailResource.NOTIF_EMAIL_SENT_FORMAT, currentEmail.Receiver);
                    }
                    else
                    {
                        returnModel.Msg = UserApiResource.COMMON_ERROR_SYSTEM;
                    }
                }

                jsonString = JsonConvert.SerializeObject(returnModel);

                logger.DebugFormat("RESEND EMAIL-API Returned Model encrypted = {0}", jsonString);
            }
            catch (Exception ex)
            {
                string strError = "Failed for RESEND EMAIL-API request: " + ex.Message;
                logger.ErrorException(strError, ex);
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = strError;

                jsonString = JsonConvert.SerializeObject(returnModel);
                return new JsonActionResult(HttpStatusCode.OK, jsonString);
            }
            finally
            {
                logger.Debug("Ended RESEND EMAIL-API request");
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [Route("verifyotp")]
        [HttpPost]
        public async Task<IHttpActionResult> VerifyOTP(VerifyOTPModel model)
        {
            CreateDocumentApi(model);
            var returnModel = new ResponseApiModel();
            var jsonString = string.Empty;
            var currentDomain = Request.RequestUri.Host;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            var validateMsg = string.Empty;
            var errorCode = ValidateApiCallingTime(model.Time, ref validateMsg);

            var isValid = (errorCode == EnumCommonCode.Success || errorCode == 0);
            try
            {
                logger.Debug("Begin VERIFY OTP-API request for " + model.Action);

                if (!isValid)
                {
                    returnModel.Code = errorCode;
                    returnModel.Msg = validateMsg;
                }

                if (isValid)
                {
                    var dtCreate = DateTime.Now;
                    try
                    {
                        dtCreate = DateTime.ParseExact(model.Time, Constant.DATEFORMAT_yyyyMMddHHmmss, null);
                    }
                    catch
                    {
                    }

                    var result = _userStore.VerifyOTPCode(
                        new UserCodeIdentity
                        {
                            UserId = model.UserId,
                            Code = model.OTPCode,
                            CreatedDate = dtCreate,
                            TokenKey = model.Token,
                            Action = model.Action
                        });

                    await Task.FromResult(result);
                    if (result == EnumCommonCode.Success)
                    {
                        //Write log
                        var logInfo = GetDataForLogging(model);
                        logInfo.ActionDesc = string.Format("The OTPCode [{0}] of User [{1}] was verified successfully for {2}", model.OTPCode, model.UserId, model.Action);
                        logInfo.Domain = currentDomain;
                        logInfo.ActionType = model.Action;

                        _userStore.WriteUserLog(logInfo, SingleSignOnSettings.AllowToTraceLoginRequest);

                        returnModel.Code = EnumCommonCode.Success;
                        returnModel.Msg = GetOTPResponseMessage(result, "verify", model.Action);
                    }
                    else
                    {
                        returnModel.Code = result;
                        returnModel.Msg = GetOTPResponseMessage(result, "verify");
                    }
                }

                returnModel.TokenKey = model.Token;
                jsonString = JsonConvert.SerializeObject(returnModel);

                logger.DebugFormat("VERIFY OTP-API Returned Model encrypted = {0}", jsonString);
            }
            catch (Exception ex)
            {
                string strError = "Failed for VERIFY OTP-API request: " + ex.Message;
                logger.ErrorException(strError, ex);
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = strError;

                jsonString = JsonConvert.SerializeObject(returnModel);
                return new JsonActionResult(HttpStatusCode.BadRequest, jsonString);
            }
            finally
            {
                logger.Debug("Ended VERIFY OTP-API request");
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [Route("activeaccount")]
        [HttpPost]
        public async Task<IHttpActionResult> ActiveAccount(ApiActiveAccountModel model)
        {

            CreateDocumentApi(model);
            var returnModel = new ResponseApiModel();
            var jsonString = string.Empty;
            var currentDomain = Request.RequestUri.Host;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            var validateMsg = string.Empty;
            var errorCode = 0;
            errorCode = ValidateApiCallingTime(model.Time, ref validateMsg);

            var isValid = (errorCode == EnumCommonCode.Success || errorCode == 0);
            try
            {
                logger.Debug("Begin ACTIVE ACCOUNT-API request");

                if (!isValid)
                {
                    returnModel.Code = errorCode;
                    returnModel.Msg = validateMsg;
                }

                if (isValid)
                {
                    var code = EnumCommonCode.Success;
                    var info = new IdentityActiveAccount();
                    info.UserName = model.UserName;
                    info.HashingData = model.HashingData;

                    code = _userStore.ActiveAccountByEmail(info);

                    await Task.FromResult(code);

                    returnModel.Code = code;
                    returnModel.Msg = GetActiveAccountMessage(code);
                }

                jsonString = JsonConvert.SerializeObject(returnModel);

                logger.DebugFormat("ACTIVE ACCOUNT-API Returned Model encrypted = {0}", jsonString);
            }
            catch (Exception ex)
            {
                string strError = "Failed for ACTIVE ACCOUNT-API request: " + ex.Message;
                logger.ErrorException(strError, ex);
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = UserWebResource.COMMON_EXCEPTION_NOTIF;

                jsonString = JsonConvert.SerializeObject(returnModel);
                return new JsonActionResult(HttpStatusCode.BadRequest, jsonString);
            }
            finally
            {
                logger.Debug("Ended ACTIVE ACCOUNT-API request");
            }


            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [Route("logout")]
        [HttpPost]
        public async Task<IHttpActionResult> Logout(LogoutModel model)
        {

            CreateDocumentApi(model);
            var returnModel = new ResponseApiModel();
            var jsonString = string.Empty;
            var currentDomain = Request.RequestUri.Host;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            var validateMsg = string.Empty;
            var errorCode = ValidateApiCallingTime(model.Time, ref validateMsg);

            var isValid = (errorCode == EnumCommonCode.Success || errorCode == 0);
            try
            {
                logger.Debug("Begin LOG OUT-API request");

                if (!isValid)
                {
                    returnModel.Code = errorCode;
                    returnModel.Msg = validateMsg;
                }

                if (isValid)
                {
                    var userIdentity = _userStore.GetUserById(model.UserId);
                    await Task.FromResult(userIdentity);

                    if (userIdentity != null)
                    {
                        if (userIdentity.TokenKey != model.Token)
                        {
                            returnModel.Code = EnumCommonCode.Error_UserOrTokenNotFound;
                            returnModel.Msg = UserApiResource.COMMON_ERROR_USERTOKEN_NOTFOUND;
                        }
                        else
                        {
                            //Begin logout
                            // ClearLoginDataCachedById(model.UserId, currentDomain);
                            //logger.Debug("Clear login cache successfully.");

                            ////Write log
                            //var logInfo = GetDataForLogging(model);
                            //logInfo.ActionDesc = string.Format("The OTPCode [{0}] of User [{1}] was verified successfully", model.OTPCode, model.UserId);
                            //logInfo.Domain = currentDomain;
                            //logInfo.ActionType = ActionType.VerifyOTP;

                            //_userStore.WriteUserLog(logInfo, SingleSignOnSettings.AllowToTraceLoginRequest);

                            returnModel.Code = EnumCommonCode.Success;
                            returnModel.Msg = UserApiResource.COMMON_AUTH_SUCCESS;
                        }
                    }
                    else
                    {
                        returnModel.Code = EnumCommonCode.Error_UserOrTokenNotFound;
                        returnModel.Msg = UserApiResource.COMMON_ERROR_USERTOKEN_NOTFOUND;
                    }
                }

                returnModel.TokenKey = model.Token;
                jsonString = JsonConvert.SerializeObject(returnModel);

                logger.DebugFormat("LOG OUT-API Returned Model encrypted = {0}", jsonString);
            }
            catch (Exception ex)
            {
                string strError = "Failed for LOG OUT-API request: " + ex.Message;
                logger.ErrorException(strError, ex);
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = strError;

                jsonString = JsonConvert.SerializeObject(returnModel);
                return new JsonActionResult(HttpStatusCode.BadRequest, jsonString);
            }
            finally
            {
                logger.Debug("Ended LOG OUT-API request");
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [Route("checkuserexists")]
        [HttpPost]
        public async Task<IHttpActionResult> CheckUserExists(ApiCheckUserExistsModel model)
        {
            CreateDocumentApi(model);
            var returnModel = new ResponseApiModel();
            var jsonString = string.Empty;
            var currentDomain = Request.RequestUri.Host;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            var validateMsg = string.Empty;
            var errorCode = ValidateCheckUserExistsData(model, ref validateMsg);

            var isValid = (errorCode == EnumCommonCode.Success || errorCode == 0);
            try
            {
                logger.Debug("Begin CHECK USER EXISTS-API request");

                if (!isValid)
                {
                    returnModel.Code = errorCode;
                    returnModel.Msg = validateMsg;
                }

                if (isValid)
                {
                    var userIdentity = _userStore.GetUserByUserName(model.UserName);
                    await Task.FromResult(userIdentity);

                    if (userIdentity != null)
                    {
                        returnModel.Code = EnumCommonCode.Success;
                        returnModel.Msg = UserApiResource.COMMON_USERNAME_EXISTED;
                    }
                    else
                    {
                        returnModel.Code = EnumCommonCode.Success;
                        returnModel.Msg = UserApiResource.COMMON_USERNAME_NOT_EXISTED;
                    }
                }

                returnModel.TokenKey = model.Token;
                jsonString = JsonConvert.SerializeObject(returnModel);

                logger.DebugFormat("CHECK USER EXISTS-API Returned Model encrypted = {0}", jsonString);
            }
            catch (Exception ex)
            {
                string strError = "Failed for CHECK USER EXISTS-API request: " + ex.Message;
                logger.ErrorException(strError, ex);
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = strError;

                jsonString = JsonConvert.SerializeObject(returnModel);
                return new JsonActionResult(HttpStatusCode.BadRequest, jsonString);
            }
            finally
            {
                logger.Debug("Ended CHECK USER EXISTS-API request");
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [Route("checkusertoken")]
        [HttpPost]
        public async Task<IHttpActionResult> CheckUserToken(ApiCheckUserTokenModel model)
        {

            CreateDocumentApi(model);
            var returnModel = new ResponseApiModel();
            var jsonString = string.Empty;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            var validateMsg = string.Empty;
            var errorCode = ValidateApiCallingTime(model.Time, ref validateMsg);

            var isValid = (errorCode == EnumCommonCode.Success || errorCode == 0);
            try
            {
                if (!isValid)
                {
                    returnModel.Code = errorCode;
                    returnModel.Msg = validateMsg;
                }

                if (isValid)
                {
                    var tokenData = AccountHelper.GetUserToken(model.UserId);
                    await Task.FromResult(tokenData);

                    if (tokenData != null)
                    {
                        var tokenValid = false;
                        if (!string.IsNullOrEmpty(tokenData.TokenKey))
                        {
                            if (string.Equals(tokenData.TokenKey, model.Token, StringComparison.CurrentCultureIgnoreCase))
                            {
                                tokenValid = true;
                                //if (tokenData.ExpiredDate != null && tokenData.ExpiredDate >= DateTime.Now)
                                //{
                                //    tokenValid = true;
                                //}
                            }
                        }

                        if (!tokenValid)
                        {
                            returnModel.Code = EnumCommonCode.Error;
                            returnModel.Msg = UserApiResource.COMMON_ERROR_USERTOKEN_NOTFOUND;
                        }
                        else
                        {
                            returnModel.Code = EnumCommonCode.Success;
                            returnModel.Msg = UserApiResource.COMMON_SUCCESS_TOKEN_VALID;
                        }
                    }
                    else
                    {
                        returnModel.Code = EnumCommonCode.Error;
                        returnModel.Msg = UserApiResource.COMMON_ERROR_USERTOKEN_NOTFOUND;
                    }
                }
                else
                {
                    returnModel.Code = EnumCommonCode.Error;
                    returnModel.Msg = validateMsg;
                }

                returnModel.TokenKey = model.Token;
                jsonString = JsonConvert.SerializeObject(returnModel);
            }
            catch (Exception ex)
            {
                string strError = "Failed for CHECK USER TOKEN-API request: " + ex.Message;
                logger.ErrorException(strError, ex);
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = strError;

                jsonString = JsonConvert.SerializeObject(returnModel);
                return new JsonActionResult(HttpStatusCode.BadRequest, jsonString);
            }
            finally
            {
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [Route("getinfo")]
        [HttpPost]
        public async Task<IHttpActionResult> GetInfo(ApiGetUserInfoModel model)
        {

            CreateDocumentApi(model);
            var returnModel = new ResponseApiModel();
            var jsonString = string.Empty;
            var currentDomain = Request.RequestUri.Host;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            var validateMsg = string.Empty;
            //var errorCode = ValidateApiCallingTime(model.Time, ref validateMsg);
            var errorCode = 0;
            var isValid = (errorCode == EnumCommonCode.Success || errorCode == 0);
            try
            {
                logger.Debug("Begin GET USER INFO-API request");

                if (!isValid)
                {
                    returnModel.Code = errorCode;
                    returnModel.Msg = validateMsg;
                }

                if (isValid)
                {
                    var userIdentity = _userStore.ApiGetUserInfo(model.UserId, model.Token);
                    await Task.FromResult(userIdentity);

                    if (userIdentity != null)
                    {
                        returnModel.Code = EnumCommonCode.Success;
                        if (!string.IsNullOrEmpty(userIdentity.Avatar))
                        {
                            userIdentity.Avatar = CdnHelper.GetFullImgPath(userIdentity.Avatar);
                        }
                        returnModel.Data = userIdentity;
                        returnModel.Msg = UserApiResource.COMMON_SUCCESS;
                    }
                    else
                    {
                        returnModel.Code = EnumCommonCode.Success;
                        returnModel.Msg = UserApiResource.COMMON_ERROR_USERTOKEN_NOTFOUND;
                    }
                }

                returnModel.TokenKey = model.Token;
                jsonString = JsonConvert.SerializeObject(returnModel);

                logger.DebugFormat("GET USER INFO-API Returned Model encrypted = {0}", jsonString);
            }
            catch (Exception ex)
            {
                string strError = "Failed for GET USER INFO-API request: " + ex.Message;
                logger.ErrorException(strError, ex);
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = strError;

                jsonString = JsonConvert.SerializeObject(returnModel);
                return new JsonActionResult(HttpStatusCode.BadRequest, jsonString);
            }
            finally
            {
                logger.Debug("Ended GET USER INFO-API request");
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [Route("getlistprofile")]
        [HttpPost]
        public async Task<IHttpActionResult> GetListUserInfo(ApiListUserInfoModel model)
        {

            CreateDocumentApi(model);
            var returnModel = new ResponseApiModel();
            var jsonString = string.Empty;
            var currentDomain = Request.RequestUri.Host;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            var validateMsg = string.Empty;
            try
            {
                logger.Debug("Begin GetListUserInfo request");
                var listResult = new List<IdentityUser>();
                if (model.ListUserId != null && model.ListUserId.Count > 0)
                {
                    for (int i = 0; i < model.ListUserId.Count; i++)
                    {
                        var userId = model.ListUserId[i];
                        var itemUser = AccountHelper.GetUserInfoFromCache(userId);
                        if (itemUser != null)
                        {
                            listResult.Add(itemUser);
                            model.ListUserId.RemoveAt(i);
                            i--;
                        }

                    }
                }

                if (model.ListUserId != null && model.ListUserId.Count > 0)
                {
                    var strListUser = string.Join(",", model.ListUserId);

                    var listUser = AccountHelper.GetListUserProfile(strListUser);
                    await Task.FromResult(listUser);

                    returnModel.Msg = UserApiResource.COMMON_SUCCESS;

                    if (listUser != null && listUser.Count > 0)
                    {
                        foreach (var item in listUser)
                        {
                            if (string.IsNullOrEmpty(item.Avatar))
                            {
                                item.Avatar = CdnHelper.GetFullImgPath(item.Avatar);
                            }
                        }
                        listResult.AddRange(listUser);
                    }
                }

                returnModel.Data = listResult;

                jsonString = JsonConvert.SerializeObject(returnModel);

                logger.DebugFormat("Method GetListUserInfo Returned Model encrypted = {0}", jsonString);
            }
            catch (Exception ex)
            {
                string strError = "Failed for Method GetListUserInfo request: " + ex.Message;
                logger.ErrorException(strError, ex);
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = strError;

                jsonString = JsonConvert.SerializeObject(returnModel);
                return new JsonActionResult(HttpStatusCode.BadRequest, jsonString);
            }
            finally
            {
                logger.Debug("Ended Method GetListUserInfo request");
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [Route("getuserbyemail")]
        [HttpPost]
        public async Task<IHttpActionResult> GetUserByEmail(ApiGetUserByEmailModel model)
        {
            CreateDocumentApi(model);
            var returnModel = new ResponseApiModel();
            var jsonString = string.Empty;
            var currentDomain = Request.RequestUri.Host;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            var validateMsg = string.Empty;

            try
            {
                logger.Debug("Begin GET USER BY EMAIL-API request");


                var userIdentity = _userStore.GetUserByEmail(model.Email);
                await Task.FromResult(userIdentity);

                if (userIdentity != null)
                {
                    returnModel.Code = EnumCommonCode.Success;
                    if (!string.IsNullOrEmpty(userIdentity.Avatar))
                    {
                        userIdentity.Avatar = CdnHelper.GetFullImgPath(userIdentity.Avatar);
                    }
                    returnModel.Data = userIdentity;
                    returnModel.Msg = UserApiResource.COMMON_SUCCESS;
                }
                else
                {
                    returnModel.Code = EnumCommonCode.Success;
                    returnModel.Msg = UserApiResource.COMMON_ERROR_USERTOKEN_NOTFOUND;
                }


                returnModel.TokenKey = string.Empty;
                jsonString = JsonConvert.SerializeObject(returnModel);

                logger.DebugFormat("GET USER BY EMAIL-API Returned Model encrypted = {0}", jsonString);
            }
            catch (Exception ex)
            {
                string strError = "Failed for GETUSER BY EMAIL-API request: " + ex.Message;
                logger.ErrorException(strError, ex);
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = strError;

                jsonString = JsonConvert.SerializeObject(returnModel);
                return new JsonActionResult(HttpStatusCode.BadRequest, jsonString);
            }
            finally
            {
                logger.Debug("Ended GET USER BY EMAIL-API request");
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [Route("recoverpasswordstep1")]
        [HttpPost]
        public async Task<IHttpActionResult> RecoverPasswordStep1(ApiChangePassword1Model model)
        {

            CreateDocumentApi(model);
            var returnModel = new ResponseApiModel();
            var jsonString = string.Empty;
            var currentDomain = Request.RequestUri.Host;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            var validateMsg = string.Empty;

            try
            {
                logger.Debug("Begin GET USER BY EMAIL-API request");
                var identity = _userStore.GetUserById(model.UserId);

                var pwdHash = string.Empty;
                if (true)//identity.IsEmail)
                {
                    pwdHash = Utility.Md5HashingData(model.NewPassword);
                }

                var result = _userStore.RecoverPasswordStep1(model.UserId, pwdHash);


                await Task.FromResult(result);

                //SendEmail to user
                var sendMailStatus = SendEmailToRecoverPwd(identity.Id, identity.Email, model.PasswordType);
                returnModel.Code = sendMailStatus;
                if (sendMailStatus == EnumCommonCode.Success)
                {
                    returnModel.Msg = string.Format(EmailResource.NOTIF_EMAIL_SENT_FORMAT, identity.Email);
                }
                else
                {
                    returnModel.Msg = EmailResource.ERROR_SENDING_EMAIL_FAILED;
                }

                returnModel.Code = EnumCommonCode.Success;

                returnModel.Msg = UserApiResource.COMMON_SUCCESS;


                returnModel.TokenKey = string.Empty;
                jsonString = JsonConvert.SerializeObject(returnModel);

                logger.DebugFormat("GET USER BY EMAIL-API Returned Model encrypted = {0}", jsonString);
            }
            catch (Exception ex)
            {
                string strError = "Failed for GETUSER BY EMAIL-API request: " + ex.Message;
                logger.ErrorException(strError, ex);
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = strError;

                jsonString = JsonConvert.SerializeObject(returnModel);
                return new JsonActionResult(HttpStatusCode.BadRequest, jsonString);
            }
            finally
            {
                logger.Debug("Ended GET USER BY EMAIL-API request");
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [Route("recoverpasswordstep2")]
        [HttpPost]
        public async Task<IHttpActionResult> RecoverPasswordStep2(ApiChangePassword2Model model)
        {

            CreateDocumentApi(model);
            var returnModel = new ResponseApiModel();
            var jsonString = string.Empty;
            var currentDomain = Request.RequestUri.Host;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            var validateMsg = string.Empty;

            try
            {
                logger.Debug("Begin RecoverPasswordStep2 request");
                var result = _userStore.RecoverPasswordStep2(model.UserId, model.PasswordType);


                await Task.FromResult(result);

                returnModel.Code = result;
                if(returnModel.Code == EnumCommonCode.Success)
                    returnModel.Msg = UserApiResource.COMMON_SUCCESS;


                returnModel.TokenKey = string.Empty;
                jsonString = JsonConvert.SerializeObject(returnModel);

                logger.DebugFormat("RecoverPasswordStep2 Returned Model encrypted = {0}", jsonString);
            }
            catch (Exception ex)
            {
                string strError = "Failed for RecoverPasswordStep2 request: " + ex.Message;
                logger.ErrorException(strError, ex);
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = strError;

                jsonString = JsonConvert.SerializeObject(returnModel);
                return new JsonActionResult(HttpStatusCode.BadRequest, jsonString);
            }
            finally
            {
                logger.Debug("Ended RecoverPasswordStep2 request");
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [Route("recoverpasswordotp")]
        [HttpPost]
        public async Task<IHttpActionResult> RecoverPasswordOTP(ApiRecoverPasswordOTPModel model)
        {
            var returnModel = new ResponseApiModel();
            var jsonString = string.Empty;
            var currentDomain = Request.RequestUri.Host;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            var validateMsg = string.Empty;

            try
            {
                logger.Debug("Begin RecoverPasswordOTP request");
                var userInfo = _userStore.GetUserByInfo(model.PhoneNumber);

                //var pwdHash = string.Empty;
                //pwdHash = Utility.Md5HashingData(model.NewPassword);

                if(userInfo == null)
                {
                    returnModel.Msg = UserWebResource.COMMON_ERROR_INFO_NOTFOUND;
                    returnModel.Code = EnumCommonCode.Error;
                    return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
                }

                if (string.IsNullOrEmpty(userInfo.PhoneNumber))
                {
                    returnModel.Msg = UserWebResource.COMMON_ERROR_PHONE_INVALID;
                    returnModel.Code = EnumCommonCode.Error;
                    return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
                }

                var otpCode = Utility.GenerateOTPCode();
                var createOTPStatus = EnumCommonCode.Success;
                var actionType = ActionType.RecoverPassword1;

                if(model.PasswordLevel == PasswordLevelType.Level2)
                    actionType = ActionType.RecoverPassword2;

                var tranId = CreateNewOTPForAction(userInfo.Id, userInfo.TokenKey, otpCode, model.NewPassword, actionType, ref createOTPStatus);

                await Task.FromResult(tranId);
                if (createOTPStatus == EnumCommonCode.Success)
                {
                    if (!string.IsNullOrEmpty(tranId))
                    {
                        //Begin send OTP to user
                        SendOTPCodeToUser(userInfo.PhoneNumber, otpCode, tranId, OTPType.OTPSMS);
                    }

                    returnModel.Msg = string.Format(UserApiResource.COMMON_SUCCESS_OTPCODE_SENT, userInfo.PhoneNumber);
                    returnModel.Code = EnumCommonCode.Success;
                }
                else
                {
                    returnModel.Code = EnumCommonCode.Error;
                    returnModel.Msg = tranId;
                }

                returnModel.Data = userInfo.Id;
                returnModel.TokenKey = string.Empty;
                jsonString = JsonConvert.SerializeObject(returnModel);

                logger.DebugFormat("RecoverPasswordOTP Returned Model encrypted = {0}", jsonString);
            }
            catch (Exception ex)
            {
                string strError = "Failed for RecoverPasswordOTP request: " + ex.Message;
                logger.ErrorException(strError, ex);
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = strError;

                jsonString = JsonConvert.SerializeObject(returnModel);
                return new JsonActionResult(HttpStatusCode.BadRequest, jsonString);
            }
            finally
            {
                logger.Debug("Ended RecoverPasswordOTP request");
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [Route("checkpwd2isvalid")]
        [HttpPost]
        public async Task<IHttpActionResult> CheckPwd2IsValid(ApiCheckPwd2IsValidModel model)
        {

            CreateDocumentApi(model);
            var returnModel = new ResponseApiModel();
            var jsonString = string.Empty;
            var currentDomain = Request.RequestUri.Host;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            var validateMsg = string.Empty;
            var errorCode = ValidateApiCallingTime(model.Time, ref validateMsg);

            var isValid = (errorCode == EnumCommonCode.Success || errorCode == 0);
            try
            {
                logger.Debug("Begin CHECK_PW2_VALID -API request");

                if (!isValid)
                {
                    returnModel.Code = errorCode;
                    returnModel.Msg = validateMsg;
                }

                if (isValid)
                {
                    var result = _userStore.CheckPwd2IsValid(new IdentityUser { Id = model.UserId, PasswordHash2 = model.Pwd, TokenKey = model.Token });

                    await Task.FromResult(result);
                    if (result == EnumCommonCode.Success)
                    {
                        returnModel.Msg = UserApiResource.SUCCESS_PWD2_CORRECT;
                    }
                    else if (result == EnumCommonCode.Error_UserOrTokenNotFound)
                    {
                        returnModel.Msg = UserApiResource.COMMON_ERROR_USERTOKEN_NOTFOUND;
                    }
                    else
                    {
                        returnModel.Msg = UserApiResource.ERROR_PWD2_INCORRECT;
                    }

                    returnModel.Code = result;
                }

                returnModel.TokenKey = model.Token;
                jsonString = JsonConvert.SerializeObject(returnModel);

                logger.DebugFormat("CHECK_PW2_VALID-API Returned Model encrypted = {0}", jsonString);
            }
            catch (Exception ex)
            {
                string strError = "Failed for CHECK_PW2_VALID-API request: " + ex.Message;
                logger.ErrorException(strError, ex);
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = strError;

                jsonString = JsonConvert.SerializeObject(returnModel);
                return new JsonActionResult(HttpStatusCode.BadRequest, jsonString);
            }
            finally
            {
                logger.Debug("Ended CHECK_PW2_VALID-API request");
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        #region Helpers

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

        private int ValidateLoginData(ApiAuthLoginModel model, ref string message)
        {
            var errorCode = 0;
            errorCode = ValidateApiCallingTime(model.Time, ref message);

            if (errorCode == 0)
            {
                var hashData = HashingSignInData(model);
                if (model.Hash != hashData)
                {
                    errorCode = EnumCommonCode.ErrorHashing;
                    message = UserApiResource.COMMON_ERROR_HASH_INCORRECT;
                }
            }

            return errorCode;
        }

        private int ValidateCheckUserExistsData(ApiCheckUserExistsModel model, ref string message)
        {
            var errorCode = 0;
            errorCode = ValidateApiCallingTime(model.Time, ref message);

            if (errorCode == 0)
            {
                var token = Utility.Md5HashingData(string.Format("{0}|{1}", model.UserName, model.Time));
                if (token != model.Token)
                {
                    errorCode = EnumCommonCode.ErrorHashing;
                    message = UserApiResource.COMMON_ERROR_TOKEN_INVALID;
                }
            }

            return errorCode;
        }

        private ApiAuthLoginModel ReFormatLoginModel(ApiAuthLoginModel model)
        {
            if (model != null)
            {
                model.UserName = model.UserName.Trim();
            }

            return model;
        }

        private ApiAuthLoginWithModel ReFormatLoginWithModel(ApiAuthLoginWithModel model)
        {
            if (model != null)
            {
                model.UserName = model.UserName.Trim();
            }

            return model;
        }

        private int BeginResendEmail(IdentitySystemEmail emailInfo)
        {
            var result = EnumCommonCode.Success;
            try
            {
                var hasTimeout = false;
                var emailModel = new EmailInputModel();

                emailModel.body = emailInfo.Body;               
                emailModel.sendto = emailInfo.Receiver;
                emailModel.subject = emailInfo.Subject;
               
                //Send email
                EmailService.Send(emailModel, ref hasTimeout);
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to Send Email to [0] due to {1}", emailInfo.Receiver, ex.Message);
                logger.Error(strError);
                result = EnumCommonCode.Error;
            }

            return result;
        }

        private int SendEmailToRecoverPwd(int userId, string email, string pwdType)
        {
            var result = EnumCommonCode.Success;
            try
            {
                var hasTimeout = false;
                var emailModel = new EmailInputModel();

                emailModel.body = EmailHelper.GetEmailTemplate("RecoverPassword");

                var recoverData = string.Format("{0}|{1}", userId, pwdType);
                //var dataEncrypt = Utility.TripleDESEncrypt(SystemSettings.EncryptKey, recoverData);
                var dataEncrypt = Utility.EncryptText(recoverData, SystemSettings.EncryptKey);

                var rawData = string.Format("{0}.{1}.{2}", Utility.Md5HashingData(DateTime.Now.ToString(Constant.DATEFORMAT_yyyyMMddHHmmss)), dataEncrypt, Utility.Md5HashingData(userId.ToString()));

                var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
                var recoverLink = string.Format("{0}/{1}/{2}?token={3}", SystemSettings.FrontendURL, "WebAuth", "Password_Reset", rawData);
                if (!string.IsNullOrEmpty(emailModel.body))
                {
                    emailModel.body = emailModel.body.Replace(EmailTemplateConst.Receiver, email);
                    emailModel.body = emailModel.body.Replace(EmailTemplateConst.RecoverPwdLink, recoverLink);
                }
                emailModel.sendto = email;
                emailModel.subject = EmailResource.SUBJECT_RECOVER_PASSWORD;

                var emailStore = GlobalContainer.IocContainer.Resolve<IStoreSystemEmail>();
                var emailInfo = new IdentitySystemEmail();
                emailInfo.Subject = emailModel.subject;
                emailInfo.Body = emailModel.body;
                emailInfo.Sender = string.Empty;
                emailInfo.Receiver = email;
                emailInfo.ReceiverId = userId;
                emailInfo.Action = (pwdType == PasswordLevelType.Level1) ? ActionType.RecoverPassword1 : ActionType.RecoverPassword2;

                //Create new email
                emailStore.Insert(emailInfo);

                //Send email
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

        private string HashingSignInData(ApiAuthLoginModel model)
        {
            var rawDataToHash = string.Format("{0}|{1}|{2}", model.UserName, model.Password, model.Time);

            return Utility.Md5HashingData(rawDataToHash);
        }

        private string GenereateHashingForNewAccount(string userName, string email)
        {
            var recoverData = string.Format("{0}|{1}", userName, email);
            var dataEncrypt = Utility.EncryptText(recoverData, SystemSettings.EncryptKey);

            var rawData = string.Format("{0}.{1}.{2}", Utility.Md5HashingData(DateTime.Now.ToString(Constant.DATEFORMAT_yyyyMMddHHmmss)), dataEncrypt, Utility.Md5HashingData(userName));

            return rawData;
        }

        private string GenerateTokenKey(string myStr)
        {
            var tokenKey = string.Empty;
            var clientIp = HttpContext.Current.Request.UserHostAddress;
            var dtNow = DateTime.Now.ToString(Constant.DATEFORMAT_yyyyMMddHHmmss);

            //Rule: MD5(< username >|< DateTime.Now.ToString(yyyyMMddHHmmsss) >|< clientIP >
            var rawTokenStr = string.Format("{0}|{1}|{2}", myStr, dtNow, clientIp);

            tokenKey = Utility.Md5HashingData(rawTokenStr);

            return tokenKey;
        }

        private UserLogIdentity GetDataForLogging(object model)
        {
            var loginIdentity = new UserLogIdentity();
            var currentContext = HttpContext.Current.Request;
            loginIdentity.UserIp = currentContext.UserHostAddress;
            loginIdentity.UserAgent = currentContext.UserAgent;
            loginIdentity.Method = ActionMethod.Api;
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

        private int ValidateApiCallingTime(string time, ref string message)
        {
            var errorCode = 0;
            if (!string.IsNullOrEmpty(time))
            {
                try
                {
                    var dtLogin = DateTime.ParseExact(time, Constant.DATEFORMAT_yyyyMMddHHmmss, null);
                    if (dtLogin.AddHours(SingleSignOnSettings.ApiLoginTimeValidInHours) < DateTime.Now)
                    {
                        errorCode = EnumCommonCode.ErrorTimeInvalid;
                        message = UserApiResource.COMMON_ERROR_TIME_INVALID;
                    }
                }
                catch
                {
                    errorCode = EnumCommonCode.ErrorTimeInvalid;
                    message = UserApiResource.COMMON_ERROR_TIME_INVALID;
                }
            }
            return errorCode;
        }

        private string GetOTPResponseMessage(int statusCode, string type = "create", string action = "")
        {
            var message = string.Empty;
            if (statusCode == EnumCommonCode.Success)
            {
                if (type == "create")
                {
                    message = UserApiResource.OTP_SUCCESS_CREATE;
                }
                else
                {
                    if (action == ActionType.ChangePassword2)
                    {
                        message = UserApiResource.COMMON_SUCCESS_CHANGEPWD2;
                    }
                    else
                    {
                        message = UserApiResource.OTP_SUCCESS_VERIFY;
                    }
                }
            }
            else if (statusCode == EnumCreateAndVerifyOTPCodeStatus.Error_OTPCodeNotExistsOrUsed)
            {
                message = UserApiResource.OTP_ERROR_OTPCODE_NOTEXISTS_ORUSED;
            }
            else if (statusCode == EnumCommonCode.Error_UserOrTokenNotFound)
            {
                message = UserApiResource.COMMON_ERROR_USERTOKEN_NOTFOUND;
            }
            else if (statusCode == EnumCreateAndVerifyOTPCodeStatus.Error_OTPCodeExpired)
            {
                message = UserApiResource.OTP_ERROR_OTPCODE_EXPIRED;
            }
            else if (statusCode == EnumCreateAndVerifyOTPCodeStatus.Error_Pwd2NotCorrect)
            {
                message = UserApiResource.ERROR_PWD2_INCORRECT;
            }

            return message;
        }

        private void ClearLoginDataCachedById(int userId, string domain)
        {
            try
            {
                var currentUser = _userStore.GetUserById(userId);
                var redisKey = currentUser.UserName + currentUser.PasswordHash;
                if (currentUser != null)
                {
                    _redisService.ClearCachedDataByKeyAndDomain(currentUser.UserName + currentUser.PasswordHash, domain);
                    _redisService.ClearCachedDataByKeyAndDomain(currentUser.Email + currentUser.PasswordHash, domain);
                    _redisService.ClearCachedDataByKeyAndDomain(currentUser.PhoneNumber + currentUser.PasswordHash, domain);
                }
            }
            catch (Exception ex)
            {
                var strError = "Could not GetUserById to CleareUserCache due to: {0}";
                strError = string.Format(ex.Message);
            }
        }

        private string GetActiveAccountMessage(int statusCode)
        {
            var message = string.Empty;
            if (statusCode == EnumCommonCode.Success)
            {
                message = UserApiResource.COMMON_SUCCESS;
            }
            else if (statusCode == EnumCommonCode.Error_Info_NotFound)
            {
                message = UserApiResource.COMMON_INFO_NOTFOUND;
            }
            else if (statusCode == EnumCommonCode.ErrorHashing)
            {
                message = UserApiResource.COMMON_ERROR_HASH_INCORRECT;
            }

            return message;
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

        #endregion
    }
}
