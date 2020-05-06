using Newtonsoft.Json;
using MySite.ActionResults;
using MySite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.ModelBinding;

namespace MySite.Helpers.Validation
{
    public class ModelValidation
    {
        public static JsonActionResult ApiValidate(ModelStateDictionary modelState, dynamic returnModel)
        {
            //string messages = string.Join("; ", modelState.Values
            //                             .SelectMany(x => x.Errors)
            //                             .Select(x => x.ErrorMessage + x.Exception));


            //messages = string.Empty;
            //foreach (var currentState in modelState.Values)
            //{
            //    foreach (var error in currentState.Errors)
            //    {
            //        messages += error.ErrorMessage + error.Exception + "; ";
            //    }
            //}

            //returnModel.Code = EnumCommonCode.Error;
            //returnModel.Msg = messages;

            //var jsonString = JsonConvert.SerializeObject(returnModel);
            //return new JsonActionResult(HttpStatusCode.BadRequest, jsonString);

            var messages = string.Empty;
            foreach (var modelStateKey in modelState.Keys)
            {
                var value = modelState[modelStateKey];
                foreach (var error in value.Errors)
                {
                    messages += modelStateKey.Replace("model.","") + ": " + error.ErrorMessage + error.Exception + "; ";
                }
            }

            returnModel.Code = EnumCommonCode.Error;
            returnModel.Msg = messages;

            var jsonString = JsonConvert.SerializeObject(returnModel);
            return new JsonActionResult(HttpStatusCode.BadRequest, jsonString);
        }
    }
}