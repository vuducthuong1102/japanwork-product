using System;
using System.Net;
using System.Web.Http;
using System.Threading.Tasks;
using ApiJobMarket.Logging;
using ApiJobMarket.ActionResults;
using ApiJobMarket.Helpers;
using ApiJobMarket.Models;
using ApiJobMarket.DB.Sql.Stores;
using Newtonsoft.Json;
using ApiJobMarket.Helpers.Validation;
using Autofac;
using System.Collections.Generic;
using ApiJobMarket.SharedLib.Extensions;
using ApiJobMarket.DB.Sql.Entities;
using System.Linq;

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/countries")]
    public class ApiCountryController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiCountryController>();

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetList()
        {
            var requestName = "Countries-GetList";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;

                var listCountry = CommonHelpers.GetCountries();

                await Task.FromResult(listCountry);                

                returnModel.value = listCountry;

                jsonString = JsonConvert.SerializeObject(returnModel);

                //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);
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

        //[HttpGet]
        //[Route("{id:int}")]
        //public async Task<IHttpActionResult> GetDetail(int id)
        //{
        //    CreateDocumentApi(id);
        //    var requestName = "Countries-GetDetail";
        //    var returnModel = new ApiResponseCommonModel();
        //    var jsonString = string.Empty;
        //    if (!ModelState.IsValid)
        //    {
        //        return ModelValidation.ApiValidate(ModelState, returnModel);
        //    }

        //    try
        //    {
        //        //logger.Debug(string.Format("Begin {0} request", requestName));

        //        //Check and register from database
        //        var returnCode = EnumCommonCode.Success;

        //        var storeCountry = GlobalContainer.IocContainer.Resolve<IStoreCountry>();

        //        var info = storeCountry.GetById(id);
        //        await Task.FromResult(info);

        //        if(info != null)
        //        {
        //            ApiResponseCountryItemModel returnInfo = new ApiResponseCountryItemModel();
        //            returnInfo.id = info.id;
        //            returnInfo.city = info.city;
        //            returnInfo.furigana = info.furigana;
        //            returnInfo.prefecture_id = info.prefecture_id;

        //            returnModel.value = returnInfo;
        //        }

        //        jsonString = JsonConvert.SerializeObject(returnModel);

        //        //logger.DebugFormat("{0} Returned Model encrypted = {1}", requestName, jsonString);
        //    }
        //    catch (Exception ex)
        //    {
        //        string strError = string.Format("Failed for {0} request: {1}", requestName, ex.Message);
        //        logger.ErrorException(strError, ex);


        //        return CatchJsonExceptionResult(returnModel);
        //    }
        //    finally
        //    {
        //        //logger.Debug(string.Format("Ended {0} request", requestName));
        //    }

        //    return new JsonActionResult(HttpStatusCode.OK, jsonString);
        //}
    }
}
