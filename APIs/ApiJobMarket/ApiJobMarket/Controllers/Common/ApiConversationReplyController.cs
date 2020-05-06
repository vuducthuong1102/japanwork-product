//using System;
//using System.Net;
//using System.Web.Http;
//using System.Threading.Tasks;
//using Newtonsoft.Json;
//using System.Collections.Generic;
//using System.Linq;
//using Autofac;
//using ApiJobMarket.DB.Sql.Stores;
//using ApiJobMarket.Logging;
//using ApiJobMarket.Models;
//using ApiJobMarket.Helpers.Validation;
//using ApiJobMarket.Helpers;
//using ApiJobMarket.DB.Sql.Entities;
//using ApiJobMarket.Services;
//using ApiJobMarket.Helpers.Resource;
//using ApiJobMarket.ActionResults;
//using System.Dynamic;
//using ApiJobMarket.ShareLibs;
//using ApiJobMarket.Settings;

//namespace ApiJobMarket.Controllers
//{
//    [Authorize]
//    [RoutePrefix("api/conversationreply")]
//    public class ApiConversationReplyController : BaseApiController
//    {
//        private readonly ILog logger = LogProvider.For<ApiConversationReplyController>();
//        private IStoreConversationReply _storeConversationReply;

//        public ApiConversationReplyController()
//        {
//            _storeConversationReply = GlobalContainer.IocContainer.Resolve<IStoreConversationReply>();
//        }

//        [Route("getbypage")]
//        [HttpPost]
//        public async Task<IHttpActionResult> GetByPage(ApiConversationFilterModel model)
//        {
//            CreateDocumentApi(model);
//            var returnModel = new ResponseApiModel();
//            var jsonString = string.Empty;
//            var currentDomain = Request.RequestUri.Host;
//            if (!ModelState.IsValid)
//            {
//                return ModelValidation.ApiValidate(ModelState, returnModel);
//            }

//            try
//            {
//                logger.Debug("Begin ConversationReply_GetByPage request");

//                //Check and register from database
//                var returnCode = EnumCommonCode.Success;

//                var filter = ParseFilterIdentity(model);
//                var result = _storeConversationReply.GetByPage(filter);

//                result = GetProfileUser(result, model);
//                await Task.FromResult(result);

//                returnModel.Data = result;
//                returnModel.Code = EnumCommonCode.Success;
//                returnModel.Msg = ApiResourceHelper.GetMessageByCode(returnCode);

//                jsonString = JsonConvert.SerializeObject(returnModel);
//                logger.DebugFormat("ConversationReply_GetByPage Returned Model encrypted = {0}", jsonString);
//            }
//            catch (Exception ex)
//            {
//                string strError = "Failed for ConversationReply_GetByPage request: " + ex.Message;
//                logger.ErrorException(strError, ex);
//                returnModel.Code = EnumCommonCode.Error;
//                returnModel.Msg = strError;

//                jsonString = JsonConvert.SerializeObject(returnModel);
//                return new JsonActionResult(HttpStatusCode.BadRequest, jsonString);
//            }
//            finally
//            {
//                logger.Debug("Ended ConversationReply_GetByPage request");
//            }

//            return new JsonActionResult(HttpStatusCode.OK, jsonString);
//        }

//        private List<IdentityConversationReply> GetProfileUser(List<IdentityConversationReply> listConversationReplys, ApiConversationFilterModel model)
//        {
//            if (listConversationReplys != null && listConversationReplys.Count > 0)
//            {
//                var listId = new List<int>();
//                listId.Add(model.OwnerId);
//                listId.Add(model.UserTwo);
//                if (listId.Count > 0)
//                {
//                    var apiUserInfoModel = new ApiListUserInfoModel();
//                    apiUserInfoModel.ListUserId = listId;

//                    var listUserReturned = AccountServices.GetListUserProfileAsync(apiUserInfoModel).Result;
//                    List<IdentityUser> listOwners = null;
//                    if (listUserReturned.Data != null)
//                    {
//                        listOwners = JsonConvert.DeserializeObject<List<IdentityUser>>(listUserReturned.Data.ToString());
//                    }

//                    if (listOwners != null && listOwners.Count > 0)
//                    {
//                        var userOne = listOwners.FirstOrDefault(s => s.Id == model.OwnerId);
//                        if (userOne != null)
//                        {
//                            listConversationReplys[0].UserOneInfo = GetBaseOnlineUserInfo(userOne);
//                        }
//                        var userTwo = listOwners.FirstOrDefault(s => s.Id == model.UserTwo);
//                        if (userTwo != null)
//                        {
//                            listConversationReplys[0].UserTwoInfo = GetBaseOnlineUserInfo(userTwo);
//                        }
//                    }
//                }
//            }
//            return listConversationReplys;
//        }

//        private dynamic ParseFilterIdentity(ApiConversationFilterModel model)
//        {
//            dynamic filter = new ExpandoObject();
//            filter.Keyword = model.keyword;
           
//            filter.OwnerId = Utils.ConvertToIntFromQuest(model.OwnerId);
//            filter.UserTwo = Utils.ConvertToIntFromQuest(model.UserTwo);
//            var pageSize = Utils.ConvertToIntFromQuest(model.page_size);
//            var pageIndex = Utils.ConvertToIntFromQuest(model.page_index);

//            if (pageIndex == 0)
//                pageIndex = 1;

//            if (pageSize == 0 || pageSize > SystemSettings.DefaultPageSize)
//                pageSize = SystemSettings.DefaultPageSize;

//            filter.PageSize = pageSize;
//            filter.PageIndex = pageIndex;

//            return filter;
//        }

//        [Route("add")]
//        [HttpPost]
//        public async Task<IHttpActionResult> Add(ApiConversationReplyModel model)
//        {
//            CreateDocumentApi(model);
//            var returnModel = new ResponseApiModel();
//            var jsonString = string.Empty;
//            var currentDomain = Request.RequestUri.Host;

//            if (!ModelState.IsValid)
//            {
//                return ModelValidation.ApiValidate(ModelState, returnModel);
//            }

//            try
//            {
//                logger.Debug("Begin Conversation reply add request");
                
//                var record = new IdentityConversationReply
//                {
//                    ConversationId = model.ConversationId,
//                    Content = model.Content,
//                    UserId = model.UserId,
//                    Type = model.Type,
//                    Ip = model.Ip,
//                    Status = model.Status
//                };

//                //Check and register from database
//                var result = _storeConversationReply.Insert(record);

//                await Task.FromResult(result);

//                record.Id = result;
//                record.CreatedDate = DateTime.Now;

//                var apiUserInfoModel = new ApiListUserInfoModel();
//                apiUserInfoModel.ListUserId.Add(model.UserId);

//                var listUserReturned = AccountServices.GetListUserProfileAsync(apiUserInfoModel).Result;
//                List<IdentityUser> listOwners = null;
//                if (listUserReturned.Data != null)
//                {
//                    listOwners = JsonConvert.DeserializeObject<List<IdentityUser>>(listUserReturned.Data.ToString());
//                    if(listOwners != null && listOwners.Count > 0)
//                    {
//                        //Sender
//                        record.UserOneInfo = GetBaseOnlineUserInfo(listOwners[0]);
//                    }
//                }
//                //if (Code == EnumCommonCode.Success)
//                //{
//                //    //Send message
//                //    NotificationHelper.CommentReplyPostAction(model.UserId, model.PostId, model.OwnerId, model.Content);
//                //}

//                returnModel.Code = EnumCommonCode.Success;
//                returnModel.Msg = ApiResourceHelper.GetMessageByCode(returnModel.Code);
//                returnModel.Data = record;

//                jsonString = JsonConvert.SerializeObject(returnModel);

//                logger.DebugFormat("Conversation reply add Returned Model encrypted = {0}", jsonString);
//            }
//            catch (Exception ex)
//            {
//                string strError = "Failed for Conversation reply add request: " + ex.Message;
//                logger.ErrorException(strError, ex);
//                returnModel.Code = EnumCommonCode.Error;
//                returnModel.Msg = strError;

//                jsonString = JsonConvert.SerializeObject(returnModel);
//                return new JsonActionResult(HttpStatusCode.BadRequest, jsonString);
//            }
//            finally
//            {
//                logger.Debug("Ended Conversation reply add request");
//            }

//            return new JsonActionResult(HttpStatusCode.OK, jsonString);
//        }

//        [Route("delete")]
//        [HttpPost]
//        public async Task<IHttpActionResult> Delete(ApiConversationReplyModel model)
//        {
//            CreateDocumentApi(model);
//            var returnModel = new ResponseApiModel();
//            var jsonString = string.Empty;
//            var currentDomain = Request.RequestUri.Host;

//            if (!ModelState.IsValid)
//            {
//                return ModelValidation.ApiValidate(ModelState, returnModel);
//            }

//            try
//            {
//                logger.Debug("Begin Delete conversation reply request");

//                var identityComment = new IdentityConversationReply()
//                {
//                    Id = model.Id,
//                    UserId = model.UserId
//                };
              
//                var result = _storeConversationReply.Delete(identityComment);

//                await Task.FromResult(result);

//                returnModel.Code = EnumCommonCode.Success;
//                returnModel.Msg = ApiResourceHelper.GetMessageByCode(returnModel.Code);

//                jsonString = JsonConvert.SerializeObject(returnModel);

//                logger.DebugFormat("Delete conversation reply Returned Model encrypted = {0}", jsonString);
//            }
//            catch (Exception ex)
//            {
//                string strError = "Failed for Delete conversation reply request: " + ex.Message;
//                logger.ErrorException(strError, ex);
//                returnModel.Code = EnumCommonCode.Error;
//                returnModel.Msg = strError;

//                jsonString = JsonConvert.SerializeObject(returnModel);
//                return new JsonActionResult(HttpStatusCode.BadRequest, jsonString);
//            }
//            finally
//            {
//                logger.Debug("Ended Delete conversation reply request");
//            }

//            return new JsonActionResult(HttpStatusCode.OK, jsonString);
//        }       
//    }
//}
