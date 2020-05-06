using Newtonsoft.Json;
using SingleSignOn.ActionResults;
using SingleSignOn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.ModelBinding;

namespace SingleSignOn.Helpers.Validation
{
    public class ModelValidation
    {
        public static JsonActionResult ApiValidate(ModelStateDictionary modelState, dynamic returnModel)
        {
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
            return new JsonActionResult(HttpStatusCode.OK, jsonString);
        }
    }
}