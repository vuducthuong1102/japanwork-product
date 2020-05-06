using System;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using SingleSignOn.Areas.HelpPage.ModelDescriptions;
using SingleSignOn.Areas.HelpPage.Models;
using SingleSignOn.DB.Sql.Stores;

namespace SingleSignOn.Areas.HelpPage.Controllers
{
    /// <summary>
    /// The controller that will handle requests for the help page.
    /// </summary>
    public class HelpController : Controller
    {
        private const string ErrorViewName = "Error";
        private IStoreDocumentApi _storeDocumentApi;

        public HelpController(IStoreDocumentApi storeDocumentApi)
            : this(GlobalConfiguration.Configuration)
        {
            _storeDocumentApi = storeDocumentApi;
        }

        public HelpController(HttpConfiguration config)
        {
            Configuration = config;
        }

        public HttpConfiguration Configuration { get; private set; }

        public ActionResult Index()
        {
            ViewBag.DocumentationProvider = Configuration.Services.GetDocumentationProvider();
            return View(Configuration.Services.GetApiExplorer().ApiDescriptions);
        }

        public ActionResult Api(string apiId)
        {
            if (!String.IsNullOrEmpty(apiId))
            {
                HelpPageApiModel apiModel = Configuration.GetHelpPageApiModel(apiId);
                if (apiModel != null)
                {
                    return View(apiModel);
                }
            }

            return View(ErrorViewName);
        }

        public ActionResult GetSample(string url)
        {
            var apistr = url.Replace("Help/Api/POST-", "").Replace("-", "/");
            string linkUrl = Request.Url.Scheme + "://" + Request.Url.Authority + apistr;
            string htmlReturn = string.Empty;
            try
            {
                var item = _storeDocumentApi.GetByLinkUrl(apistr);
                if (item != null)
                {
                    htmlReturn = JObject.Parse(item.Data).ToString();
                }

            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying GetSample because: {0}", ex.ToString());
            }

            return Json(new { html = htmlReturn, linkurl = linkUrl }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ResourceModel(string modelName)
        {
            if (!String.IsNullOrEmpty(modelName))
            {
                ModelDescriptionGenerator modelDescriptionGenerator = Configuration.GetModelDescriptionGenerator();
                ModelDescription modelDescription;
                if (modelDescriptionGenerator.GeneratedModels.TryGetValue(modelName, out modelDescription))
                {
                    return View(modelDescription);
                }
            }

            return View(ErrorViewName);
        }

        public ActionResult ApiLogin()
        {
            return View();
        }

        public ActionResult RefreshToken()
        {
            return View();
        }

        public ActionResult OTP()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        public ActionResult UpdateProfile()
        {
            return View();
        }

        public ActionResult Logout()
        {
            return View();
        }

        public ActionResult CheckUserExists()
        {
            return View();
        }

        public ActionResult ChangeAuthMethod()
        {
            return View();
        }

        public ActionResult CheckPwd2IsValid()
        {
            return View();
        }

        public ActionResult GetInfo()
        {
            return View();
        }

        public ActionResult RecoverPassword()
        {
            return View();
        }
    }
}