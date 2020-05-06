using System;
using System.Net;
using System.Web.Http;
using System.Threading.Tasks;
using ApiJobMarket.Logging;
using ApiJobMarket.ActionResults;
using ApiJobMarket.Helpers;
using ApiJobMarket.Models;
using Newtonsoft.Json;
using ApiJobMarket.Helpers.Validation;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/policy")]
    public class ApiPolicyController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiPolicyController>();

        [HttpGet]
        [Route("privacy")]
        public async Task<HttpResponseMessage> Privacy(string platform = "")
        {            
            var requestName = "Policy-Privacy";
            var response = new HttpResponseMessage();

            try
            {
                var returnCode = EnumCommonCode.Success;
                var currentLang = GetCurrentRequestLang();
                if (string.IsNullOrEmpty(platform))
                {
                    platform = "android";
                }

                var viewName = string.Format("../Policy/{0}/privacy_{1}", "ios", currentLang);

                if (platform.Equals("android", StringComparison.CurrentCultureIgnoreCase))
                {
                    viewName = string.Format("../Policy/{0}/privacy_{1}", "android", currentLang);
                }
                else if(platform.Equals("ios", StringComparison.CurrentCultureIgnoreCase))
                {
                    viewName = string.Format("../Policy/{0}/privacy_{1}", "ios", currentLang);
                }                

                var result = RenderPartialViewAsString(viewName, null);

                await Task.FromResult(result);

                response.Content = new StringContent(result, Encoding.UTF8, "text/html");

                //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);
            }
            catch (Exception ex)
            {
                string strError = string.Format("Failed for {0} request: {1}", requestName, ex.Message);
                logger.ErrorException(strError, ex);
            }
            finally
            {

            }

            return response;
        }

        [HttpGet]
        [Route("terms_of_use")]
        public async Task<HttpResponseMessage> TermsOfUse(string platform = "")
        {
            var requestName = "Policy-TermsOfUse";
            var response = new HttpResponseMessage();

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var currentLang = GetCurrentRequestLang();

                var viewName = string.Format("../Policy/{0}/terms_of_use_{1}", "ios", currentLang);

                if (platform.Equals("android", StringComparison.CurrentCultureIgnoreCase))
                {
                    viewName = string.Format("../Policy/{0}/terms_of_use_{1}", "android", currentLang);
                }
                else if (platform.Equals("ios", StringComparison.CurrentCultureIgnoreCase))
                {
                    viewName = string.Format("../Policy/{0}/terms_of_use_{1}", "ios", currentLang);
                }

                var result = RenderPartialViewAsString(viewName, null);

                await Task.FromResult(result);

                response.Content = new StringContent(result, Encoding.UTF8, "text/html");

                //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);
            }
            catch (Exception ex)
            {
                string strError = string.Format("Failed for {0} request: {1}", requestName, ex.Message);
                logger.ErrorException(strError, ex);
            }
            finally
            {
                //logger.Debug(string.Format("Ended {0} request", requestName));
            }

            return response;
        }
    }
}
