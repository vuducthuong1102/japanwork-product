using System;
using System.Net;
using System.Web.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using Autofac;
using System.Web;
using Manager.SharedLibs.Logging;
using Manager.WebApp.Models;
using Manager.WebApp.Helpers;
using MsSql.AspNet.Identity.Stores;
using Manager.SharedLibs;

namespace Manager.WebApp.Controllers
{
    [RoutePrefix("api/user")]
    public class ApiUserController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiUserController>();

        [HttpPost]
        [Route("operators")]
        public async Task<IHttpActionResult> GetOperatorsByPage(ApiCommonFilterModel model)
        {
            var requestName = "User-GetOperatorsByPage";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var _store = GlobalContainer.IocContainer.Resolve<IStoreUser>();

                var myList = _store.GetByPage(model);
                await Task.FromResult(myList);
                
                if (myList.HasData())
                {
                    foreach (var item in myList)
                    {
                        item.Avatar = SocialCdnHelper.GetAvatarOrDefault(item.Avatar);
                    }

                    returnModel.value = myList;
                    returnModel.total = myList[0].TotalCount;
                }

                jsonString = JsonConvert.SerializeObject(returnModel);
            }
            catch (Exception ex)
            {
                string strError = string.Format("Failed for {0} request: {1}", requestName, ex.Message);
                logger.ErrorException(strError, ex);

                return CatchJsonExceptionResult(returnModel);
            }
            finally
            {
                //logger.Debug(string.Format("Ended {0} request", requestName));
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }

        [HttpPost]
        [Route("operator")]
        public async Task<IHttpActionResult> GetOperatorProfile(ApiCommonOperatorInfoModel model)
        {
            var requestName = "User-GetOperatorProfile";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                var _store = GlobalContainer.IocContainer.Resolve<IStoreUser>();

                var info = _store.GetUserById(model.Id);
                await Task.FromResult(info);
                if (info != null)
                {
                    info.Avatar = SocialCdnHelper.GetAvatarOrDefault(info.Avatar);
                }

                returnModel.value = info;

                jsonString = JsonConvert.SerializeObject(returnModel);
            }
            catch (Exception ex)
            {
                string strError = string.Format("Failed for {0} request: {1}", requestName, ex.Message);
                logger.ErrorException(strError, ex);

                return CatchJsonExceptionResult(returnModel);
            }
            finally
            {
                //logger.Debug(string.Format("Ended {0} request", requestName));
            }

            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }
    }
}
