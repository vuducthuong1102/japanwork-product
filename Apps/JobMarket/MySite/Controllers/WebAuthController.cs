using System;
using System.Web.Mvc;
using MySite.Logging;
using MySite.Models;
using System.Threading.Tasks;
using MySite.Services;
using Newtonsoft.Json;
using MySite.Helpers;
using MySite.Settings;
using MySite.Extensions;
using MySite.Resources;
using MySite.Caching;
using System.Linq;
using Autofac;
using MySite.Caching.Providers;
using System.Text.RegularExpressions;
using MySite.ShareLibs;
using MySite.App_Start;
using System.Web;
using Microsoft.Owin.Security;
using MySite.Models.Account;
using reCAPTCHA.MVC;
using System.Web.Security;
using System.Collections.Generic;
using MySite.Attributes;
using SingleSignOn.DB.Sql.Entities;
using System.Net;
using CaptchaMvc.HtmlHelpers;
using MySite.ShareLibs.Extensions;

namespace MySite.Controllers
{
    public class WebAuthController : BaseController
    {
        private readonly ILog logger = LogProvider.For<WebAuthController>();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public WebAuthController()
        {
            //Constructor
        }

        public WebAuthController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [PreventCrossOrigin]
        public ActionResult ExternalLogin(string provider)
        {
            //ControllerContext.HttpContext.Session.RemoveAll();
            var URLReturn = string.Empty;
            if (Request["URLReturn"] != null)
            {
                URLReturn = Request["URLReturn"].ToString();
            }

            Session["Workaround"] = 0;

            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "WebAuth", new { URLReturn = URLReturn }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string URLReturn)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }
            var id = loginInfo.ExternalIdentity.Claims.FirstOrDefault().Value;

            // Sign in the user with this external login provider if the user already has a login
            var email = loginInfo.Email;
            var SocialProvider = loginInfo.Login.LoginProvider;
            var SocialToken = loginInfo.Login.ProviderKey;


            var apiModel = new ApiAuthLoginWithModel
            {
                UserName = id,
                Email = loginInfo.Email,
                DisplayName = loginInfo.ExternalIdentity.Name,
                SocialProvider = SocialProvider,
                AppCode = SystemSettings.AppCode
            };

            //if (SocialProvider == "Google")
            //{
            //https://picasaweb.google.com/data/entry/api/user/116162548440046835045?alt=json
            //}
            //else if (SocialProvider == "Facebook")
            //{
            //    apiModel.Avatar = "https://graph.facebook.com/" + apiModel.UserName + "/picture?type=normal";
            //}

            var returnModel = new ResponseWebAuthLoginModel();
            try
            {
                logger.Debug("Begin LOGIN-WEB request");

                logger.DebugFormat("LOGIN-WEB Returned Model encrypted = {0}", JsonConvert.SerializeObject(returnModel));

                //Call api to login
                returnModel = await AccountServices.LoginWithAsync(apiModel);

                if (returnModel.Code == EnumCommonCode.Success)
                {
                    Session["LoginTimesFailed"] = 0;

                    //Save user data to session
                    var cookieModel = new UserCookieModel { UserId = returnModel.Data.Id, TokenKey = returnModel.Data.TokenKey, OTPType = returnModel.Data.OTPType };
                    UserCookieManager.SetCookie(MySiteSettings.SSOCommonUserKey, cookieModel, returnModel.Data.LoginDurations);

                    //Save to cache
                    var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                    cacheProvider.Set(string.Format("{0}_{1}", "USER", cookieModel.UserId), returnModel.Data, returnModel.Data.LoginDurations);

                    if (!string.IsNullOrEmpty(URLReturn))
                    {
                        return Redirect(URLReturn);
                    }
                    else
                    {
                        return Redirect("~");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(returnModel.Msg))
                    {
                        this.AddNotification(returnModel.Msg, NotificationType.ERROR);
                    }
                    else
                    {
                        this.AddNotification(UserWebResource.COMMON_EXCEPTION_NOTIF, NotificationType.ERROR);
                    }
                }

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                var rawModel = JsonConvert.SerializeObject(email);
                var strError = string.Format("Failed for LOGIN-WEB request: {0} because: {1}", rawModel, ex.Message);
                logger.Error(strError);

                this.AddNotification(UserWebResource.COMMON_EXCEPTION_NOTIF, NotificationType.ERROR);
                return RedirectToAction("Login");
            }
            finally
            {
                logger.Debug("Ended LOGIN-WEB request");
            }
        }

        /// <summary>
        /// GET: /WebAuth/Login
        /// </summary>
        [AllowAnonymous]
        public ActionResult Login(string ReturnUrl)
        {
            if (Request["ReturnUrl"] != null)
            {
                ReturnUrl = Request["ReturnUrl"].ToString();
            }

            //if (string.IsNullOrEmpty(ReturnUrl))
            //{
            //    if (Request.UrlReferrer != null)
            //    {
            //        ReturnUrl = String.Format("{0}{1}{2}{3}", Request.UrlReferrer.Scheme,
            //            Uri.SchemeDelimiter, Request.UrlReferrer.Authority, Request.UrlReferrer.AbsolutePath);
            //    }
            //}

            ViewBag.ReturnUrl = ReturnUrl;

            if (User.Identity.IsAuthenticated)
            {
                ResponseWebAuthLoginModel redirectModel = new ResponseWebAuthLoginModel();
                redirectModel.ReturnUrl = ReturnUrl;
                try
                {
                    var userInfo = AccountHelper.GetCurrentUser();
                    if (userInfo != null)
                    {
                        redirectModel.Data.Id = userInfo.Id;
                        redirectModel.TokenKey = userInfo.TokenKey;

                        return RedirectToLocal(redirectModel);
                    }
                }
                catch (Exception ex)
                {
                    var strError = string.Format("Failed when trying to Login due to: {0}", ex.ToString());
                    logger.Error(strError);
                    this.AddNotification(UserWebResource.COMMON_EXCEPTION_NOTIF, NotificationType.ERROR);
                }
            }

            WebAuthLoginViewModel model = new WebAuthLoginViewModel();
            model.ReturnUrl = ReturnUrl;
            model.NumberOfFailedLogins = Convert.ToInt32(Session["LoginTimesFailed"]);
            return View(model);
        }


        /// <summary>
        /// POST: /WebAuth/Login
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [PreventCrossOrigin]
        public async Task<ActionResult> Login(WebAuthLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //model.NumberOfFailedLogins = Convert.ToInt32(Session["LoginTimesFailed"]);
            //if (model.NumberOfFailedLogins >= MySiteSettings.NumberOfFailedLoginsToShowCaptcha)
            //{
            //    var captchaMessage = UserWebResource.COMMON_ERROR_CAPTCHA_INVALID;
            //    if (!this.IsCaptchaValid(captchaMessage))
            //    {
            //        this.AddNotification(captchaMessage, NotificationType.ERROR);
            //        return View(model);
            //    }
            //}

            var apiModel = ReFormatLoginModel(model);

            var currentDomain = Request.Url.Host;
            var returnModel = new ResponseWebAuthLoginModel();
            try
            {
                logger.Debug("Begin LOGIN-WEB request");

                logger.DebugFormat("LOGIN-WEB Returned Model encrypted = {0}", JsonConvert.SerializeObject(returnModel));

                //Call api to login
                returnModel = await AccountServices.LoginAsync(apiModel);

                if (returnModel.Code == (int)HttpStatusCode.OK)
                {
                    returnModel.ReturnUrl = model.ReturnUrl;
                    Session["LoginTimesFailed"] = 0;

                    //Save user data to session
                    //var cookieModel = new UserCookieModel { UserId = returnModel.Data.Id, TokenKey = returnModel.Data.TokenKey, OTPType = returnModel.Data.OTPType };
                    //UserCookieManager.SetCookie(MySiteSettings.SSOCommonUserKey, cookieModel, returnModel.Data.LoginDurations);

                    ////Save to cache
                    //var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                    //cacheProvider.Set(string.Format("{0}_{1}", "USER", cookieModel.UserId), returnModel.Data, returnModel.Data.LoginDurations);

                    //ClearOutputCache();

                    if (returnModel.Data != null)
                    {
                        var jobSeekerReturn = JobSeekerServices.GetDetailForUpdateAsync(new ApiJobSeekerGetDetailModel { id = returnModel.Data.Id }).Result;
                        if (jobSeekerReturn != null && jobSeekerReturn.value != null)
                        {
                            ApiJobMarket.DB.Sql.Entities.IdentityJobSeeker jobSeekerInfo = JsonConvert.DeserializeObject<ApiJobMarket.DB.Sql.Entities.IdentityJobSeeker>(jobSeekerReturn.value.ToString());
                            if (jobSeekerInfo != null)
                            {
                                returnModel.Data.DisplayName = (!string.IsNullOrEmpty(jobSeekerInfo.display_name) ? jobSeekerInfo.display_name : returnModel.Data.DisplayName);
                                returnModel.Data.FullName = (!string.IsNullOrEmpty(jobSeekerInfo.fullname) ? jobSeekerInfo.fullname : returnModel.Data.FullName);

                                if (jobSeekerInfo.Extensions != null && !string.IsNullOrEmpty(jobSeekerInfo.image))
                                {
                                    if (MyObjectExtensions.PropertyExists(jobSeekerInfo.Extensions, "image_full"))
                                    {
                                        returnModel.Data.Avatar = jobSeekerInfo.Extensions.image_full;
                                    }
                                }

                                returnModel.Data.Email = (!string.IsNullOrEmpty(jobSeekerInfo.email) ? jobSeekerInfo.email : returnModel.Data.Email);
                            }
                        }
                    }

                    var userDataJson = JsonConvert.SerializeObject(returnModel.Data);

                    FormsAuthentication.SetAuthCookie(userDataJson, model.RememberMe);

                    if (!string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        if (!string.IsNullOrEmpty(model.ReturnUrl) && !model.ReturnUrl.Contains("WebAuth/Register")
                        && !model.ReturnUrl.Contains("WebAuth/Login")
                        && !model.ReturnUrl.Contains("xac-thuc-tai-khoan")
                        && !model.ReturnUrl.Contains("quen-mat-khau")
                        )
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("index", "home");
                            //return Redirect("~");
                        }
                    }
                    else
                    {
                        if (Request.UrlReferrer != null)
                        {
                            if (Request.UrlReferrer.Host == Request.Url.Host)
                                return Redirect(Request.UrlReferrer.ToString());
                            else
                                return Redirect("~");
                        }
                        else
                        {
                            return Redirect("~");
                        }
                    }
                }
                else
                {
                    model.NumberOfFailedLogins++;
                    Session["LoginTimesFailed"] = model.NumberOfFailedLogins;

                    if (!string.IsNullOrEmpty(returnModel.Msg))
                    {
                        this.AddNotification(returnModel.Msg, NotificationType.ERROR);
                    }
                    else
                    {
                        this.AddNotification(UserWebResource.COMMON_EXCEPTION_NOTIF, NotificationType.ERROR);
                    }

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                var rawModel = JsonConvert.SerializeObject(model);
                var strError = string.Format("Failed for LOGIN-WEB request: {0} because: {1}", rawModel, ex.Message);
                logger.Error(strError);

                this.AddNotification(UserWebResource.COMMON_EXCEPTION_NOTIF, NotificationType.ERROR);

                return View(model);
            }
            finally
            {
                logger.Debug("Ended LOGIN-WEB request");
            }
        }

        /// <summary>
        /// POST: /WebAuth/Login
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [PreventCrossOrigin]
        public async Task<ActionResult> QuickLogin(WebAuthLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var messages = string.Empty;
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var value = ModelState[modelStateKey];
                    foreach (var error in value.Errors)
                    {
                        messages += modelStateKey.Replace("model.", "") + ": " + error.ErrorMessage + error.Exception + "; ";
                    }
                }

                return Json(new { success = false, message = messages }, JsonRequestBehavior.AllowGet);
            }

            var apiModel = ReFormatLoginModel(model);
            var message = string.Empty;
            var currentDomain = Request.Url.Host;
            var returnModel = new ResponseWebAuthLoginModel();
            try
            {
                logger.Debug("Begin QUICK-LOGIN-WEB request");

                logger.DebugFormat("QUICK-LOGIN-WEB Returned Model encrypted = {0}", JsonConvert.SerializeObject(returnModel));

                //Call api to login
                returnModel = await AccountServices.LoginAsync(apiModel);

                if (returnModel.Code == (int)HttpStatusCode.OK)
                {
                    returnModel.ReturnUrl = model.ReturnUrl;
                    Session["LoginTimesFailed"] = 0;

                    //Save user data to session
                    //var cookieModel = new UserCookieModel { UserId = returnModel.Data.Id, TokenKey = returnModel.Data.TokenKey, OTPType = returnModel.Data.OTPType };
                    //UserCookieManager.SetCookie(MySiteSettings.SSOCommonUserKey, cookieModel, returnModel.Data.LoginDurations);

                    if (returnModel.Data != null)
                    {
                        var jobSeekerReturn = JobSeekerServices.GetDetailForUpdateAsync(new ApiJobSeekerGetDetailModel { id = returnModel.Data.Id }).Result;
                        if (jobSeekerReturn != null && jobSeekerReturn.value != null)
                        {
                            ApiJobMarket.DB.Sql.Entities.IdentityJobSeeker jobSeekerInfo = JsonConvert.DeserializeObject<ApiJobMarket.DB.Sql.Entities.IdentityJobSeeker>(jobSeekerReturn.value.ToString());
                            if (jobSeekerInfo != null)
                            {
                                returnModel.Data.DisplayName = (!string.IsNullOrEmpty(jobSeekerInfo.display_name) ? jobSeekerInfo.display_name : returnModel.Data.DisplayName);
                                returnModel.Data.FullName = (!string.IsNullOrEmpty(jobSeekerInfo.fullname) ? jobSeekerInfo.fullname : returnModel.Data.FullName);

                                if (jobSeekerInfo.Extensions != null && !string.IsNullOrEmpty(jobSeekerInfo.image))
                                {
                                    if (MyObjectExtensions.PropertyExists(jobSeekerInfo.Extensions, "image_full"))
                                    {
                                        returnModel.Data.Avatar = jobSeekerInfo.Extensions.image_full;
                                    }
                                }

                                returnModel.Data.Email = (!string.IsNullOrEmpty(jobSeekerInfo.email) ? jobSeekerInfo.email : returnModel.Data.Email);
                            }
                        }
                    }

                    var userDataJson = JsonConvert.SerializeObject(returnModel.Data);

                    FormsAuthentication.SetAuthCookie(userDataJson, model.RememberMe);

                    //Save to cache
                    //var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();
                    //cacheProvider.Set(string.Format("{0}_{1}", "USER", cookieModel.UserId), returnModel.Data, returnModel.Data.LoginDurations);

                    ClearOutputCache();

                    return Json(new { success = true, message = UserWebResource.MS_LOGIN_SUCCESS }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (!string.IsNullOrEmpty(returnModel.Msg))
                    {
                        message = returnModel.Msg;
                    }
                    else
                    {
                        message = UserWebResource.COMMON_EXCEPTION_NOTIF;
                    }

                    if (returnModel.Code == (int)EnumCommonCode.Error_Account_Inactive)
                    {

                        return Json(new { success = false, message = message, state = "email_inactive" }, JsonRequestBehavior.AllowGet);
                    }

                    model.NumberOfFailedLogins++;
                    Session["LoginTimesFailed"] = model.NumberOfFailedLogins;

                    //return RedirectToErrorPage("Error");

                    return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var rawModel = JsonConvert.SerializeObject(model);
                var strError = string.Format("Failed for LOGIN-WEB request: {0} because: {1}", rawModel, ex.Message);
                logger.Error(strError);

                message = UserWebResource.COMMON_EXCEPTION_NOTIF;

                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                logger.Debug("Ended QUICK-LOGIN-WEB request");
            }
        }

        public ActionResult Logout()
        {
            try
            {
                //Clear cache first
                AccountHelper.ClearLoginDataCached();

                //Clear cookie
                UserCookieManager.ClearCookie(MySiteSettings.SSOCommonUserKey);

                if (Request.Cookies[".AspNet.ExternalCookie"] != null)
                {
                    HttpCookie myCookie = new HttpCookie(".AspNet.ExternalCookie");
                    myCookie.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(myCookie);
                }

                var authTypes = AuthenticationManager.GetAuthenticationTypes().ToList();
                var authTypeNames = new List<string>();

                foreach (var authType in authTypes)
                {
                    if (!authTypeNames.Contains(authType.AuthenticationType))
                        authTypeNames.Add(authType.AuthenticationType);
                }
                AuthenticationManager.SignOut(authTypeNames.ToArray());

                FormsAuthentication.SignOut();
                foreach (var cookie in Request.Cookies.AllKeys)
                {
                    Request.Cookies.Remove(cookie);
                }

                //// Clear authentication cookie
                //HttpCookie rFormsCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
                //rFormsCookie.Expires = DateTime.Now.AddYears(-1);
                //Response.Cookies.Add(rFormsCookie);

                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();

                ClearOutputCache(true);
            }
            catch (Exception ex)
            {
                string strError = "Failed for LOGOUT request: " + ex.Message;
                logger.ErrorException(strError, ex);
            }

            //return RedirectToAction("Login", "WebAuth");
            return Redirect("~");

            //if (Request.UrlReferrer != null)
            //{
            //    if (Request.UrlReferrer.Host == Request.Url.Host)
            //        return Redirect(Request.UrlReferrer.ToString());
            //    else
            //        return Redirect("~");
            //}
            //else
            //{
            //    return Redirect("~");
            //}
        }

        public ActionResult Register()
        {
            return View();
        }

        //[ActionName("Register")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[PreventCrossOrigin]
        [CaptchaValidator(ErrorMessage = nameof(UserWebResource.COMMON_ERROR_CAPTCHA_INVALID))]
        public async Task<ActionResult> Register_Old(WebAccountRegisterModel model, bool captchaValid)
        {
            var captchaMessage = UserWebResource.COMMON_ERROR_CAPTCHA_INVALID;
            if (!captchaValid)
            {
                ModelState.AddModelError("captcha", captchaMessage);
                this.AddNotification(captchaMessage, NotificationType.ERROR);
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage + x.Exception));
                this.AddNotification(messages, NotificationType.ERROR);
                return View(model);
            }

            var apiModel = ReFormatRegisterModel(model);
            var validateMsg = string.Empty;
            var currentDomain = Request.Url.Host;

            var returnModel = new ResponseApiModel();

            var errorCode = ValidateRegisterData(model, ref validateMsg);

            var isValid = (errorCode == EnumCommonCode.Success || errorCode == 0);
            try
            {
                logger.Debug("Begin REGISTER-WEB request");
                logger.DebugFormat("REGISTER-WEB Model encrypted = {0}", JsonConvert.SerializeObject(model));
                if (isValid)
                {
                    var isPhoneNumber = IsPhoneNumber(apiModel.UserName);
                    var isEmail = IsEmail(apiModel.UserName);

                    apiModel.IsPhoneNumber = isPhoneNumber;
                    apiModel.IsEmail = isEmail;
                    apiModel.Display_Name = apiModel.UserName;

                    returnModel = await AccountServices.RegisterAsync(apiModel);

                    if (returnModel != null)
                    {
                        var userId = Utils.ConvertToInt32(returnModel.Data);

                        if (returnModel.Code == EnumCommonCode.Success)
                        {
                            if (string.IsNullOrEmpty(returnModel.Msg))
                            {
                                //Thong bao thanh cong
                                this.AddNotification(UserWebResource.MS_REGISTER_SUCCESS, NotificationType.SUCCESS);
                            }
                            else
                            {
                                this.AddNotification(returnModel.Msg, NotificationType.SUCCESS);
                                //if (isPhoneNumber)
                                //{
                                //    var verifyOTPModel = new WebVerifyOTPModel();
                                //    verifyOTPModel.UserId = userId;
                                //    verifyOTPModel.ActionType = ActionType.ActiveAccount;

                                //    return RedirectToAction("VerifyOTP", "WebAuth", verifyOTPModel);
                                //}
                            }

                            ModelState.Clear();
                            return View(new WebAccountRegisterModel());
                        }
                        else
                        {
                            //Thong bao that bai
                            this.AddNotification(returnModel.Msg, NotificationType.ERROR);
                        }
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

        //[ActionName("Register")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[PreventCrossOrigin]        
        //public async Task<ActionResult> Register_1(WebAccountRegisterModel model)
        //{
        //    var captchaMessage = UserWebResource.COMMON_ERROR_CAPTCHA_INVALID;
        //    //if (!captchaValid)
        //    //{
        //    //    ModelState.AddModelError("captcha", captchaMessage);
        //    //    this.AddNotification(captchaMessage, NotificationType.ERROR);
        //    //    return View(model);
        //    //}

        //    if (!ModelState.IsValid)
        //    {
        //        string messages = string.Join("; ", ModelState.Values
        //                                .SelectMany(x => x.Errors)
        //                                .Select(x => x.ErrorMessage + x.Exception));
        //        this.AddNotification(messages, NotificationType.ERROR);
        //        return View(model);
        //    }

        //    var apiModel = ReFormatRegisterModel(model);
        //    var validateMsg = string.Empty;
        //    var currentDomain = Request.Url.Host;

        //    var returnModel = new ResponseApiModel();

        //    var errorCode = ValidateRegisterData(model, ref validateMsg);

        //    var isValid = (errorCode == EnumCommonCode.Success || errorCode == 0);
        //    try
        //    {
        //        logger.Debug("Begin REGISTER-WEB request");
        //        logger.DebugFormat("REGISTER-WEB Model encrypted = {0}", JsonConvert.SerializeObject(model));
        //        if (isValid)
        //        {
        //            if (!this.IsCaptchaValid(""))
        //            {
        //                ModelState.AddModelError("CaptchaInputText", captchaMessage);
        //                this.AddNotification(captchaMessage, NotificationType.ERROR);
        //                return View(model);

        //                //this.AddNotification(UserWebResource.COMMON_ERROR_CAPTCHA_INVALID, NotificationType.ERROR);

        //                //return View(model);
        //            }

        //            var isPhoneNumber = IsPhoneNumber(apiModel.UserName);
        //            var isEmail = IsEmail(apiModel.UserName);

        //            apiModel.IsPhoneNumber = isPhoneNumber;
        //            apiModel.IsEmail = isEmail;
        //            apiModel.Display_Name = apiModel.UserName;

        //            returnModel = await AccountServices.RegisterAsync(apiModel);

        //            if (returnModel != null)
        //            {
        //                var userId = Utils.ConvertToInt32(returnModel.Data);

        //                if (returnModel.Code == (int)HttpStatusCode.OK)
        //                {
        //                    if (string.IsNullOrEmpty(returnModel.Msg))
        //                    {
        //                        //Thong bao thanh cong
        //                        this.AddNotification(UserWebResource.MS_REGISTER_SUCCESS, NotificationType.SUCCESS);
        //                    }
        //                    else
        //                    {
        //                        this.AddNotification(returnModel.Msg, NotificationType.SUCCESS);
        //                        //if (isPhoneNumber)
        //                        //{
        //                        //    var verifyOTPModel = new WebVerifyOTPModel();
        //                        //    verifyOTPModel.UserId = userId;
        //                        //    verifyOTPModel.ActionType = ActionType.ActiveAccount;

        //                        //    return RedirectToAction("VerifyOTP", "WebAuth", verifyOTPModel);
        //                        //}
        //                    }

        //                    ModelState.Clear();
        //                    return View(new WebAccountRegisterModel());
        //                }
        //                else
        //                {
        //                    //Thong bao that bai
        //                    this.AddNotification(returnModel.Msg, NotificationType.ERROR);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            this.AddNotification(validateMsg, NotificationType.ERROR);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string strError = "Failed for REGISTER-WEB request: " + ex.Message;
        //        logger.ErrorException(strError, ex);
        //        this.AddNotification(UserWebResource.COMMON_EXCEPTION_NOTIF, NotificationType.ERROR);
        //    }
        //    finally
        //    {
        //        logger.Debug("Ended REGISTER-WEB request");
        //    }

        //    return View(model);
        //}

        [ActionName("Register")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventCrossOrigin]
        public async Task<ActionResult> Register(WebAccountRegisterModel model)
        {
            var captchaMessage = UserWebResource.COMMON_ERROR_CAPTCHA_INVALID;
            //if (!captchaValid)
            //{
            //    ModelState.AddModelError("captcha", captchaMessage);
            //    this.AddNotification(captchaMessage, NotificationType.ERROR);
            //    return View(model);
            //}

            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage + x.Exception));
                this.AddNotification(messages, NotificationType.ERROR);
                return View(model);
            }

            var apiModel = ReFormatRegisterModel(model);
            var validateMsg = string.Empty;
            var currentDomain = Request.Url.Host;

            var returnModel = new ApiResponseCommonModel();

            var errorCode = ValidateRegisterData(model, ref validateMsg);

            var isValid = (errorCode == EnumCommonCode.Success || errorCode == 0);
            try
            {
                logger.Debug("Begin REGISTER-WEB request");
                logger.DebugFormat("REGISTER-WEB Model encrypted = {0}", JsonConvert.SerializeObject(model));
                if (isValid)
                {
                    if (!this.IsCaptchaValid(""))
                    {
                        ModelState.AddModelError("CaptchaInputText", captchaMessage);
                        this.AddNotification(captchaMessage, NotificationType.ERROR);
                        return View(model);

                        //this.AddNotification(UserWebResource.COMMON_ERROR_CAPTCHA_INVALID, NotificationType.ERROR);

                        //return View(model);
                    }

                    var isPhoneNumber = IsPhoneNumber(apiModel.UserName);
                    var isEmail = IsEmail(apiModel.UserName);

                    apiModel.IsPhoneNumber = isPhoneNumber;
                    apiModel.IsEmail = isEmail;
                    apiModel.Display_Name = apiModel.UserName;
                    apiModel.AppCode = SystemSettings.AppCode;

                    returnModel = await JobSeekerServices.RegisterAsync(apiModel);

                    await Task.FromResult(returnModel);
                    if (returnModel != null)
                    {
                        if (returnModel.status == (int)HttpStatusCode.OK || returnModel.status == EnumCommonCode.Success)
                        {
                            if (string.IsNullOrEmpty(returnModel.message))
                            {
                                //Thong bao thanh cong
                                this.AddNotification(UserWebResource.MS_REGISTER_SUCCESS, NotificationType.SUCCESS);
                            }
                            else
                            {
                                this.AddNotification(returnModel.message, NotificationType.SUCCESS);
                                //if (isPhoneNumber)
                                //{
                                //    var verifyOTPModel = new WebVerifyOTPModel();
                                //    verifyOTPModel.UserId = userId;
                                //    verifyOTPModel.ActionType = ActionType.ActiveAccount;

                                //    return RedirectToAction("VerifyOTP", "WebAuth", verifyOTPModel);
                                //}
                            }

                            ModelState.Clear();
                            return View(new WebAccountRegisterModel());
                        }
                        else
                        {
                            //Thong bao that bai
                            this.AddNotification(returnModel.message, NotificationType.ERROR);
                        }
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

        public ActionResult VerifyOTP(WebVerifyOTPModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("VerifyOTP")]
        [ValidateAntiForgeryToken]
        [PreventCrossOrigin]
        public async Task<ActionResult> ConfirmVerifyOTP(WebVerifyOTPModel model)
        {
            var returnModel = new ResponseApiModel();
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage + x.Exception));
                this.AddNotification(messages, NotificationType.ERROR);
                return View(model);
            }

            //var captchaMessage = UserWebResource.COMMON_ERROR_CAPTCHA_INVALID;
            //if (!this.IsCaptchaValid(captchaMessage))
            //{
            //    ModelState.AddModelError("", captchaMessage);
            //    return View(model);
            //}

            try
            {
                logger.Debug("Begin VERIFYOTP-WEB request");
                logger.DebugFormat("VERIFYOTP-WEB Model encrypted = {0}", JsonConvert.SerializeObject(model));
                var apiModel = new VerifyOTPModel();

                apiModel.Action = model.ActionType;
                apiModel.UserId = model.UserId;
                apiModel.OTPCode = model.OTPCode;
                apiModel.Time = DateTime.Now.ToString(Constant.DATEFORMAT_yyyyMMddHHmmss);

                returnModel = await AccountServices.VerifyOTPAsync(apiModel);

                if (returnModel != null)
                {
                    if (returnModel.Code == EnumCommonCode.Success)
                    {
                        this.AddNotification(returnModel.Msg, NotificationType.SUCCESS);
                        ModelState.Clear();

                        if (model.ActionType == ActionType.ActiveAccount)
                            return RedirectToAction("Login", "WebAuth", new { ReturnUrl = "/" });
                    }
                    else
                    {
                        //Thong bao that bai
                        this.AddNotification(returnModel.Msg, NotificationType.ERROR);
                    }
                }
            }
            catch (Exception ex)
            {
                string strError = "Failed for VERIFYOTP-WEB request: " + ex.Message;
                logger.ErrorException(strError, ex);
                this.AddNotification(UserWebResource.COMMON_EXCEPTION_NOTIF, NotificationType.ERROR);
            }
            finally
            {
                logger.Debug("Ended VERIFYOTP-WEB request");
            }

            return View(model);
        }


        [AllowAnonymous]
        public ActionResult RecoverPassword()
        {
            RequestSendEmailRecoverPasswordModel model = new RequestSendEmailRecoverPasswordModel();
            if (Request["email"] != null)
                model.Email = Request["email"].ToString();

            return View(model);
        }

        [HttpPost]
        [ActionName("RecoverPassword")]
        public async Task<ActionResult> RecoverPassword_Confirm(RequestSendEmailRecoverPasswordModel model)
        {
            var captchaMessage = UserWebResource.COMMON_ERROR_CAPTCHA_INVALID;
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
                if (!this.IsCaptchaValid(""))
                {
                    ModelState.AddModelError("CaptchaInputText", captchaMessage);
                    this.AddNotification(captchaMessage, NotificationType.ERROR);
                    return View(model);
                }

                if (!string.IsNullOrEmpty(model.Email))
                    model.Email = model.Email.Trim();

                var apiUserResult = await AccountServices.SendEmailRecoverPassword(new ApiSendEmailRecoverPasswordModel { Email = model.Email, Time = DateTime.Now.ToString(Constant.DATEFORMAT_yyyyMMddHHmmss) });
                if (apiUserResult != null)
                {
                    message = apiUserResult.Msg;
                    if (string.IsNullOrEmpty(message))
                        message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                    if (apiUserResult.Code == (int)HttpStatusCode.OK)
                    {
                        this.AddNotification(message, NotificationType.SUCCESS);
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        this.AddNotification(message, NotificationType.ERROR);
                    }
                }
                else
                {
                    this.AddNotification(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to SendEmailRecoverPassword because: {0}", ex.ToString()));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Password_Reset(string token)
        {
            var result = EnumCommonCode.Error_Info_NotFound;
            if (string.IsNullOrEmpty(token))
            {
                this.AddNotification(UserWebResource.COMMON_ERROR_DATA_INVALID, NotificationType.ERROR);
            }

            try
            {
                var dataArr = token.Split('.');
                if (dataArr.Count() < 3)
                {
                    this.AddNotification(UserWebResource.COMMON_ERROR_DATA_INVALID, NotificationType.ERROR);
                    return View();
                }

                //var rawData = Utility.TripleDESDecrypt(SystemSettings.EncryptKey, dataArr[1]);
                var rawData = Utility.DecryptText(dataArr[1], SystemSettings.EncryptKey);

                var userData = rawData.Split('|');
                if (userData.Count() >= 2)
                {
                    var apiModel = new ApiChangePassword2Model
                    {
                        UserId = Utils.ConvertToInt32(userData[0]),
                        PasswordType = userData[1]
                    };
                    var apiResponse = await AccountServices.RecoverPasswordStep2Async(apiModel);
                    if (apiResponse != null)
                    {
                        result = apiResponse.Code;
                    }

                    if (result != EnumCommonCode.Success)
                    {
                        if (result == EnumCommonCode.Error_Info_NotFound)
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
                        //var logInfo = GetDataForLogging(token);
                        //logInfo.ActionDesc = string.Format("User [{0}] was recovered password ({1}) successfully", userData[0], userData[1]);
                        //logInfo.Domain = Request.Url.Host;
                        //logInfo.ActionType = (userData[1] == "level1" ? ActionType.RecoverPassword1 : ActionType.RecoverPassword2);

                        ////Write log
                        //_userStore.WriteUserLog(logInfo, MySiteSettings.AllowToTraceLoginRequest);

                        ClearLoginDataCached();
                        UserCookieManager.ClearCookie(MySiteSettings.SSOCommonUserKey);

                        this.AddNotification(UserWebResource.COMMON_CHANGEPWD_SUCCESS, NotificationType.SUCCESS);
                    }
                }
                else
                {
                    this.AddNotification(UserWebResource.COMMON_ERROR_DATA_INVALID, NotificationType.ERROR);
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

        [HttpGet]
        public async Task<ActionResult> ActiveAccount(string token)
        {
            var result = EnumCommonCode.Success;
            var returnModel = new ResponseApiModel();
            if (string.IsNullOrEmpty(token))
            {
                this.AddNotification(UserWebResource.COMMON_ERROR_DATA_INVALID, NotificationType.ERROR);
            }

            try
            {
                var dataArr = token.Split('.');
                if (dataArr.Count() < 3)
                {
                    this.AddNotification(UserWebResource.COMMON_ERROR_DATA_INVALID, NotificationType.ERROR);
                    return View();
                }

                var rawData = Utility.DecryptText(dataArr[1], SystemSettings.EncryptKey);

                var userData = rawData.Split('|');
                if (userData.Count() >= 2)
                {
                    //result = _userStore.RecoverPasswordStep2(userData[0], userData[1]);
                    var apiModel = new ApiActiveAccountModel();

                    apiModel.ActiveMethod = ActionType.ActiveAccount;
                    apiModel.UserName = userData[0];
                    apiModel.HashingData = token;
                    apiModel.Time = DateTime.Now.ToString(Constant.DATEFORMAT_yyyyMMddHHmmss);

                    returnModel = await AccountServices.VerifyEmailAsync(apiModel);
                }

                if (returnModel.Code == EnumCommonCode.Success)
                {
                    var apiModel = new ApiJobSeekerWishModel();
                    apiModel.job_seeker_id = Utils.ConvertToInt32(returnModel.Data);
                    if (apiModel.job_seeker_id > 0)
                    {
                        await JobSeekerWishServices.UpdateAsync(apiModel);
                    }

                    this.AddNotification(UserWebResource.MS_REGISTER_SUCCESS, NotificationType.SUCCESS);
                    ModelState.Clear();
                }
                else
                {
                    //Thong bao that bai
                    this.AddNotification(returnModel.Msg, NotificationType.ERROR);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying to ActiveAccount due to: {0}", ex.ToString());
                logger.Error(strError);
                this.AddNotification(UserWebResource.COMMON_ERROR_DATA_INVALID, NotificationType.ERROR);
            }

            return RedirectToAction("Register", "WebAuth");
        }

        [AllowAnonymous]
        public ActionResult ResendEmailActive()
        {
            RequestSendEmailActiveModel model = new RequestSendEmailActiveModel();
            if (Request["email"] != null)
                model.Email = Request["email"].ToString();

            return View(model);
        }

        [HttpPost]
        [ActionName("ResendEmailActive")]
        public async Task<ActionResult> ResendEmailActive_Confirm(RequestSendEmailActiveModel model)
        {
            var captchaMessage = UserWebResource.COMMON_ERROR_CAPTCHA_INVALID;
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
                if (!this.IsCaptchaValid(""))
                {
                    ModelState.AddModelError("CaptchaInputText", captchaMessage);
                    this.AddNotification(captchaMessage, NotificationType.ERROR);
                    return View(model);
                }

                if (!string.IsNullOrEmpty(model.Email))
                    model.Email = model.Email.Trim();

                var apiUserResult = await AccountServices.SendEmailAtiveAccount(new ApiSendEmailActiveAccountModel { Email = model.Email, Time = DateTime.Now.ToString(Constant.DATEFORMAT_yyyyMMddHHmmss) });
                if (apiUserResult != null)
                {
                    message = apiUserResult.Msg;
                    if (string.IsNullOrEmpty(message))
                        message = UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                    if (apiUserResult.Code == (int)HttpStatusCode.OK)
                    {
                        this.AddNotification(message, NotificationType.SUCCESS);
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        this.AddNotification(message, NotificationType.ERROR);
                    }
                }
                else
                {
                    this.AddNotification(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(UserWebResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT, NotificationType.ERROR);

                logger.Error(string.Format("Failed to ResendEmailActive because: {0}", ex.ToString()));
            }

            return View(model);
        }

        #region Helpers

        private string HashingSignInData(ApiAuthLoginModel model)
        {
            var rawDataToHash = string.Format("{0}|{1}|{2}", model.UserName, model.Password, model.Time);

            return Utility.Md5HashingData(rawDataToHash);
        }

        private ApiAuthLoginModel ReFormatLoginModel(WebAuthLoginViewModel model)
        {
            var apiModel = new ApiAuthLoginModel();

            if (model != null)
            {
                apiModel.UserName = model.UserName.Trim();
                apiModel.Password = Utility.Md5HashingData(model.Password);
                apiModel.Time = DateTime.Now.ToString(Constant.DATEFORMAT_yyyyMMddHHmmss);
            }

            apiModel.Hash = HashingSignInData(apiModel);

            return apiModel;
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
            //var parmsList = "?UserId={0}&Token={1}";
            //if (model.Data != null)
            //{
            //    parmsList = string.Format(parmsList, model.Data.Id, model.TokenKey);
            //}

            if (!string.IsNullOrEmpty(model.ReturnUrl))
            {
                try
                {
                    if (!Url.IsLocalUrl(model.ReturnUrl))
                    {
                        Uri uri = new Uri(model.ReturnUrl);
                        model.ReturnUrl = uri.GetLeftPart(UriPartial.Path);
                    }
                }
                catch
                {
                }
                //model.ReturnUrl = model.ReturnUrl + parmsList;
                return Redirect(model.ReturnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private ApiRegisterModel ReFormatRegisterModel(WebAccountRegisterModel model)
        {
            var apiModel = new ApiRegisterModel();
            if (model != null)
            {
                apiModel.UserName = !string.IsNullOrEmpty(model.UserName) ? model.UserName.Trim() : string.Empty;
                apiModel.Password = Utility.Md5HashingData(model.Password);
                apiModel.Display_Name = !string.IsNullOrEmpty(model.Display_Name) ? model.Display_Name.Trim() : string.Empty;
                apiModel.Time = DateTime.Now.ToString(Constant.DATEFORMAT_yyyyMMddHHmmss);
                if (IsEmail(model.UserName))
                {
                    apiModel.Email = model.UserName;
                }

                if (IsPhoneNumber(model.UserName))
                {
                    apiModel.Phone = model.UserName;
                }
            }

            return apiModel;
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

            if (errorCode == 0)
            {
                var isPhoneNumber = IsPhoneNumber(model.UserName);
                var isEmail = IsEmail(model.UserName);
                if (!isPhoneNumber && !isEmail)
                {
                    errorCode = EnumCommonCode.ErrorTimeInvalid;
                    message = UserWebResource.REGISTER_ERROR_USERNAME;
                    ModelState.AddModelError("UserName", message);
                }
            }

            return errorCode;
        }

        private string GetRegisterResponseMessage(int statusCode)
        {
            var message = string.Empty;
            if (statusCode == EnumCommonCode.Success)
            {
                message = UserWebResource.MS_REGISTER_SUCCESS;
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
            //if (string.IsNullOrEmpty(domain)) domain = Request.Url.Host;
            //if (UserCookieManager.IsAuthenticated())
            //{
            //    try
            //    {
            //        var userLogin = UserCookieManager.GetUserCookie<UserCookieModel>(MySiteSettings.SSOCommonUserKey);
            //        var currentUser = _userStore.GetUserById(userLogin.UserId);

            //        if (currentUser != null)
            //        {
            //            //_redisService.ClearCachedDataByKeyAndDomain(currentUser.UserName + currentUser.PasswordHash, domain);
            //            //_redisService.ClearCachedDataByKeyAndDomain(currentUser.Email + currentUser.PasswordHash, domain);
            //            //_redisService.ClearCachedDataByKeyAndDomain(currentUser.PhoneNumber + currentUser.PasswordHash, domain);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        var strError = "Could not ClearLoginCacheData in web due to: {0}";
            //        strError = string.Format(ex.Message);
            //    }
            //}

        }

        private int SendEmailToRecoverPwd(string userId, string email, string pwdType)
        {
            var result = EnumCommonCode.Success;
            //Begin sending email
            var emailModel = new EmailModel
            {
                Sender = SystemSettings.EmailSender,
                SenderPwd = SystemSettings.EmailSenderPwd,
                Receiver = email,
                // Subject = EmailResource.SUBJECT_RECOVER_PASSWORD
            };
            emailModel.Body = EmailHelper.GetEmailTemplate("RecoverPassword");

            var recoverData = string.Format("{0}|{1}", userId, pwdType);
            //var dataEncrypt = Utility.TripleDESEncrypt(SystemSettings.EncryptKey, recoverData);
            var dataEncrypt = Utility.EncryptText(recoverData, SystemSettings.EncryptKey);

            var rawData = string.Format("{0}.{1}.{2}", Utility.Md5HashingData(DateTime.Now.ToString(Constant.DATEFORMAT_yyyyMMddHHmmss)), dataEncrypt, Utility.Md5HashingData(userId));

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

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        private int SendEmailToActiveAccount(string userName, string email)
        {
            var result = EnumCommonCode.Success;
            //Begin sending email
            var emailModel = new EmailModel
            {
                Sender = SystemSettings.EmailSender,
                SenderPwd = SystemSettings.EmailSenderPwd,
                Receiver = email,
                //Subject = EmailResource.SUBJECT_RECOVER_PASSWORD
            };
            emailModel.Body = EmailHelper.GetEmailTemplate("RecoverPassword");

            var recoverData = string.Format("{0}|{1}", userName, email);
            //var dataEncrypt = Utility.TripleDESEncrypt(SystemSettings.EncryptKey, recoverData);
            var dataEncrypt = Utility.EncryptText(recoverData, SystemSettings.EncryptKey);

            var rawData = string.Format("{0}.{1}.{2}", Utility.Md5HashingData(DateTime.Now.ToString(Constant.DATEFORMAT_yyyyMMddHHmmss)), dataEncrypt, Utility.Md5HashingData(userName));

            var baseUrl = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);
            var activeLink = string.Format("{0}/{1}/{2}?token={3}", baseUrl, "WebAuth", "Password_Reset", rawData);
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
    }
}
