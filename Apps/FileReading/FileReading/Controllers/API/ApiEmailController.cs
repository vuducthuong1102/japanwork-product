using FileReading.ActionResults;
using FileReading.Helpers;
using FileReading.Logging;
using FileReading.Models;
using FileReading.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace FileReading.Controllers
{
    //[Authorize]
    [RoutePrefix("api/email")]
    public class ApiEmailController : ApiController
    {
        private readonly ILog logger = LogProvider.For<ApiUploadFileController>();

        [AllowAnonymous]
        [HttpPost]
        [Route("generate_stack_files")]
        public IHttpActionResult GenerateStackFiles(BatchEmailModel model)
        {
            var returnModel = new ResponseApiModel();
            try
            {
                if (model == null)
                {
                    returnModel.Code = EnumCommonCode.Error;
                    returnModel.Msg = "Model is invalid. Model could not be null";

                    return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
                }

                var hasList = (model.Receivers != null && model.Receivers.Count > 0);

                if (!hasList)
                {
                    returnModel.Code = EnumCommonCode.Error;
                    returnModel.Msg = "Model is invalid. Receivers list could not be null";

                    return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
                }                              

                if (model.Receivers != null && model.Receivers.Count > 0)
                {
                    //Begin generating files
                    CreateEmailStackFiles(model);
                }                

                returnModel.Code = EnumCommonCode.Success;
            }
            catch (Exception ex)
            {
                string strError = "Failed for GenerateStackFiles: " + ex.Message;
                returnModel.Code = EnumCommonCode.Error;
                returnModel.Msg = strError;
                logger.Error(strError);

                return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
            }

            return new JsonActionResult(HttpStatusCode.OK, JsonConvert.SerializeObject(returnModel));
        }

        #region Helpers

        public void CreateEmailStackFiles(BatchEmailModel model)
        {
            var dtNow = DateTime.UtcNow;
            var preFixFormat = "{0}_{1}";
            var prefix = string.Empty;
            var surFix = EpochTime.GetIntDate(dtNow);
            var fileNameFormat = "{0}_{1}_{2}";
            var fileExt = ".json";
            var folderPath = SystemSettings.GenerateFileUrl + "/Email/ToSend";        
            try
            {
                // Check if the directory we are saving to exists
                if (!Directory.Exists(folderPath))
                {
                    // If it doesn't exist, create the directory
                    Directory.CreateDirectory(folderPath);
                }
                
                foreach (var item in model.Receivers)
                {
                    var itemModel = model;
                    itemModel.Receivers = null;

                    if(model.TargetType == (int)EnumEmailTargetType.JobSeeker)
                        prefix = string.Format(preFixFormat, "JK", item.job_seeker_id);

                    else if(model.TargetType == (int)EnumEmailTargetType.Company)
                        prefix = string.Format(preFixFormat, "COM", item.company_id);

                    itemModel.Receiver = item.email;                    

                    var fileName = string.Format(fileNameFormat, prefix, surFix, fileExt);
                    fileName = folderPath + "/" + fileName;

                    var fileContent = JsonConvert.SerializeObject(itemModel);

                    //Save new file
                    File.WriteAllText(fileName, fileContent);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed for CreateEmailStackFiles because: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        #endregion
    }
}