using MyCloud.SharedLib.Logging;
using System;
using System.Web;
using System.Web.Http;

namespace MyCloud.Controllers
{
    /// <summary>
    /// This class will act as a base class which other Web API controllers will inherit from, 
    /// for now it will contain three basic methods
    /// </summary>
    public class BaseApiController : ApiController
    {
        private readonly ILog logger = LogProvider.For<BaseApiController>();


        protected IHttpActionResult CreateBadRequest(string strErrorCode, string strErrorMessage)
        {
            var strError = strErrorCode + "-" + strErrorMessage;
            return CreateBadRequest(strError);
        }

        protected IHttpActionResult CreateBadRequest(string strErrorMessage)
        {
            logger.ErrorFormat("Return BadRequest: {0}", strErrorMessage);
            return BadRequest(strErrorMessage);
        }
       
        public string GetCurrentRequestLang()
        {
            var lang = "vi-VN";
            if (HttpContext.Current.Request.Headers["Accept-Language"] != null)
            {
                lang = HttpContext.Current.Request.Headers["Accept-Language"].ToString();
            }

            return lang;
        }
    }
}