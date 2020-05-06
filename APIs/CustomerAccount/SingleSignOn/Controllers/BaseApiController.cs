using System.Web;
using System.Web.Http;
using Autofac;
using Newtonsoft.Json;
using SingleSignOn.DB.Sql.Stores;
using SingleSignOn.Logging;
using SingleSignOn.Settings;

namespace SingleSignOn.Controllers
{
    /// <summary>
    /// This class will act as a base class which other Web API controllers will inherit from, 
    /// for now it will contain three basic methods
    /// </summary>

    public class BaseApiController : ApiController
    {
        private readonly ILog logger = LogProvider.For<BaseApiController>();

        private IStoreDocumentApi _documentStore;
        public BaseApiController()
        {
            _documentStore = GlobalContainer.IocContainer.Resolve<IStoreDocumentApi>();
        }

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

        public void CreateDocumentApi(object ob)
        {
            if (SystemSettings.IsLogParamater)
            {
                string linkUrl = HttpContext.Current.Request.Url.AbsolutePath;

                string data = JsonConvert.SerializeObject(ob);
                _documentStore.Insert(linkUrl, data);
            }
        }

    }
}