using System.Web.Mvc;

namespace FileReading.Controllers
{
    public class ErrorController : BaseController
    {
        public ActionResult FileNotFound()
        {
            return View();
        }
    }
}