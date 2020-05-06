using MySite.App_Start;
using MySite.Models;
using MySite.Models.Account;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Newtonsoft.Json;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace MySite
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/WebAuth/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");
            SocialProviderModel facebook = new SocialProviderModel
            {
                Code = "Facebook",
                ClientId = "1283899668348874",
                ClientSecret = "ba1f74ad1e82be4a6357d99cfeb08da0"
            };
            var facebookAuthenticationOptions = new FacebookAuthenticationOptions()
            {
                AppId = facebook.ClientId,
                AppSecret = facebook.ClientSecret,
                //BackchannelHttpHandler = new FacebookBackChannelHandler(),
                //UserInformationEndpoint = "https://graph.facebook.com/v2.4/me?fields=id,name,email,first_name,last_name"
            };
            facebookAuthenticationOptions.Scope.Add("email");
            facebookAuthenticationOptions.Scope.Add("public_profile");

            // Set requested fields
            facebookAuthenticationOptions.Fields.Add("email");
            facebookAuthenticationOptions.Fields.Add("first_name");
            facebookAuthenticationOptions.Fields.Add("last_name");

            //facebookAuthenticationOptions.Provider = new FacebookAuthenticationProvider()
            //{
            //    OnAuthenticated = (context) =>
            //    {
            //        // Attach the access token if you need it later on for calls on behalf of the user
            //        context.Identity.AddClaim(new System.Security.Claims.Claim("FacebookAccessToken", context.AccessToken));

            //        foreach (var claim in context.User)
            //        {
            //            //var claimType = string.Format("urn:facebook:{0}", claim.Key);
            //            var claimType = string.Format("{0}", claim.Key);
            //            string claimValue = claim.Value.ToString();

            //            if (!context.Identity.HasClaim(claimType, claimValue))
            //                context.Identity.AddClaim(new System.Security.Claims.Claim(claimType, claimValue, "XmlSchemaString", "Facebook"));
            //        }

            //        return Task.FromResult(0);
            //    }
            //};

            app.UseFacebookAuthentication(facebookAuthenticationOptions);
            //facebookAuthenticationOptions.Scope.Add("email");
            //facebookAuthenticationOptions.Scope.Add("first_name");
            //app.UseFacebookAuthentication(facebookAuthenticationOptions);
            // Project:  lud-alex-model-zx-test-login


            SocialProviderModel google = new SocialProviderModel
            {
                Code = "Google",
                ClientId = "720014388885-mtsl6hs38puaqushvkd1u91vjd7p4l82.apps.googleusercontent.com",
                ClientSecret = "bnU4_WPy2ppDRCZjh-zb9_am"
            };

            //new CacheBussiness().CacheSocialProvider().FirstOrDefault(c => c.Code.Trim().Equals("Google"));
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = google.ClientId,
                ClientSecret = google.ClientSecret,
                Provider = new GoogleOAuth2AuthenticationProvider(),
            });
        }
    }
    public class FacebookBackChannelHandler : HttpClientHandler
    {
        protected override async System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var result = await base.SendAsync(request, cancellationToken);
            if (!request.RequestUri.AbsolutePath.Contains("access_token"))
                return result;

            // For the access token we need to now deal with the fact that the response is now in JSON format, not form values. Owin looks for form values.
            var content = await result.Content.ReadAsStringAsync();
            var facebookOauthResponse = JsonConvert.DeserializeObject<FacebookOauthResponse>(content);

            var outgoingQueryString = HttpUtility.ParseQueryString(string.Empty);
            outgoingQueryString.Add(nameof(facebookOauthResponse.access_token), facebookOauthResponse.access_token);
            outgoingQueryString.Add(nameof(facebookOauthResponse.expires_in), facebookOauthResponse.expires_in + string.Empty);
            outgoingQueryString.Add(nameof(facebookOauthResponse.token_type), facebookOauthResponse.token_type);
            var postdata = outgoingQueryString.ToString();

            var modifiedResult = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(postdata)
            };

            return modifiedResult;
        }
    }

    public class FacebookOauthResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
}