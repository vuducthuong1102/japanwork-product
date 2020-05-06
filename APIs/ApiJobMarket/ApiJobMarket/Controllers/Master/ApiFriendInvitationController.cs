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
using ApiJobMarket.Resources;
using ApiJobMarket.ShareLibs;
using System.Dynamic;
using System.Web;
using ApiJobMarket.DB.Sql.Entities;
using System.Text.RegularExpressions;
using ApiJobMarket.Settings;

namespace ApiJobMarket.Controllers
{
    [Authorize]
    [RoutePrefix("api/job_seekers")]
    public class ApiFriendInvitationController : BaseApiController
    {
        private readonly ILog logger = LogProvider.For<ApiFriendInvitationController>();

        //[HttpPost]
        //[Route("accepts")]
        //public async Task<IHttpActionResult> AcceptFriendInvitation(ApiFriendInvitationActionModel model)
        //{
        //    CreateDocumentApi(model);
        //    var requestName = "FriendInvitations-AcceptFriendInvitation";
        //    var returnModel = new ApiResponseCommonModel();
        //    var jsonString = string.Empty;
        //    var logoPath = string.Empty;

        //    if (!ModelState.IsValid)
        //    {
        //        return ModelValidation.ApiValidate(ModelState, returnModel);
        //    }

        //    try
        //    {
        //        //logger.Debug(string.Format("Begin {0} request", requestName));

        //        var returnCode = EnumCommonCode.Success;               
        //        var storeFriendInvitation = GlobalContainer.IocContainer.Resolve<IStoreFriendInvitation>();

        //        var job_id = Utils.ConvertToIntFromQuest(model.job_id);
        //        dynamic appData = new ExpandoObject();
        //        appData.id = Utils.ConvertToIntFromQuest(model.id);
        //        appData.job_id = job_id;
        //        appData.job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);

        //        var info = JobHelpers.GetBaseInfoJob(job_id, GetCurrentRequestLang());
        //        if (info == null)
        //        {
        //            returnModel.error.error_code = EnumErrorCode.E050101.ToString();
        //            returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E050101);

        //            return CachErrorResult(returnModel);
        //        }

        //        var status = storeFriendInvitation.Accept(appData);
        //        await Task.FromResult(status);

        //        returnModel.value = status;
        //        returnModel.message = UserApiResource.SUCCESS_INVITATION_ACCEPTED;
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

        //[HttpPost]
        //[Route("ignorances")]
        //public async Task<IHttpActionResult> IgnoreFriendInvitation(ApiFriendInvitationActionModel model)
        //{
        //    CreateDocumentApi(model);
        //    var requestName = "FriendInvitations-IgnoreFriendInvitation";
        //    var returnModel = new ApiResponseCommonModel();
        //    var jsonString = string.Empty;
        //    var logoPath = string.Empty;

        //    if (!ModelState.IsValid)
        //    {
        //        return ModelValidation.ApiValidate(ModelState, returnModel);
        //    }

        //    try
        //    {
        //        //logger.Debug(string.Format("Begin {0} request", requestName));

        //        var returnCode = EnumCommonCode.Success;
        //        var storeFriendInvitation = GlobalContainer.IocContainer.Resolve<IStoreFriendInvitation>();

        //        var job_id = Utils.ConvertToIntFromQuest(model.job_id);
        //        dynamic appData = new ExpandoObject();
        //        appData.id = Utils.ConvertToIntFromQuest(model.id);
        //        appData.job_id = job_id;
        //        appData.job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);

        //        var info = JobHelpers.GetBaseInfoJob(job_id, GetCurrentRequestLang());
        //        if (info == null)
        //        {
        //            returnModel.error.error_code = EnumErrorCode.E050101.ToString();
        //            returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E050101);

        //            return CachErrorResult(returnModel);
        //        }                

        //        var status = storeFriendInvitation.Ignore(appData);
        //        await Task.FromResult(status);

        //        returnModel.value = status;
        //        returnModel.message = UserApiResource.SUCCESS_INVITATION_IGNORED;

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

        [HttpGet]
        [Route("{id:int}/invited_friends")]
        public async Task<IHttpActionResult> GetInvitedFriendsByPage(int id)
        {
            var requestName = "Job_Seekers-GetInvitedFriendsByPage";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var apiFilter = GetFilterConfig();
                dynamic filter = new ExpandoObject();

                var httpRequest = HttpContext.Current.Request;
                filter.keyword = apiFilter.keyword;
                filter.status = apiFilter.status;
                filter.job_seeker_id = id;

                var myStore = GlobalContainer.IocContainer.Resolve<IStoreFriendInvitation>();

                List<IdentityFriendInvitation> listData = myStore.GetListByJobSeeker(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);
                await Task.FromResult(listData);

                if (listData.HasData())
                {
                    returnModel.total = listData[0].total_count;
                    foreach (var item in listData)
                    {
                        if (item.job_id > 0)
                        {
                            item.job_info = JobHelpers.GetBaseInfoJob(item.job_id, GetCurrentRequestLang());
                            foreach (var enm in Enum.GetValues(typeof(EnumApplicationStatus)))
                            {
                                if (item.status == (int)enm)
                                    item.status_label = EnumExtensions.GetEnumDescription((Enum)enm);
                            }
                        }
                    }
                }

                returnModel.value = listData;
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

        [HttpGet]
        [Route("{id:int}/receivers")]
        public async Task<IHttpActionResult> GetReceivers(int id)
        {
            var requestName = "Agencies-GetFriendInvitationsByPage";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));

                var apiFilter = GetFilterConfig();
                dynamic filter = new ExpandoObject();

                var httpRequest = HttpContext.Current.Request;
                filter.keyword = apiFilter.keyword;
                filter.status = apiFilter.status;
                filter.agency_id = Utils.ConvertToInt32(httpRequest["agency_id"]); ;
                filter.invite_id = id;
                filter.staff_id = Utils.ConvertToInt32(httpRequest["staff_id"]);
                filter.job_id = Utils.ConvertToInt32(httpRequest["job_id"]);

                filter.language_code = GetCurrentRequestLang();

                var myStore = GlobalContainer.IocContainer.Resolve<IStoreFriendInvitation>();

                List<IdentityFriendInvitation> listData = myStore.GetReceivers(filter, apiFilter.page_index.Value, apiFilter.page_size.Value);
                await Task.FromResult(listData);

                if (listData.HasData())
                {
                    returnModel.total = listData[0].total_count;
                    foreach (var item in listData)
                    {
                        item.job_info = JobHelpers.GetBaseInfoJob(item.job_id, filter.language_code);
                    }
                }

                returnModel.value = listData;
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

        [HttpPost]
        [Route("application_invite")]
        public async Task<IHttpActionResult> ApplicationInvite(ApiFriendInvitationModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Job_Seekers-ApplicationInvite";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            var logoPath = string.Empty;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var returnCode = EnumCommonCode.Success;
                var job_id = Utils.ConvertToIntFromQuest(model.job_id);
                var job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);

                var myStore = GlobalContainer.IocContainer.Resolve<IStoreFriendInvitation>();
                var jskStore = GlobalContainer.IocContainer.Resolve<IStoreJobSeeker>();

                var identity = new IdentityFriendInvitation();
                identity.Invitations = new List<IdentityFriendInvitation>();
                identity.job_id = job_id;
                identity.sender_id = job_seeker_id;
                List<string> emails = new List<string>();

                if (model.Emails.HasData())
                {
                    foreach (var item in model.Emails)
                    {
                        if (!IsEmail(item))
                            continue;

                        var itemInfo = new IdentityFriendInvitation();
                        itemInfo.sender_id = job_seeker_id;
                        itemInfo.job_id = job_id;
                        itemInfo.receiver_email = item;

                        emails.Add(item);

                        identity.Invitations.Add(itemInfo);
                    }
                }

                identity.note = model.note;

                var job_info = JobHelpers.GetBaseInfoJob(job_id, GetCurrentRequestLang());
                if (job_info == null)
                {
                    //Job not found
                    returnModel.error.error_code = EnumErrorCode.E050101.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E050101);

                    return CachErrorResult(returnModel);
                }

                var userInfo = JobSeekerHelpers.GetBaseInfo(job_seeker_id);
                var displayName = string.Empty;
                var jobSeekerEmail = string.Empty;
                if (userInfo == null)
                {
                    //JobSeeker not found
                    returnModel.error.error_code = EnumErrorCode.E000105.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E000105);

                    return CachErrorResult(returnModel);
                }
                else
                {
                    displayName = userInfo.fullname;
                    jobSeekerEmail = userInfo.email;

                    if (string.IsNullOrEmpty(displayName))
                        displayName = userInfo.fullname_furigana;

                    if (string.IsNullOrEmpty(displayName))
                        displayName = userInfo.display_name;

                }

                if (!identity.Invitations.HasData())
                {
                    //Data invalid
                    returnModel.error.error_code = EnumErrorCode.E000107.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E000107);

                    return CachErrorResult(returnModel);
                }

                if (emails.HasData())
                {
                    if(emails.Count > 1)
                    {
                        //Data invalid
                        returnModel.error.error_code = EnumCommonCode.Error.ToString();
                        returnModel.error.message = EmailResource.ERROR_JUST_SEND_ONLY_ONE_EMAIL;

                        return CachErrorResult(returnModel);
                    }
                    
                    //if(emails.Count == 1)
                    //{
                    //    var existedJobSk = jskStore.GetByEmail(emails[0]);
                    //    if (existedJobSk != null)
                    //    {
                    //        //Data invalid
                    //        returnModel.error.error_code = EnumCommonCode.Error.ToString();
                    //        returnModel.error.message = EmailResource.ERROR_EMAIL_USER_ALREADY_USED_SYSTEM;

                    //        return CachErrorResult(returnModel);
                    //    }
                    //}
                }                

                var result = myStore.Invite(identity);

                await Task.FromResult(result);

                if (result > 0 && emails.HasData())
                {                 
                    var hashingData = GenereateHashingForInvitation(job_id, job_seeker_id, result, emails[0]);

                    //Send email
                    SendEmailForInviteApplication(result, emails, hashingData, jobSeekerEmail, displayName, model.note, job_info);
                }

                returnModel.value = result;
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

        [HttpPost]
        [Route("using_system_invite")]
        public async Task<IHttpActionResult> UsingSystemInvite(ApiFriendInvitationModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Job_Seekers-UsingSystemInvite";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            var logoPath = string.Empty;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var returnCode = EnumCommonCode.Success;
                var job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);

                var myStore = GlobalContainer.IocContainer.Resolve<IStoreFriendInvitation>();
                var jskStore = GlobalContainer.IocContainer.Resolve<IStoreJobSeeker>();

                var identity = new IdentityFriendInvitation();
                identity.Invitations = new List<IdentityFriendInvitation>();
                identity.sender_id = job_seeker_id;
                List<string> emails = new List<string>();

                var userInfo = JobSeekerHelpers.GetBaseInfo(job_seeker_id);
                var displayName = string.Empty;
                var jobSeekerEmail = string.Empty;
                if (userInfo == null)
                {
                    //JobSeeker not found
                    returnModel.error.error_code = EnumErrorCode.E000105.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E000105);

                    return CachErrorResult(returnModel);
                }
                else
                {
                    displayName = userInfo.fullname;
                    jobSeekerEmail = userInfo.email;

                    if (string.IsNullOrEmpty(displayName))
                        displayName = userInfo.fullname_furigana;

                    if (string.IsNullOrEmpty(displayName))
                        displayName = userInfo.display_name;
                }

                if (model.Emails.HasData())
                {
                    foreach (var item in model.Emails)
                    {
                        if (!IsEmail(item))
                            continue;

                        var itemInfo = new IdentityFriendInvitation();
                        itemInfo.sender_id = job_seeker_id;                     
                        itemInfo.receiver_email = item;

                        emails.Add(item);

                        identity.Invitations.Add(itemInfo);
                    }
                }

                identity.note = model.note;

                if(!identity.Invitations.HasData())
                {
                    //Data invalid
                    returnModel.error.error_code = EnumErrorCode.E000107.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E000107);

                    return CachErrorResult(returnModel);
                }

                if (emails.HasData())
                {
                    if (emails.Count > 1)
                    {
                        //Data invalid
                        returnModel.error.error_code = EnumCommonCode.Error.ToString();
                        returnModel.error.message = EmailResource.ERROR_JUST_SEND_ONLY_ONE_EMAIL;

                        return CachErrorResult(returnModel);
                    }

                    if (emails.Count == 1)
                    {
                        var existedJobSk = jskStore.GetByEmail(emails[0]);
                        if (existedJobSk != null)
                        {
                            //Data invalid
                            returnModel.error.error_code = EnumCommonCode.Error.ToString();
                            returnModel.error.message = EmailResource.ERROR_EMAIL_USER_ALREADY_USED_SYSTEM;

                            return CachErrorResult(returnModel);
                        }
                    }
                }

                var result = myStore.Invite(identity);

                await Task.FromResult(result);

                if (result > 0 && emails.HasData())
                {                 

                    var hashingData = GenereateHashingForInvitation(0, job_seeker_id, result, emails[0]);

                    //Send email
                    SendEmailForUsingSystem(result, emails, hashingData, jobSeekerEmail, displayName, model.note);
                }

                returnModel.value = result;
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

        [HttpPost]
        [Route("invitation_accept")]
        public async Task<IHttpActionResult> InvitationAccept(ApiFriendInvitationAcceptModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Job_Seekers-InvitationAccept";
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
                var myStore = GlobalContainer.IocContainer.Resolve<IStoreFriendInvitation>();

                var identity = new IdentityFriendInvitation();

                identity.invite_id = Utils.ConvertToIntFromQuest(model.invite_id);
                identity.job_id = Utils.ConvertToIntFromQuest(model.job_id);
                identity.sender_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);
                identity.receiver_email = model.email;                
              
                var result = myStore.Accept(identity);

                await Task.FromResult(result);

                returnModel.value = identity;
                returnModel.status = returnCode;

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

        [HttpPost]
        [Route("invitation_resend")]
        public async Task<IHttpActionResult> ApplicationInviteResend(ApiFriendInvitationModel model)
        {
            CreateDocumentApi(model);
            var requestName = "Job_Seekers-ApplicationInviteResend";
            var returnModel = new ApiResponseCommonModel();
            var jsonString = string.Empty;
            var logoPath = string.Empty;

            if (!ModelState.IsValid)
            {
                return ModelValidation.ApiValidate(ModelState, returnModel);
            }

            try
            {
                //logger.Debug(string.Format("Begin {0} request", requestName));
                var returnCode = EnumCommonCode.Success;
                var job_id = Utils.ConvertToIntFromQuest(model.job_id);
                var job_seeker_id = Utils.ConvertToIntFromQuest(model.job_seeker_id);

                var myStore = GlobalContainer.IocContainer.Resolve<IStoreFriendInvitation>();
                var jskStore = GlobalContainer.IocContainer.Resolve<IStoreJobSeeker>();

                var identity = new IdentityFriendInvitation();
                identity.Invitations = new List<IdentityFriendInvitation>();
                identity.job_id = job_id;
                identity.sender_id = job_seeker_id;
                List<string> emails = new List<string>();

                if (model.Emails.HasData())
                {
                    foreach (var item in model.Emails)
                    {
                        if (!IsEmail(item))
                            continue;

                        var itemInfo = new IdentityFriendInvitation();
                        itemInfo.sender_id = job_seeker_id;
                        itemInfo.job_id = job_id;
                        itemInfo.receiver_email = item;

                        emails.Add(item);

                        identity.Invitations.Add(itemInfo);
                    }
                }

                identity.note = model.note;

                IdentityJob jobInfo = null;
                if (job_id > 0)
                {
                    jobInfo = JobHelpers.GetBaseInfoJob(job_id, GetCurrentRequestLang());
                    if (jobInfo == null)
                    {
                        //Job not found
                        returnModel.error.error_code = EnumErrorCode.E050101.ToString();
                        returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E050101);

                        return CachErrorResult(returnModel);
                    }
                }

                var userInfo = JobSeekerHelpers.GetBaseInfo(job_seeker_id);
                var displayName = string.Empty;
                var jobSeekerEmail = string.Empty;
                if (userInfo == null)
                {
                    //JobSeeker not found
                    returnModel.error.error_code = EnumErrorCode.E000105.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E000105);

                    return CachErrorResult(returnModel);
                }
                else
                {
                    displayName = userInfo.fullname;
                    jobSeekerEmail = userInfo.email;

                    if (string.IsNullOrEmpty(displayName))
                        displayName = userInfo.fullname_furigana;

                    if (string.IsNullOrEmpty(displayName))
                        displayName = userInfo.display_name;

                }

                if (!identity.Invitations.HasData())
                {
                    //Data invalid
                    returnModel.error.error_code = EnumErrorCode.E000107.ToString();
                    returnModel.error.message = EnumExtensions.GetEnumDescription((Enum)EnumErrorCode.E000107);

                    return CachErrorResult(returnModel);
                }

                if (emails.HasData())
                {
                    if (emails.Count > 1)
                    {
                        //Data invalid
                        returnModel.error.error_code = EnumCommonCode.Error.ToString();
                        returnModel.error.message = EmailResource.ERROR_JUST_SEND_ONLY_ONE_EMAIL;

                        return CachErrorResult(returnModel);
                    }

                    if (emails.Count == 1)
                    {
                        if(job_id <= 0)
                        {
                            var existedJobSk = jskStore.GetByEmail(emails[0]);
                            if (existedJobSk != null)
                            {
                                //Data invalid
                                returnModel.error.error_code = EnumCommonCode.Error.ToString();
                                returnModel.error.message = EmailResource.ERROR_EMAIL_USER_ALREADY_USED_SYSTEM;

                                return CachErrorResult(returnModel);
                            }
                        }
                    }
                }

                var result = myStore.Invite(identity);

                await Task.FromResult(result);

                if (result > 0 && emails.HasData())
                {

                    var hashingData = GenereateHashingForInvitation(job_id, job_seeker_id, result, emails[0]);

                    if(job_id > 0)
                    {
                        //Send email
                        SendEmailForInviteApplication(result, emails, hashingData, jobSeekerEmail, displayName, model.note, jobInfo);
                    }
                    else
                    {
                        //Send email
                        SendEmailForUsingSystem(result, emails, hashingData, jobSeekerEmail, displayName, model.note);
                    }                   
                }

                returnModel.value = result;
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


        protected int SendEmailForInviteApplication(int inviteId, List<string> emails, string hashingData, string senderEmail, string senderName, string note, dynamic data)
        {
            var result = EnumCommonCode.Success;
            //Begin sending email
            var emailModel = new EmailModel
            {
                Sender = SystemSettings.EmailSender,
                SenderPwd = SystemSettings.EmailSenderPwd,
                Receiver = string.Join(",", emails),
                Subject = EmailResource.SUBJECT_APPLICATION_INVITE,
                SenderName = senderName,
                SenderEmail = senderEmail,
                Note = note,
                Data = data
            };

            emailModel.ActiveLink = string.Format("{0}?&token={1}", SystemSettings.Email_AccepFriendInvitationLink, hashingData);

            emailModel.Body = RenderPartialViewAsString("../EmailTemplates/Invitation/JobApplication", emailModel);

            var sendEmailResult = EmailHelper.SendEmail(emailModel);
            //var sendEmailResult = string.Empty;
            if (!string.IsNullOrEmpty(sendEmailResult))
            {
                //Sending email was failed
                logger.Error(sendEmailResult);
                result = EnumCommonCode.Error;
            }

            return result;
        }

        protected int SendEmailForUsingSystem(int inviteId, List<string> emails, string hashingData, string senderEmail, string senderName, string note)
        {
            var result = EnumCommonCode.Success;
            //Begin sending email
            var emailModel = new EmailModel
            {
                Sender = SystemSettings.EmailSender,
                SenderPwd = SystemSettings.EmailSenderPwd,
                Receiver = string.Join(",", emails),
                Subject = EmailResource.SUBJECT_USING_SYSTEM_INVITE,
                SenderName = senderName,
                SenderEmail = senderEmail,
                Note = note
            };

            emailModel.ActiveLink = string.Format("{0}?&token={1}", SystemSettings.Email_AccepFriendInvitationLink, hashingData);

            emailModel.Body = RenderPartialViewAsString("../EmailTemplates/Invitation/UsingSystem", emailModel);

            var sendEmailResult = EmailHelper.SendEmail(emailModel);
            //var sendEmailResult = string.Empty;
            if (!string.IsNullOrEmpty(sendEmailResult))
            {
                //Sending email was failed
                logger.Error(sendEmailResult);
                result = EnumCommonCode.Error;
            }

            return result;
        }

        private static bool IsEmail(string input)
        {
            var regexPatern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" + "@" + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";
            if (!string.IsNullOrEmpty(input))
            {
                return Regex.IsMatch(input, regexPatern);
            }

            return false;
        }
        private string GenereateHashingForInvitation(int job_id, int sender_id, int inviteId, string targetEmail)
        {
            var recoverData = string.Format("{0}|{1}|{2}|{3}", job_id, sender_id, inviteId, targetEmail);

            var dataEncrypt = Utility.EncryptText(recoverData, SystemSettings.EncryptKey);

            var rawData = string.Format("{0}.{1}.{2}", Utility.Md5HashingData(DateTime.Now.ToString(Constant.DATEFORMAT_yyyyMMddHHmmss)), dataEncrypt, Utility.Md5HashingData(sender_id.ToString()));

            return rawData;
        }
    }
}
