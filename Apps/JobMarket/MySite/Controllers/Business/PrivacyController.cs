using System.Web.Mvc;
using MySite.Logging;

namespace MySite.Controllers
{
    public class PrivacyController : BaseController
    {
        private readonly ILog logger = LogProvider.For<PrivacyController>();


        public PrivacyController()
        {
           
        }

        public ActionResult TermsConditions()
        {
            return View();
        }


        public ActionResult PrivacyPolicy()
        {
            return View();
        }
    }
}
