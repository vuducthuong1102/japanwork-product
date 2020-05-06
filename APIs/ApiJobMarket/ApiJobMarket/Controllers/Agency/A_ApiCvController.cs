using System;
using System.Net;
using System.Web.Http;
using System.Threading.Tasks;
using ApiJobMarket.Logging;
using ApiJobMarket.ActionResults;
using ApiJobMarket.Helpers;
using ApiJobMarket.Models;
using Newtonsoft.Json;
using ApiJobMarket.Helpers.Validation;
using ApiJobMarket.SharedLib.Extensions;
using System.Collections.Generic;
using ApiJobMarket.DB.Sql.Stores;
using Autofac;
using System.Web;
using System.Linq;
using ApiJobMarket.Services;
using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.ShareLibs;
using ApiJobMarket.Resources;
using System.Dynamic;

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/A_cv")]
    public class A_ApiCvController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<A_ApiCvController>();
        private readonly string DATE_FORMAT = "dd-MM-yyyy";

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetDetail(int id)
        {
            CreateDocumentApi(id);
            var requestName = "CVs-GetDetail";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var returnCode = EnumCommonCode.Success;
                //var storeCv = GlobalContainer.IocContainer.Resolve<IStoreCv>();

                //var info = storeCv.GetDetail(id);
                //await Task.FromResult(info);

                //if (info != null)
                //{
                //    info.image_full_path = CdnHelper.SocialGetFullImgPath(info.image);
                //}
                var info = CvHelpers.GetBaseInfoCv(id);
                if (info != null)
                {
                    info.jobseeker = JobSeekerHelpers.GetBaseInfo(info.job_seeker_id);
                    if (info.jobseeker != null)
                    {
                        info.jobseeker.image = CdnHelper.SocialGetFullImgPath(info.jobseeker.image);
                    }
                }
                await Task.FromResult(info);

                returnModel.value = info;
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

        #region Helpers

        private IdentityCv ExtractFormData(ApiCvUpdateModel model)
        {
            var info = new IdentityCv();
            info.id = Utils.ConvertToIntFromQuest(model.cv.id);
            info.job_seeker_id = Utils.ConvertToIntFromQuest(model.cv.job_seeker_id);
            info.cv_title = model.cv.cv_title;
            try
            {
                info.date = DateTime.ParseExact(model.cv.date, "dd-MM-yyyy", null);
            }
            catch
            {
                info.date = DateTime.Now;
            }

            info.fullname = model.cv.fullname;
            info.fullname_furigana = model.cv.fullname_furigana;
            info.gender = Utils.ConvertToIntFromQuest(model.cv.gender);

            try
            {
                info.birthday = DateTime.ParseExact(model.cv.birthday, "dd-MM-yyyy", null);
            }
            catch
            {
                info.birthday = DateTime.Now;
            }

            info.email = model.cv.email;
            info.phone = model.cv.phone;
            info.japanese_level_number = model.cv.japanese_level_number;
            info.marriage = (model.cv.marriage == 0) ? false : true;
            info.dependent_num = Utils.ConvertToIntFromQuest(model.cv.dependent_num);
            info.highest_edu = Utils.ConvertToIntFromQuest(model.cv.highest_edu);
            info.pr = model.cv.pr;
            info.hobby_skills = model.cv.hobby_skills;
            info.reason = model.cv.reason;
            info.time_work = model.cv.time_work;
            info.aspiration = model.cv.aspiration;
            info.form = Utils.ConvertToIntFromQuest(model.cv.form);
            info.image = model.cv.image;
            info.reason_pr = model.cv.reason_pr;
            info.check_address = (model.cv.check_address == 0) ? false : true;
            info.check_work = (model.cv.check_work == 0) ? false : true;
            info.check_ceti = (model.cv.check_ceti == 0) ? false : true;
            info.check_timework = (model.cv.check_timework == 0) ? false : true;
            info.check_aspiration = (model.cv.check_aspiration == 0) ? false : true;
            info.station_id = Utils.ConvertToIntFromQuest(model.cv.station_id);
            info.train_line_id = Utils.ConvertToIntFromQuest(model.cv.train_line_id);

            if (model.work_history.HasData())
            {
                info.work_history = new List<IdentityJobSeekerWorkHistory>();
                foreach (var item in model.work_history)
                {
                    if (item.id > 0)
                        continue;

                    var work = new IdentityJobSeekerWorkHistory();

                    work.id = Utils.ConvertToInt32(item.id);
                    work.company = item.company;
                    work.content_work = item.content_work;
                    work.address = item.address;

                    try
                    {
                        if (!string.IsNullOrEmpty(item.start_date))
                            work.start_date = DateTime.ParseExact(item.start_date, DATE_FORMAT, null);

                        if (!string.IsNullOrEmpty(item.end_date))
                            work.end_date = DateTime.ParseExact(item.end_date, DATE_FORMAT, null);
                    }
                    catch
                    {
                        //work.start_date = DateTime.Now;
                        //work.end_date = DateTime.Now;
                    }

                    work.form = Utils.ConvertToIntFromQuest(item.form);
                    work.status = Utils.ConvertToIntFromQuest(item.status);

                    info.work_history.Add(work);
                }
            }

            if (model.edu_history.HasData())
            {
                info.edu_history = new List<IdentityJobSeekerEduHistory>();
                foreach (var item in model.edu_history)
                {
                    if (item.id > 0)
                        continue;

                    var edu = new IdentityJobSeekerEduHistory();

                    edu.id = Utils.ConvertToInt32(item.id);
                    edu.school = item.school;
                    edu.address = item.address;
                    edu.qualification_id = Utils.ConvertToIntFromQuest(item.qualification_id);
                    edu.major_id = Utils.ConvertToIntFromQuest(item.major_id);

                    try
                    {
                        if (!string.IsNullOrEmpty(item.start_date))
                            edu.start_date = DateTime.ParseExact(item.start_date, DATE_FORMAT, null);

                        if (!string.IsNullOrEmpty(item.end_date))
                            edu.end_date = DateTime.ParseExact(item.end_date, DATE_FORMAT, null);
                    }
                    catch
                    {
                        //edu.start_date = DateTime.Now;
                        //edu.end_date = DateTime.Now;
                    }

                    edu.status = Utils.ConvertToIntFromQuest(item.status);

                    info.edu_history.Add(edu);
                }
            }

            if (model.certification.HasData())
            {
                info.certification = new List<IdentityJobSeekerCertificate>();
                foreach (var item in model.certification)
                {
                    if (item.id > 0)
                        continue;

                    var cer = new IdentityJobSeekerCertificate();

                    cer.id = Utils.ConvertToInt32(item.id);
                    cer.name = item.name;
                    cer.point = item.point;
                    cer.pass = Utils.ConvertToIntFromQuest(item.pass);

                    try
                    {
                        if (!string.IsNullOrEmpty(item.start_date))
                            cer.start_date = DateTime.ParseExact(item.start_date, DATE_FORMAT, null);
                    }
                    catch
                    {
                       
                    }

                    try
                    {
                        if (!string.IsNullOrEmpty(item.end_date))
                            cer.end_date = DateTime.ParseExact(item.end_date, DATE_FORMAT, null);
                    }
                    catch
                    {
                       
                    }

                    info.certification.Add(cer);
                }
            }

            if (model.address != null)
            {
                info.address = new IdentityJobSeekerAddress();
                info.address.country_id = Utils.ConvertToIntFromQuest(model.address.country_id);
                info.address.region_id = Utils.ConvertToIntFromQuest(model.address.region_id);
                info.address.prefecture_id = Utils.ConvertToIntFromQuest(model.address.prefecture_id);
                info.address.city_id = Utils.ConvertToIntFromQuest(model.address.city_id);
                info.address.detail = model.address.detail;
                info.address.furigana = model.address.furigana;
                info.address.postal_code = model.address.postal_code;
                info.address.train_line_id = Utils.ConvertToIntFromQuest(model.address.train_line_id);
                info.address.station_id = Utils.ConvertToIntFromQuest(model.address.station_id);
            }

            if (model.address_contact != null)
            {
                info.address_contact = new IdentityJobSeekerAddress();
                info.address_contact.country_id = Utils.ConvertToIntFromQuest(model.address_contact.country_id);
                info.address_contact.region_id = Utils.ConvertToIntFromQuest(model.address_contact.region_id);
                info.address_contact.prefecture_id = Utils.ConvertToIntFromQuest(model.address_contact.prefecture_id);
                info.address_contact.city_id = Utils.ConvertToIntFromQuest(model.address_contact.city_id);
                info.address_contact.detail = model.address_contact.detail;
                info.address_contact.furigana = model.address_contact.furigana;
                info.address_contact.postal_code = model.address_contact.postal_code;
                info.address_contact.is_contact_address = true;
                info.address.train_line_id = Utils.ConvertToIntFromQuest(model.address.train_line_id);
                info.address.station_id = Utils.ConvertToIntFromQuest(model.address.station_id);
            }

            return info;
        }

        private string UploadFromHttpRequest(int company_id)
        {
            var httpRequest = HttpContext.Current.Request;
            var fileName = string.Empty;
            var filePath = string.Empty;
            //Upload file
            var uploadModel = new ApiUploadFileModel();
            uploadModel.Files.Add(httpRequest.Files.Get(0));
            uploadModel.SubDir = "Companies";
            uploadModel.ObjectId = company_id.ToString();
            uploadModel.InCludeDatePath = false;

            if (uploadModel.Files != null && uploadModel.Files[0] != null)
            {
                var apiResult = CdnServices.UploadImagesAsync(uploadModel).Result;
                if (apiResult != null)
                {
                    if (apiResult.Code == EnumCommonCode.Success)
                    {
                        var imagesList = JsonConvert.DeserializeObject<List<string>>(apiResult.Data.ToString());
                        if (imagesList != null && imagesList.Count > 0)
                        {
                            filePath = imagesList[0];
                        }
                    }

                    fileName = uploadModel.Files[0].FileName;
                }
                else
                {
                    logger.Error("Failed to get Upload image because: The CDN Api return null response");
                }
            }
            else
            {
                logger.Error("Failed to get Upload image because: The image is null");
            }

            return filePath;
        }

        private List<FileUploadResponseModel> UploadImage(IdentityCv info = null)
        {
            List<FileUploadResponseModel> returnUploaded = null;
            var httpRequest = HttpContext.Current.Request;
            var fileName = string.Empty;
            var filePath = string.Empty;
            //Upload file
            var uploadModel = new ApiUploadFileModel();
            //uploadModel.FilesInString.Add(model.image);
            uploadModel.SubDir = "CVs/appStorage";

            var cv_id = Utils.ConvertToInt32(httpRequest["cv_id"]);
            var job_seeker_id = Utils.ConvertToInt32(httpRequest["job_seeker_id"]);
            if (cv_id > 0)
            {
                uploadModel.ObjectId = cv_id.ToString();

                if (!string.IsNullOrEmpty(info.image))
                {
                    //Delete old file
                    var apiDeleteModel = new FilesDeleteModel();
                    apiDeleteModel.FilesPath = new List<string>();
                    apiDeleteModel.FilesPath.Add(info.image);

                    var deleteResult = CdnServices.DeleteImagesAsync(apiDeleteModel).Result;
                }
            }
            else
            {
                uploadModel.SubDir = "CVs/svStorage";
                uploadModel.ObjectId = job_seeker_id.ToString();
            }

            uploadModel.InCludeDatePath = false;

            var files = httpRequest.Files;
            if (files != null && files.Count > 0)
            {
                uploadModel.Files.Add(files.Get(0));
            }

            if (uploadModel.Files.HasData())
            {
                var apiResult = CdnServices.UploadImagesAsync(uploadModel).Result;
                if (apiResult != null)
                {
                    if (apiResult.Code == EnumCommonCode.Success)
                    {
                        returnUploaded = JsonConvert.DeserializeObject<List<FileUploadResponseModel>>(apiResult.Data.ToString());
                        if (returnUploaded.HasData())
                        {
                            foreach (var item in returnUploaded)
                            {
                                item.FullPath = CdnHelper.SocialGetFullImgPath(item.Path);
                            }
                        }
                    }
                }
                else
                {
                    logger.Error("Failed to get Upload image because: The CDN Api return null response");
                }
            }
            else
            {
                logger.Error("Failed to get Upload image because: The image is null");
            }

            return returnUploaded;
        }

        #endregion
    }
}
