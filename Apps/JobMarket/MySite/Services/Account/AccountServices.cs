using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using MySite.Logging;
using MySite.Settings;
using MySite.Resources;
using MySite.Models;
using MySite.Caching;
using MySite.ShareLibs.Exceptions;
using System.Threading.Tasks;
using MySite.ShareLibs;
using MySite.Models.Account;
using System.Collections.Generic;

namespace MySite.Services
{
    public class AccountServices
    {
        private static readonly ILog logger = LogProvider.For<AccountServices>();

        #region SingleSignOn core

        public static async Task<ResponseWebAuthLoginModel> LoginAsync(ApiAuthLoginModel model)
        {
            var strError = string.Empty;
            var result = new ResponseWebAuthLoginModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteSingleSignOnApi, "api/user/login");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientCore();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();

                    // Parsing the returned result                    
                    var responseString = await response.Content.ReadAsStringAsync();

                    result = JsonConvert.DeserializeObject<ResponseWebAuthLoginModel>(responseString);
                }

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to LOGIN - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        internal static object GetNotificationAsync(ApiGetListByPageModel apiFilterModel)
        {
            throw new NotImplementedException();
        }

        public static async Task<ResponseWebAuthLoginModel> LoginWithAsync(ApiAuthLoginWithModel model)
        {
            var strError = string.Empty;
            var result = new ResponseWebAuthLoginModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteSingleSignOnApi, "api/user/loginwith");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientCore();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();

                    // Parsing the returned result                    
                    var responseString = await response.Content.ReadAsStringAsync();

                    result = JsonConvert.DeserializeObject<ResponseWebAuthLoginModel>(responseString);
                }

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to LOGIN - WITH - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        public static async Task<ResponseApiModel> Logout(LogoutModel model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteSingleSignOnApi, "api/user/logout");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientCore();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();

                    // Parsing the returned result                    
                    var responseString = await response.Content.ReadAsStringAsync();

                    result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);
                }

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to LOGOUT - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        public static async Task<ResponseApiModel> RegisterAsync(ApiRegisterModel model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteSingleSignOnApi, "api/user/register");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientCore();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = await client.PostAsync(_baseUrl, theContent);

                // Parsing the returned result                    
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to REGISTER - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        //Verify by OTP
        public static async Task<ResponseApiModel> VerifyOTPAsync(VerifyOTPModel model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteSingleSignOnApi, "api/user/verifyotp");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientCore();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = await client.PostAsync(_baseUrl, theContent);

                // Parsing the returned result                    
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to VERIFY OTP - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        //Verify by email
        public static async Task<ResponseApiModel> VerifyEmailAsync(ApiActiveAccountModel model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteSingleSignOnApi, "api/user/activeaccount");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientCore();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = await client.PostAsync(_baseUrl, theContent);

                // Parsing the returned result                    
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to ACTIVE ACCOUNT - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        public static async Task<ResponseApiModel> GetUserProfileAsync(ApiUserModel model, int userId = 0)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteSingleSignOnApi, "api/user/profile");
            if (userId != 0)
            {
                model.UserId = userId;
            }
            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientCore();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;

                // Parsing the returned result                    
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);

            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to GET PROFILE - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        public static async Task<ResponseApiModel> GetListUserProfileAsync(ApiListUserInfoModel model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteSingleSignOnApi, "api/user/getlistprofile");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientCore();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;

                // Parsing the returned result                    
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);

            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to GetListUserProfileAsync - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        public static async Task<ResponseApiModel> ChangePasswordAsync(ApiChangePasswordModel model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteSingleSignOnApi, "api/user/changepwd");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientCore();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();

                    // Parsing the returned result                    
                    var responseString = await response.Content.ReadAsStringAsync();

                    result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);
                }

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to ChangePassword1 - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        public static async Task<ResponseApiModel> GetUserByEmailAsync(ApiGetUserByEmailModel model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteSingleSignOnApi, "api/user/getuserbyemail");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientCore();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();

                    // Parsing the returned result                    
                    var responseString = await response.Content.ReadAsStringAsync();

                    result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);
                }

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api GetUserByEmailAsync - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        public static async Task<ResponseApiModel> GetUserByPhoneNumberAsync(ApiGetUserByPhoneNumberModel model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteSingleSignOnApi, "api/user/getuserbyphonenumber");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientCore();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();

                    // Parsing the returned result                    
                    var responseString = await response.Content.ReadAsStringAsync();

                    result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);
                }

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api GetUserByPhoneNumberAsync - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        public static async Task<ResponseApiModel> RecoverPasswordStep1Async(ApiChangePassword1Model model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteSingleSignOnApi, "api/user/recoverpasswordstep1");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientCore();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();

                    // Parsing the returned result                    
                    var responseString = await response.Content.ReadAsStringAsync();

                    result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);
                }

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to change password 1 - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        public static async Task<ResponseApiModel> RecoverPasswordStep2Async(ApiChangePassword2Model model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteSingleSignOnApi, "api/user/recoverpasswordstep2");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientCore();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();

                    // Parsing the returned result                    
                    var responseString = await response.Content.ReadAsStringAsync();

                    result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);
                }

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to change password 2 - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        public static async Task<ResponseApiModel> SendEmailAtiveAccount(ApiSendEmailActiveAccountModel model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteSingleSignOnApi, "api/user/resend_email_active");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientCore();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();

                    // Parsing the returned result                    
                    var responseString = await response.Content.ReadAsStringAsync();

                    result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);
                }

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api SendEmailAtiveAccount - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        public static async Task<ResponseApiModel> SendEmailRecoverPassword(ApiSendEmailRecoverPasswordModel model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteSingleSignOnApi, "api/user/send_email_recover");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientCore();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();

                    // Parsing the returned result                    
                    var responseString = await response.Content.ReadAsStringAsync();

                    result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);
                }

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api SendEmailRecoverPassword - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        public static async Task<ResponseApiModel> RecoverPasswordOTPAsync(ApiRecoverPasswordOTPModel model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteSingleSignOnApi, "api/user/recoverpasswordotp");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientCore();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();

                    // Parsing the returned result                    
                    var responseString = await response.Content.ReadAsStringAsync();

                    result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);
                }

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api RecoverPasswordOTPAsync - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        public static async Task<ResponseApiModel> UpdateProfileAsync(ApiUpdateProfileModel model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteSingleSignOnApi, "api/user/updateprofile");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientCore();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();

                    // Parsing the returned result                    
                    var responseString = await response.Content.ReadAsStringAsync();

                    result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);
                }

                //Trace log
                HttpStatusCodeTrace(response);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to UpdateProfile - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        public static async Task<ResponseApiModel> UploadAvatarTempAsync(ApiUploadAvatarModel model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteSingleSignOnApi, "api/file/upload");

            try
            {
                using (var client = new HttpClient())
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        byte[] Bytes = new byte[model.Files[0].InputStream.Length + 1];
                        model.Files[0].InputStream.Read(Bytes, 0, Bytes.Length);
                        var fileContent = new ByteArrayContent(Bytes);
                        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = model.Files[0].FileName };
                        content.Add(fileContent);

                        content.Add(new StringContent(model.UserId.ToString()), "UserId");
                        //Extends parameters
                        //Dictionary<string, string> parameters = new Dictionary<string, string>();
                        //parameters.Add("UserId", model.UserId.ToString());

                        //HttpContent DictionaryItems = new FormUrlEncodedContent(parameters);
                       // content.Add(DictionaryItems);

                        var response = client.PostAsync(_baseUrl, content).Result;

                        //Trace log
                        HttpStatusCodeTrace(response);

                        // Parsing the returned result                    
                        var responseString = await response.Content.ReadAsStringAsync();

                        result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);
                    }
                }
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to UploadAvatarTempAsync - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        public static async Task<ResponseApiModel> CropAvatarTempAsync(ApiCropAvatarModel model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteSingleSignOnApi, "api/file/cropavatar");

            try
            {
                using (var client = new HttpClient())
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        content.Add(new StringContent(model.userId), "userId");                      
                        content.Add(new StringContent(model.fileName), "fileName");                      
                        content.Add(new StringContent(model.t), "t");                      
                        content.Add(new StringContent(model.l), "l");                      
                        content.Add(new StringContent(model.w), "w");                      
                        content.Add(new StringContent(model.h), "h");                      

                        var response = client.PostAsync(_baseUrl, content).Result;

                        //Trace log
                        HttpStatusCodeTrace(response);

                        // Parsing the returned result                    
                        var responseString = await response.Content.ReadAsStringAsync();

                        result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);
                    }
                }
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to CropAvatarTempAsync - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        #endregion

        #region Social

        public static async Task<ResponseApiModel> GetCounterAsync(ApiUserModel model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteApi, "api/user/getcounter");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientSocial();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;

                // Parsing the returned result                    
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);

            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to GET COUNTER - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        public static async Task<ResponseApiModel> GetFeedCounterAsync(ApiUserModel model)
        {
            var strError = string.Empty;
            var result = new ResponseApiModel();
            var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteApi, "api/user/getfeedcounter");

            var rawData = JsonConvert.SerializeObject(model);
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Authorization
                client.AuthorizationHttpClientSocial();

                //Begin calling
                var response = new HttpResponseMessage();

                StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

                // Post to the Server and parse the response.
                response = client.PostAsync(_baseUrl, theContent).Result;

                // Parsing the returned result                    
                var responseString = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);

                //Trace log
                HttpStatusCodeTrace(response);

            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when calling api to GetFeedCounterAsync - {0} because: {1}", _baseUrl, ex.ToString());
                logger.Error(strError);

                throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
            }

            return result;
        }

        //public static async Task<ResponseApiModel> ViewNotificatioAsync(IdentityNotification model)
        //{
        //    var strError = string.Empty;
        //    var result = new ResponseApiModel();
        //    var _baseUrl = string.Format("{0}/{1}", SystemSettings.MySiteApi, "api/notif/view");

        //    var rawData = JsonConvert.SerializeObject(model);
        //    try
        //    {
        //        var client = new HttpClient();
        //        client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

        //        // We want the response to be JSON.
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Add("Accept-Language", UserCookieManager.GetCurrentLanguageOrDefault());
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //        //Authorization
        //        client.AuthorizationHttpClientSocial();

        //        //Begin calling
        //        var response = new HttpResponseMessage();

        //        StringContent theContent = new StringContent(rawData, System.Text.Encoding.UTF8, "application/json");

        //        // Post to the Server and parse the response.
        //        response = client.PostAsync(_baseUrl, theContent).Result;

        //        // Parsing the returned result                    
        //        var responseString = await response.Content.ReadAsStringAsync();
        //        result = JsonConvert.DeserializeObject<ResponseApiModel>(responseString);

        //        //Trace log
        //        HttpStatusCodeTrace(response);

        //    }
        //    catch (Exception ex)
        //    {
        //        strError = string.Format("Failed when calling api to ViewNotificatioAsync - {0} because: {1}", _baseUrl, ex.ToString());
        //        logger.Error(strError);

        //        throw new CustomSystemException(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT);
        //    }

        //    return result;
        //}

        #endregion               

        private static void HttpStatusCodeTrace(HttpResponseMessage response)
        {
            var statusCode = Utils.ConvertToInt32(response.StatusCode);
            if (statusCode != (int)HttpStatusCode.OK)
            {
                logger.Error(string.Format("Return code: {0}, message: {1}", response.StatusCode, response.RequestMessage.RequestUri));
            }
        }
    }
}