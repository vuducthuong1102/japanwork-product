using System;
using System.Linq;
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
using SingleSignOn.Helpers.Resource;
using Manager.WebApp.Helpers;
using System.Resources;
using System.Reflection;
using SingleSignOn.ShareLibs.Common;
using static SingleSignOn.ShareLibs.Common.EnumCommon;
using System.Text.RegularExpressions;
using Autofac;

namespace SingleSignOn.Controllers
{
    [Authorize]
    [RoutePrefix("api/user")]
    public class ApiAccountController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiAccountController>();
        private IApiAuthRedisService _redisService;
        private IStoreUser _userStore;

        public ApiAccountController(IApiAuthRedisService redisService, IStoreUser userStore)
        {
            _redisService = redisService;
            _userStore = userStore;
        }

        [Route("search")]
        [HttpPost]
        public async Task<IHttpActionResult> SearchUser(ApiFilterModel model)
        {
            CreateDocumentApi(model);
            var returnModel = new ResponseApiModel();
            var jsonString = string.Empty;
            var currentDomain = Request.RequestUri.Host;
            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                logger.Debug("Begin SearchUser request");

                //Check and register from database
                var returnCode = EnumCommonCode.Success;
                var filterIdentity = ParseFilterIdentity(model);

                var result = _userStore.GetByPage(filterIdentity);
                await Task.FromResult(result);

                returnModel.Data = result;
                returnModel.Code = returnCode;
                returnModel.Msg = ApiResourceHelper.GetMessageByCode(returnCode);

                jsonString = JsonConvert.SerializeObject(returnModel);
                logger.DebugFormat("SearchUser Returned Model encrypted = {0}", jsonString);
            }
            catch (Exception ex)
            {
                string strError = "Failed for SearchUser request: " + ex.Message;
                logger.ErrorException(strError, ex);
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = strError;

                jsonString = JsonConvert.SerializeObject(returnModel);
                return new JsonActionResult(HttpStatusCode.BadRequest, jsonString);
            }
            finally
            {
                logger.Debug("Ended SearchUser request");
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        private IdentityFilter ParseFilterIdentity(ApiFilterModel model)
        {
            var filterIdentity = new IdentityFilter
            {
                Keyword = model.Keyword,
                PageIndex = model.PageIndex,
                PageSize = model.PageSize,
                UserId = model.UserId,
                TokenKey = model.TokenKey
            };

            if (string.IsNullOrEmpty(model.Keyword))
            {
                filterIdentity.Keyword = string.Empty;
            }

            return filterIdentity;
        }


        [Route("register")]
        [HttpPost]
        public async Task<IHttpActionResult> Register(ApiRegisterModel model)
        {
            CreateDocumentApi(model);
            var returnModel = new ResponseApiModel();
            var jsonString = string.Empty;
            var currentDomain = Request.RequestUri.Host;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            model = ReFormatRegisterModel(model);
            var validateMsg = string.Empty;
            var errorCode = ValidateRegisterData(model, ref validateMsg);

            var isValid = (errorCode == EnumCommonCode.Success || errorCode == 0);
            try
            {
                logger.Debug("Begin REGISTER-API request");

                if (!isValid)
                {
                    returnModel.Code = errorCode;
                    returnModel.Msg = validateMsg;
                }

                if (isValid)
                {
                    var birthday = DateTime.Now;
                    if (!string.IsNullOrEmpty(model.Birthday))
                    {
                        birthday = DateTime.ParseExact(model.Birthday, Constant.DATE_FORMAT_ddMMyyyy, null);
                    }
                    var sex = UserSex.Male;
                    try
                    {
                        sex = Convert.ToInt32(model.Sex);
                    }
                    catch
                    {
                    }

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
                        OTPType = OTPType.OTPSMS,
                        AppCode = model.AppCode
                    };

                    //Generate for email activation
                    var isPhoneNumber = IsPhoneNumber(model.UserName);
                    var isEmail = IsEmail(model.UserName);

                    var hashingData = string.Empty;
                    if (isEmail)
                    {
                        hashingData = GenereateHashingForNewAccount(model.UserName, model.UserName);
                    }

                    if (isPhoneNumber)
                        userIdentity.PhoneNumber = model.UserName;

                    if (isEmail)
                        userIdentity.Email = model.UserName;

                    userIdentity.SecurityStamp = hashingData;

                    //Check and register from database
                    var code = EnumCommonCode.Success;
                    var result = _userStore.ApiRegister(userIdentity, ref code);
                    await Task.FromResult(result);

                    returnModel.Data = result;
                    returnModel.Code = code;

                    if (code == 1)
                    {
                        //SendEmail to user
                        //var sendMailStatus = SendEmailToActiveAccount(model.UserName, hashingData, result);
                        var sendMailStatus = SendEmailToActiveAccount_WithoutAmazon(model.UserName, hashingData);
                        if (sendMailStatus == EnumCommonCode.Success)
                        {
                            //Check and register from database                           
                            //var result = _userStore.ApiRegister(userIdentity, ref code);
                            //await Task.FromResult(result);

                            //returnModel.Data = result;
                            //returnModel.Code = code;

                            //if (gentokenResult)
                            {
                                //Write log successfully
                                var logInfo = GetDataForLogging(model);
                                logInfo.Domain = currentDomain;
                                logInfo.ActionType = ActionType.Register;
                                logInfo.ActionDesc = string.Format("User [{0}] was registered successfully. Need to be done by account activation", result);

                                //Write log 
                                _userStore.WriteUserLog(logInfo, SingleSignOnSettings.AllowToTraceLoginRequest);
                            }

                            returnModel.Msg = string.Format(EmailResource.NOTIF_EMAIL_SENT_FORMAT, model.UserName);
                        }
                        else
                        {
                            returnModel.Msg = EmailResource.ERROR_SENDING_EMAIL_FAILED;
                        }
                    }
                    else
                    {
                        returnModel.Msg = GetRegisterResponseMessage(code);
                    }
                }

                jsonString = JsonConvert.SerializeObject(returnModel);

                logger.DebugFormat("REGISTER-API Returned Model encrypted = {0}", jsonString);
            }
            catch (Exception ex)
            {
                string strError = "Failed for REGISTER-API request: " + ex.Message;
                logger.ErrorException(strError, ex);
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = UserApiResource.COMMON_ERROR_SYSTEM;

                jsonString = JsonConvert.SerializeObject(returnModel);
                return new JsonActionResult(HttpStatusCode.OK, jsonString);
            }
            finally
            {
                logger.Debug("Ended REGISTER-API request");
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [Route("changepwd")]
        [HttpPost]
        public async Task<IHttpActionResult> ChangePassword(ApiChangePasswordModel model)
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
            var errorCode = ValidateChangePwdData(model, ref validateMsg);

            var isValid = (errorCode == EnumCommonCode.Success || errorCode == 0);
            try
            {
                logger.Debug("Begin CHANGE PASSWORD-API request");

                if (!isValid)
                {
                    returnModel.Code = errorCode;
                    returnModel.Msg = validateMsg;
                }

                if (isValid)
                {
                    var userIdentity = _userStore.GetUserById(model.UserId);
                    var changePwdType = (model.PwdType.ToLower() == "level1") ? UserPasswordLevel.Level1 : UserPasswordLevel.Level2;
                    if (userIdentity != null)
                    {
                        var result = _userStore.ChangePassword(
                            new IdentityUser
                            {
                                Id = model.UserId,
                                PasswordHash = model.OldPwd1,
                                PasswordHash2 = model.OldPwd2,
                                NewPassword = model.NewPwd,
                                TokenKey = model.Token
                            }, changePwdType);

                        await Task.FromResult(result);
                        if (result == EnumCommonCode.Success)
                        {
                            if (changePwdType == UserPasswordLevel.Level2)
                            {
                                //SendOTPCodeToUser(model.UserId, model.Token, userIdentity.PhoneNumber, userIdentity.OTPType);
                                var otpCode = Utility.GenerateOTPCode();
                                var createOTPStatus = EnumCommonCode.Success;
                                var tranId = CreateNewOTPForAction(model.UserId, model.Token, otpCode, model.NewPwd, ActionType.ChangePassword2, ref createOTPStatus);

                                if (createOTPStatus == EnumCommonCode.Success)
                                {
                                    if (!string.IsNullOrEmpty(tranId))
                                    {
                                        SendOTPCodeToUser(userIdentity.PhoneNumber, otpCode, tranId, userIdentity.OTPType);
                                    }

                                    returnModel.Msg = UserApiResource.COMMON_SUCCESS_OTPCODE_SENT;
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
                                returnModel.Code = EnumCommonCode.Success;
                                returnModel.Msg = GetChangePwdResponseMessage(returnModel.Code, changePwdType);
                            }

                            //Write log successfully
                            var logInfo = GetDataForLogging(model);
                            logInfo.Domain = currentDomain;
                            logInfo.ActionType = (changePwdType == UserPasswordLevel.Level1) ? ActionType.ChangePassword1 : ActionType.ChangePassword2;
                            logInfo.ActionDesc = string.Format("User [{0}] changed password with token [{1}]", model.UserId, model.Token);

                            //Write log 
                            _userStore.WriteUserLog(logInfo, SingleSignOnSettings.AllowToTraceLoginRequest);
                            //Clear old pwd cached
                            ClearLoginDataCachedById(userIdentity, currentDomain);
                        }
                        else
                        {
                            returnModel.Code = result;
                            returnModel.Msg = GetChangePwdResponseMessage(returnModel.Code, changePwdType);
                        }
                    }
                    else
                    {
                        returnModel.Code = EnumCommonCode.Error_UserOrTokenNotFound;
                        returnModel.Msg = GetChangePwdResponseMessage(returnModel.Code, changePwdType);
                    }

                    returnModel.TokenKey = model.Token;
                }

                jsonString = JsonConvert.SerializeObject(returnModel);

                logger.DebugFormat("CHANGE PASSWORD-API Returned Model encrypted = {0}", jsonString);
            }
            catch (Exception ex)
            {
                string strError = "Failed for CHANGE PASSWORD-API request: " + ex.Message;
                logger.ErrorException(strError, ex);
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = UserApiResource.COMMON_ERROR_SYSTEM;

                jsonString = JsonConvert.SerializeObject(returnModel);
                return new JsonActionResult(HttpStatusCode.OK, jsonString);
            }
            finally
            {
                logger.Debug("Ended CHANGE PASSWORD-API request");
            }


            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [Route("changeauthmethod")]
        [HttpPost]
        public async Task<IHttpActionResult> ChangeAuthMethod(ApiChangeAuthMethodModel model)
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
                logger.Debug("Begin CHANGE AUTH METHOD-API request");

                if (!isValid)
                {
                    returnModel.Code = errorCode;
                    returnModel.Msg = validateMsg;
                }

                if (isValid)
                {
                    var userIdentity = _userStore.GetUserById(model.UserId);
                    if (userIdentity != null)
                    {
                        //SendOTPCodeToUser(model.UserId, model.Token, userIdentity.PhoneNumber, userIdentity.OTPType);
                        var otpCode = Utility.GenerateOTPCode();
                        var createOTPStatus = EnumCommonCode.Success;
                        var tranId = CreateNewOTPForAction(model.UserId, model.Token, otpCode, model.AuthMethod, ActionType.ChangeAuthMethod, ref createOTPStatus, userIdentity.OTPType);
                        await Task.FromResult(tranId);

                        if (createOTPStatus == EnumCommonCode.Success)
                        {
                            if (!string.IsNullOrEmpty(tranId))
                            {
                                //Send OTP code by the old method
                                SendOTPCodeToUser(userIdentity.PhoneNumber, otpCode, tranId, userIdentity.OTPType);
                            }

                            returnModel.Msg = UserApiResource.COMMON_SUCCESS_OTPCODE_SENT;
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

                    returnModel.TokenKey = model.Token;
                }

                jsonString = JsonConvert.SerializeObject(returnModel);

                logger.DebugFormat("CHANGE AUTH METHOD-API Returned Model encrypted = {0}", jsonString);
            }
            catch (Exception ex)
            {
                string strError = "Failed for CHANGE AUTH METHOD-API request: " + ex.Message;
                logger.ErrorException(strError, ex);
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = UserApiResource.COMMON_ERROR_SYSTEM;

                jsonString = JsonConvert.SerializeObject(returnModel);
                return new JsonActionResult(HttpStatusCode.OK, jsonString);
            }
            finally
            {
                logger.Debug("Ended CHANGE AUTH METHOD-API request");
            }


            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }


        //[Route("actionfollow")]
        //[HttpPost]
        //public async Task<IHttpActionResult> ActionFollow(ApiUserActionModel model)
        //{
        //    var returnModel = new ResponseApiModel();
        //    var jsonString = string.Empty;
        //    var currentDomain = Request.RequestUri.Host;

        //    if (!ModelState.IsValid)
        //    {
        //        return ModelValidation.ApiValidate(ModelState, returnModel);
        //    }

        //    var validateMsg = string.Empty;
        //    try
        //    {
        //        logger.Debug("Begin USER-FOLLOW request");

        //            var result = _userStore.UserFollow(
        //                new IdentityUserAction
        //                {
        //                  Status=model.Status,
        //                  ActionType=model.ActionType,
        //                  UserId=model.UserId,
        //                  UserActionId=model.UserActionId,
        //                  TokenKey=model.TokenKey
        //                }
        //            );

        //            await Task.FromResult(result);
        //            if (result == EnumCommonCode.Success)
        //            {                        
        //                returnModel.Code = EnumCommonCode.Success;
        //            }
        //            else
        //            {
        //                returnModel.Code = result;
        //            }

        //            returnModel.Msg = GetUpdateProfleResponseMessage(result);
        //            returnModel.TokenKey = model.TokenKey;

        //        jsonString = JsonConvert.SerializeObject(returnModel);

        //        //Clear both user cached data 
        //        AccountHelper.ClearUserCache(model.UserId);
        //        AccountHelper.ClearUserCache(model.UserActionId);

        //        logger.DebugFormat("FOLLOW-API Returned Model encrypted = {0}", jsonString);
        //    }
        //    catch (Exception ex)
        //    {
        //        string strError = "Failed for FOLLOW-API request: " + ex.Message;
        //        logger.ErrorException(strError, ex);
        //        returnModel.Code = EnumCommonCode.Error;
        //         returnModel.Msg = UserApiResource.COMMON_ERROR_SYSTEM;

        //        jsonString = JsonConvert.SerializeObject(returnModel);
        //        return new JsonActionResult(HttpStatusCode.OK, jsonString);
        //    }
        //    finally
        //    {
        //        logger.Debug("Ended FOLLOW-API request");
        //    }


        //    return new JsonActionResult(HttpStatusCode.OK, jsonString);
        //}

        [Route("recoverpassword")]
        [HttpPost]
        public async Task<IHttpActionResult> RecoverPassword(ApiRecoverPasswordModel model)
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
            errorCode = ValidateRecoverPwdData(model, ref validateMsg);

            var isValid = (errorCode == EnumCommonCode.Success || errorCode == 0);
            try
            {
                logger.Debug("Begin RECOVER PASSWORD-API request");

                if (!isValid)
                {
                    returnModel.Code = errorCode;
                    returnModel.Msg = validateMsg;
                }

                if (isValid)
                {
                    if (model.QuestionId == RecoverPasswordMethod.EMAIL)
                    {
                        var userIdentity = _userStore.GetUserByEmail(model.Answer);

                        await Task.FromResult(userIdentity);
                        if (userIdentity != null)
                        {
                            if (model.QuestionId == RecoverPasswordMethod.EMAIL)
                            {
                                model.UserId = userIdentity.Id;
                                _userStore.RecoverPasswordStep1(userIdentity.Id, model.Password);

                                //SendEmail to user
                                var sendMailStatus = SendEmailToRecoverPwd(model, model.Answer);
                                returnModel.Code = sendMailStatus;
                                if (sendMailStatus == EnumCommonCode.Success)
                                {
                                    returnModel.Msg = string.Format(EmailResource.NOTIF_EMAIL_SENT_FORMAT, userIdentity.Email);
                                }
                                else
                                {
                                    returnModel.Msg = EmailResource.ERROR_SENDING_EMAIL_FAILED;
                                }
                            }
                        }
                        else
                        {
                            returnModel.Code = EnumCommonCode.Error_UserOrTokenNotFound;
                            returnModel.Msg = UserApiResource.COMMON_ERROR_USERTOKEN_NOTFOUND;
                        }
                    }

                }

                jsonString = JsonConvert.SerializeObject(returnModel);

                logger.DebugFormat("RECOVER PASSWORD-API Returned Model encrypted = {0}", jsonString);
            }
            catch (Exception ex)
            {
                string strError = "Failed for RECOVER PASSWORD-API request: " + ex.Message;
                logger.ErrorException(strError, ex);
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = UserApiResource.COMMON_ERROR_SYSTEM;

                jsonString = JsonConvert.SerializeObject(returnModel);
                return new JsonActionResult(HttpStatusCode.OK, jsonString);
            }
            finally
            {
                logger.Debug("Ended RECOVER PASSWORD-API request");
            }


            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [Route("profile")]
        [HttpPost]
        public async Task<IHttpActionResult> GetUserProfile(ApiUserModel model)
        {
            CreateDocumentApi(model);
            var returnModel = new ResponseApiModel();
            var jsonString = string.Empty;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                logger.Debug("Begin for Api_GetUserProfile request");

                //Check and register from database
                var returnCode = EnumCommonCode.Success;

                var result = AccountHelper.GetUserProfile(model.UserId);
                await Task.FromResult(result);

                returnModel.Data = result;
                returnModel.Code = returnCode;
                returnModel.Msg = ApiResourceHelper.GetMessageByCode(returnCode);

                jsonString = JsonConvert.SerializeObject(returnModel);
                logger.DebugFormat("Api_GetUserProfile Returned Model encrypted = {0}", jsonString);
            }
            catch (Exception ex)
            {
                string strError = "Failed for Api_GetUserProfile request: " + ex.Message;
                logger.ErrorException(strError, ex);
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = UserApiResource.COMMON_ERROR_SYSTEM;

                jsonString = JsonConvert.SerializeObject(returnModel);
                return new JsonActionResult(HttpStatusCode.OK, jsonString);
            }
            finally
            {
                logger.Debug("Ended Api_GetUserProfile request");
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [Route("updateprofile")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateProfile(ApiUpdateProfileModel model)
        {
            CreateDocumentApi(model);
            var returnModel = new ResponseApiModel();
            var jsonString = string.Empty;
            var currentDomain = Request.RequestUri.Host;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            var errorMsg = string.Empty;
            var errorCode = EnumCommonCode.Success;
            try
            {
                logger.Debug("Begin UPDATE PROFILE-API request");

                var userIdentity = _userStore.GetUserById(model.UserId);

                await Task.FromResult(userIdentity);
                if (userIdentity != null)
                {
                    var currentProperty = model.FieldName.ToLower();
                    if (currentProperty == nameof(userIdentity.Email).ToLower())
                    {
                        userIdentity.Email = model.FieldValue;
                    }
                    else if (currentProperty == nameof(userIdentity.DisplayName).ToLower())
                    {
                        userIdentity.DisplayName = model.FieldValue;
                    }
                    else if (currentProperty == nameof(userIdentity.PhoneNumber).ToLower())
                    {
                        userIdentity.PhoneNumber = model.FieldValue;
                    }
                    else if (currentProperty == nameof(userIdentity.Birthday).ToLower())
                    {
                        errorCode = ValidateBirthday(model.FieldValue, ref errorMsg);
                        if (errorCode == EnumCommonCode.Success)
                        {
                            var birthday = (model.FieldValue == null) ? DateTime.Now : DateTime.ParseExact(model.FieldValue, Constant.DATE_FORMAT_ddMMyyyy, null);
                            userIdentity.Birthday = birthday;
                        }
                    }

                    if (errorCode == EnumCommonCode.Success)
                    {
                        var result = _userStore.UpdateProfile(userIdentity);
                        if (result == EnumCommonCode.Success)
                        {
                            //Write log successfully
                            var logInfo = GetDataForLogging(model);
                            logInfo.Domain = currentDomain;
                            logInfo.ActionType = ActionType.UpdateProfile;
                            logInfo.ActionDesc = string.Format("User [{0}] updated profile successfully", model.UserId);

                            //Write log 
                            _userStore.WriteUserLog(logInfo, SingleSignOnSettings.AllowToTraceLoginRequest);

                            //Clear cache info
                            AccountHelper.ClearUserCache(model.UserId);
                        }

                        returnModel.Code = result;
                        returnModel.Msg = GetUpdateProfleResponseMessage(result);
                    }
                    else
                    {
                        returnModel.Code = errorCode;
                        returnModel.Msg = errorMsg;
                    }
                }
                else
                {
                    returnModel.Code = EnumCommonCode.Error_UserOrTokenNotFound;
                    returnModel.Msg = UserApiResource.COMMON_ERROR_USERTOKEN_NOTFOUND;
                }

                jsonString = JsonConvert.SerializeObject(returnModel);

                logger.DebugFormat("UPDATE PROFILE-API Returned Model encrypted = {0}", jsonString);
            }
            catch (Exception ex)
            {
                string strError = "Failed for UPDATE PROFILE-API request: " + ex.Message;
                logger.ErrorException(strError, ex);
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = UserApiResource.COMMON_ERROR_SYSTEM;

                jsonString = JsonConvert.SerializeObject(returnModel);
                return new JsonActionResult(HttpStatusCode.OK, jsonString);
            }
            finally
            {
                logger.Debug("Ended UPDATE PROFILE request");
            }


            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        #region Helpers

        private int ValidateRegisterData(ApiRegisterModel model, ref string message)
        {
            var errorCode = 0;
            errorCode = ValidateApiCallingTime(model.Time, ref message);

            //if (errorCode == 0)
            //{
            //    if (!string.IsNullOrEmpty(model.Birthday))
            //    {
            //        try
            //        {
            //            var dt = DateTime.ParseExact(model.Birthday, Constant.DATE_FORMAT_ddMMyyyy, null);
            //        }
            //        catch
            //        {
            //            errorCode = EnumCommonCode.ErrorTimeInvalid;
            //            message = UserApiResource.REGISTER_ERROR_BIRTHDAY_INVALID;
            //        }
            //    }
            //}
            return errorCode;
        }

        private int ValidateChangePwdData(ApiChangePasswordModel model, ref string message)
        {
            var errorCode = 0;
            errorCode = ValidateApiCallingTime(model.Time, ref message);

            if (errorCode == 0)
            {
                if (!string.IsNullOrEmpty(model.PwdType))
                {
                    if (model.PwdType.ToLower() == PasswordLevelType.Level1)
                    {
                        if (string.IsNullOrEmpty(model.OldPwd1))
                        {
                            errorCode = EnumCommonCode.Error;
                            message = UserApiResource.COMMON_ERROR_PASSWORD_NULL;
                        }
                    }
                    else if (model.PwdType.ToLower() == PasswordLevelType.Level2)
                    {
                        //if (string.IsNullOrEmpty(model.OldPwd2))
                        //{
                        //    errorCode = EnumCommonCode.Error;
                        //    message = UserApiResource.COMMON_ERROR_PASSWORD_NULL;
                        //}
                    }
                    else
                    {
                        errorCode = EnumCommonCode.Error_PwdTypeInvalid;
                    }
                }
            }
            return errorCode;
        }

        private int ValidateRecoverPwdData(ApiRecoverPasswordModel model, ref string message)
        {
            var errorCode = 0;
            errorCode = ValidateApiCallingTime(model.Time, ref message);

            if (errorCode == 0)
            {
                var isValidMethod = false;
                foreach (var field in typeof(RecoverPasswordMethod).GetFields())
                {
                    if (model.QuestionId.StartsWith(field.GetValue(null).ToString()))
                    {
                        isValidMethod = true;
                        break;
                    }
                }

                if (!isValidMethod)
                {
                    errorCode = EnumCommonCode.Error_RecoverPwdMethod_Invalid;
                    message = UserApiResource.COMMON_ERROR_RECOVERPWD_METHOD_INVALID;
                }
            }

            //if (errorCode == 0)
            //{
            //    var hashData = HashingRecoverPwdData(model);
            //    if (model.Hash != hashData)
            //    {
            //        errorCode = EnumCommonCode.ErrorHashing;
            //        message = UserApiResource.COMMON_ERROR_HASH_INCORRECT;
            //    }
            //}

            return errorCode;
        }

        private string HashingRecoverPwdData(ApiRecoverPasswordModel model)
        {
            //MD5(<MD5(password)>|<MD5(repassword)>|<time>|< pwdType>|<questionID>)
            var hashingFormat = "{0}|{1}|{2}|{3}|{4}";
            var rawDataToHash = string.Format(hashingFormat, model.Password, model.RePassword, model.Time, model.PwdType, model.QuestionId);

            return Utility.Md5HashingData(rawDataToHash);
        }

        private ApiRegisterModel ReFormatRegisterModel(ApiRegisterModel model)
        {
            if (model != null)
            {
                //model.UserName = (!string.IsNullOrEmpty(model.UserName.Trim())) ? model.UserName.Trim() : string.Empty;
                //model.Email = (!string.IsNullOrEmpty(model.Email.Trim())) ? model.Email.Trim() : string.Empty;
                //model.Email = (!string.IsNullOrEmpty(model.Email.Trim())) ? model.Email.Trim() : string.Empty;
            }

            return model;
        }

        private string GenerateTokenKey(string userName)
        {
            var tokenKey = string.Empty;
            var clientIp = HttpContext.Current.Request.UserHostAddress;
            var dtNow = DateTime.Now.ToString(Constant.DATEFORMAT_yyyyMMddHHmmss);

            //Rule: MD5(< username >|< DateTime.Now.ToString(yyyyMMddHHmmsss) >|< clientIP >
            var rawTokenStr = string.Format("{0}|{1}|{2}", userName, dtNow, clientIp);

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
                    var dt = DateTime.ParseExact(time, Constant.DATEFORMAT_yyyyMMddHHmmss, null);
                    if (dt.AddHours(SingleSignOnSettings.ApiLoginTimeValidInHours) < DateTime.Now)
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

        private int ValidateBirthday(string birthday, ref string message)
        {
            var errorCode = EnumCommonCode.Success;
            try
            {
                var dt = DateTime.ParseExact(birthday, Constant.DATE_FORMAT_ddMMyyyy, null);
            }
            catch
            {
                errorCode = EnumCommonCode.Error;
                message = UserApiResource.REGISTER_ERROR_BIRTHDAY_INVALID;
            }
            return errorCode;
        }

        private string GetRegisterResponseMessage(int statusCode)
        {
            var message = string.Empty;

            global::System.Resources.ResourceManager ResManager =
            new global::System.Resources.ResourceManager(
          "SingleSignOn.Resources.UserApiResource", typeof(UserApiResource).Assembly);
            if (statusCode == EnumCommonCode.Success)
            {
                message = UserApiResource.REGISTER_SUCCESS;
            }
            else
            {
                EnumCode enumCode = (EnumCode)statusCode;
                message = ResManager.GetString(EnumCommon.GetDescription(enumCode));
            }

            return message;
        }

        private string GetChangePwdResponseMessage(int statusCode, int level = 1)
        {
            var message = string.Empty;
            if (statusCode == EnumCommonCode.Success)
            {
                if (level == UserPasswordLevel.Level2)
                {
                    message = UserApiResource.COMMON_SUCCESS_OTPCODE_SENT;
                }
                else
                {
                    message = UserApiResource.COMMON_CHANGEPWD_SUCCESS;
                }
            }
            else if (statusCode == EnumCommonCode.Error_OldPwd1NotCorrect)
            {
                message = UserApiResource.COMMON_ERROR_OLDPWD1_NOTCORRECT;
            }
            else if (statusCode == EnumCommonCode.Error_OldPwd2NotCorrect)
            {
                message = UserApiResource.COMMON_ERROR_OLDPWD2_NOTCORRECT;
            }
            else if (statusCode == EnumCommonCode.Error_NewPwdEqualOldPwd)
            {
                message = UserApiResource.COMMON_ERROR_OLDPWD_EQUAL_NEWPWD;
            }
            else if (statusCode == EnumCommonCode.Error_PwdTypeInvalid)
            {
                message = UserApiResource.COMMON_ERROR_PWDTYPE_NULL;
            }
            else if (statusCode == EnumCommonCode.Error_UserOrTokenNotFound)
            {
                message = UserApiResource.COMMON_ERROR_USERTOKEN_NOTFOUND;
            }

            return message;
        }

        private string GetUpdateProfleResponseMessage(int statusCode)
        {
            var message = string.Empty;
            global::System.Resources.ResourceManager ResManager =
            new global::System.Resources.ResourceManager(
          "SingleSignOn.Resources.UserApiResource", typeof(UserApiResource).Assembly);
            if (statusCode == EnumCommonCode.Success)
            {
                message = UserApiResource.UPDATE_PROFILE_SUCCESS;
            }
            else
            {
                EnumCode enumCode = (EnumCode)statusCode;
                message = ResManager.GetString(EnumCommon.GetDescription(enumCode));
            }

            return message;
        }

        private void ClearLoginDataCachedById(IdentityUser currentUser, string domain)
        {
            try
            {
                _redisService.ClearCachedDataByKeyAndDomain(currentUser.UserName + currentUser.PasswordHash, domain);
                _redisService.ClearCachedDataByKeyAndDomain(currentUser.Email + currentUser.PasswordHash, domain);
                _redisService.ClearCachedDataByKeyAndDomain(currentUser.PhoneNumber + currentUser.PasswordHash, domain);
            }
            catch (Exception ex)
            {
                var strError = "Could not GetUserById to CleareUserCache due to: {0}";
                strError = string.Format(ex.Message);
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

        private string GetOTPResponseMessage(int statusCode, string type = "create")
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
                    message = UserApiResource.OTP_SUCCESS_VERIFY;
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

            return message;
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
                var strError = string.Format("Failed to Send SMS to user [0] due to {1}", mobile, ex.Message);
                logger.Error(strError);
            }

            return msg;
        }

        private int SendEmailToRecoverPwd(ApiRecoverPasswordModel model, string receiver)
        {
            var result = EnumCommonCode.Success;
            try
            {
                var hasTimeout = false;
                var emailModel = new EmailInputModel();

                emailModel.body = EmailHelper.GetEmailTemplate("RecoverPassword");

                var recoverData = string.Format("{0}|{1}", model.UserId, model.PwdType);
                //var dataEncrypt = Utility.TripleDESEncrypt(SystemSettings.EncryptKey, recoverData);
                var dataEncrypt = Utility.EncryptText(recoverData, SystemSettings.EncryptKey);

                var rawData = string.Format("{0}.{1}.{2}", Utility.Md5HashingData(DateTime.Now.ToString(Constant.DATEFORMAT_yyyyMMddHHmmss)), dataEncrypt, Utility.Md5HashingData(model.UserId.ToString()));

                var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
                var recoverLink = string.Format("{0}/{1}/{2}?token={3}", SystemSettings.FrontendURL, "WebAuth", "Password_Reset", rawData);
                if (!string.IsNullOrEmpty(emailModel.body))
                {
                    emailModel.body = emailModel.body.Replace(EmailTemplateConst.Receiver, receiver);
                    emailModel.body = emailModel.body.Replace(EmailTemplateConst.RecoverPwdLink, recoverLink);
                }
                emailModel.sendto = receiver;
                emailModel.subject = EmailResource.SUBJECT_RECOVER_PASSWORD;


                var emailStore = GlobalContainer.IocContainer.Resolve<IStoreSystemEmail>();
                var emailInfo = new IdentitySystemEmail();
                emailInfo.Subject = emailModel.subject;
                emailInfo.Body = emailModel.body;
                emailInfo.Sender = string.Empty;
                emailInfo.Receiver = receiver;
                emailInfo.ReceiverId = model.UserId;
                emailInfo.Action = (model.PwdType == PasswordLevelType.Level1) ? ActionType.RecoverPassword1 : ActionType.RecoverPassword2;

                //Create new email
                emailStore.Insert(emailInfo);

                //Send email
                EmailService.Send(emailModel, ref hasTimeout);
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to Send Email to user [0] due to {1}", receiver, ex.Message);
                logger.Error(strError);
                result = EnumCommonCode.Error;
            }

            return result;
        }

        public static bool IsPhoneNumber(string input)
        {
            var regexPatern = "^([\\d() +-]+){10,}$";
            if (!string.IsNullOrEmpty(input))
            {
                return Regex.IsMatch(input, regexPatern);
            }

            return false;
        }

        public static bool IsEmail(string input)
        {
            var regexPatern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" + "@" + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";
            if (!string.IsNullOrEmpty(input))
            {
                return Regex.IsMatch(input, regexPatern);
            }

            return false;
        }


        #endregion

        //[Route("sendemail")]
        //[HttpGet]
        //public IHttpActionResult SendEmail(string email)
        //{
        //    var result = EnumCommonCode.Success;
        //    try
        //    {
        //        var hasTimeout = false;
        //        var emailModel = new EmailInputModel();
        //        emailModel.body = EmailHelper.GetEmailTemplate("ActiveAccount");

        //        var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
        //        var activeLink = string.Format("{0}/{1}/{2}?token={3}", SystemSettings.FrontendURL, "WebAuth", "ActiveAccount", "xxx");
        //        if (!string.IsNullOrEmpty(emailModel.body))
        //        {
        //            emailModel.body = emailModel.body.Replace(EmailTemplateConst.Receiver, email);
        //            emailModel.body = emailModel.body.Replace(EmailTemplateConst.ActiveAccountLink, activeLink);
        //        }
        //        emailModel.sendto = email;
        //        emailModel.subject = EmailResource.SUBJECT_VERIVY_REGISTER;

        //        EmailService.Send(emailModel, ref hasTimeout);
        //    }
        //    catch (Exception ex)
        //    {
        //        var strError = string.Format("Failed to Send Email to user [0] due to {1}", email, ex.Message);
        //        logger.Error(strError);
        //        result = EnumCommonCode.Error;
        //    }

        //    return Json("ok");
        //}

    }
}
