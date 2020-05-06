using ApiJobMarket.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace ApiJobMarket.Services
{
    public static class CommonServices
    {
        public static HttpClient AuthorizationHttpClientCore(this HttpClient client)
        {
            //Set Basic Auth
            var user = (!string.IsNullOrEmpty(AuthorizationSingleSignOnSettings.Username)) ? AuthorizationSingleSignOnSettings.Username : "username";
            var password = (!string.IsNullOrEmpty(AuthorizationSingleSignOnSettings.Password)) ? AuthorizationSingleSignOnSettings.Password : "password";
            var authorizeHeaderKey = (!string.IsNullOrEmpty(AuthorizationSingleSignOnSettings.HeaderKey)) ? AuthorizationSingleSignOnSettings.HeaderKey : "key";
            var authorizeHeaderValue = (!string.IsNullOrEmpty(AuthorizationSingleSignOnSettings.HeaderValue)) ? AuthorizationSingleSignOnSettings.HeaderValue : "value";

            var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);

            client.DefaultRequestHeaders.Add(authorizeHeaderKey, authorizeHeaderValue);

            return client;
        }
    }
}