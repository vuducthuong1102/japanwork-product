using System;
using System.Net;
using System.Web.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SingleSignOn.Logging;
using SingleSignOn.Services;
using SingleSignOn.ActionResults;
using SingleSignOn.Extensions;
using SingleSignOn.Helpers;
using SingleSignOn.Models;

namespace SingleSignOn.Controllers
{
    [RoutePrefix("api/manage")]    
    //[IPAddressFilter]
    public class ManagingController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ManagingController>();
        private IApiAuthRedisService _authService;
        public ManagingController(IApiAuthRedisService authService)
        {
            _authService = authService;
        }

        //[Route("genkey")]
        //[HttpGet]
        //public async Task<IHttpActionResult> GenerateOTPKey()
        //{
        //    //var model = new OTPKeyModel
        //    //{
        //    //    OTPKey = AES256Encryption.GenerateRandomKey(),                
        //    //    KeyGenDate = DateTime.Now.ToEpochTime()
        //    //};

        //    //var jsonModelString = JsonConvert.SerializeObject(model);
        //    //await Task.FromResult(jsonModelString);

        //    //return new JsonActionResult(HttpStatusCode.OK, jsonModelString);

        //    //var result = await _authService.SetAuthKeyModelAsync(model);
        //    //return Json(model);
        //    //if (string.IsNullOrEmpty(result))
        //    //{
        //    //    var jsonModelString = JsonConvert.SerializeObject(model);
        //    //    return new JsonActionResult(HttpStatusCode.OK, jsonModelString);
        //    //}
        //    //else
        //    //{
        //    //    return new TextPlainActionResult(HttpStatusCode.BadRequest, result);
        //    //}
        //}
    }
}
