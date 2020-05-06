using System;
using System.Web.Mvc;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Helpers;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;

namespace Manager.WebApp.Controllers
{
    public class PolicyController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<PolicyController>();

        [HttpGet]
        [Route("privacy")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> Privacy(string platform = "")
        {
            var requestName = "Policy-Privacy";
            var response = new HttpResponseMessage();

            try
            {
                var returnCode = EnumCommonCode.Success;
                var currentLang = GetCurrentLanguageOrDefault();
                if (string.IsNullOrEmpty(platform))
                {
                    platform = "android";
                }

                var viewName = string.Format("../Policy/{0}/privacy_{1}", "ios", currentLang);

                if (platform.Equals("android", StringComparison.CurrentCultureIgnoreCase))
                {
                    viewName = string.Format("../Policy/{0}/privacy_{1}", "android", currentLang);
                }
                else if (platform.Equals("ios", StringComparison.CurrentCultureIgnoreCase))
                {
                    viewName = string.Format("../Policy/{0}/privacy_{1}", "ios", currentLang);
                }

                var result = PartialViewAsString(viewName, null);

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
        [AllowAnonymous]
        public ActionResult TermsOfUse(string platform = "")
        {
            var requestName = "Policy-TermsOfUse";
            var response = new HttpResponseMessage();
            var currentLang = GetCurrentLanguageOrDefault();
            var viewName = string.Format("../Policy/{0}/terms_of_use_{1}", "web", currentLang);

            try
            {
                var returnCode = EnumCommonCode.Success;             
                if (string.IsNullOrEmpty(platform))
                {
                    viewName = string.Format("../Policy/{0}/terms_of_use_{1}", "web", currentLang);
                }
                else
                {
                    viewName = string.Format("../Policy/{0}/terms_of_use_{1}", platform, currentLang);
                }                

                //var result = PartialViewAsString(viewName, null);

                //await Task.FromResult(result);

                //response.Content = new StringContent(result, Encoding.UTF8, "text/html");

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

            return View(viewName);
        }
    }
}