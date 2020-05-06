//using System;
//using System.Net;
//using System.Web.Http;
//using System.Threading.Tasks;

//using ApiJobMarket.Logging;
//using ApiJobMarket.Services;
//using ApiJobMarket.ActionResults;
//using ApiJobMarket.Helpers;
//using ApiJobMarket.Models;
//using ApiJobMarket.DB.Sql.Stores;
//using Newtonsoft.Json;
//using ApiJobMarket.Helpers.Resource;
//using ApiJobMarket.Helpers.Validation;
//using System.Collections.Generic;
//using System.Linq;
//using Autofac;
//using ApiJobMarket.DB.Sql.Entities;
//using System.Dynamic;
//using ApiJobMarket.ShareLibs;
//using ApiJobMarket.Settings;

//namespace ApiJobMarket.Controllers
//{
//    [Authorize]
//    [RoutePrefix("api/conversation")]
//    public class ApiConversationController : BaseApiController
//    {
//        private readonly ILog logger = LogProvider.For<ApiConversationController>();
//        private IStoreConversation _storeConversation;
//        private IStoreConversationReply _storeConversationReply;

//        public ApiConversationController()
//        {
//            _storeConversation = GlobalContainer.IocContainer.Resolve<IStoreConversation>();
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
//                logger.Debug("Begin GetByPage request");

//                //Check and register from database
//                var returnCode = EnumCommonCode.Success;
//                var filterIdentity = ParseFilterIdentity(model);
//                var result = _storeConversation.GetByPage(filterIdentity);
//                result = GetProfileUser(result);
//                result = GetFirstByListConversations(result);
//                await Task.FromResult(result);

//                returnModel.Data = result;
//                returnModel.Code = EnumCommonCode.Success;
//                returnModel.Msg = ApiResourceHelper.GetMessageByCode(returnCode);

//                jsonString = JsonConvert.SerializeObject(returnModel);
//                logger.DebugFormat("GetByPage Returned Model encrypted = {0}", jsonString);
//            }
//            catch (Exception ex)
//            {
//                string strError = "Failed for GetByPage request: " + ex.Message;
//                logger.ErrorException(strError, ex);
//                returnModel.Code = EnumCommonCode.Error;
//                returnModel.Msg = strError;

//                jsonString = JsonConvert.SerializeObject(returnModel);
//                return new JsonActionResult(HttpStatusCode.BadRequest, jsonString);
//            }
//            finally
//            {
//                logger.Debug("Ended GetByPage request");
//            }

//            return new JsonActionResult(HttpStatusCode.OK, jsonString);
//        }

//        private List<IdentityConversation> GetProfileUser(List<IdentityConversation> listConversations)
//        {
//            if (listConversations != null && listConversations.Count > 0)
//            {                
//                var listId = new List<int>();
//                foreach (var item in listConversations)
//                {
//                    listId.Add(item.UserOne);
//                    listId.Add(item.UserTwo);
//                }
//                listId = listId.Distinct().ToList();

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
//                        foreach (var item in listConversations)
//                        {
//                            var userOne = listOwners.FirstOrDefault(s => s.Id == item.UserOne);
//                            if (userOne != null)
//                            {
//                                item.UserOneInfo = GetBaseOnlineUserInfo(userOne);
//                            }
//                            var userTwo = listOwners.FirstOrDefault(s => s.Id == item.UserTwo);
//                            if (userTwo != null)
//                            {
//                                item.UserTwoInfo = GetBaseOnlineUserInfo(userTwo);
//                            }
//                        }
//                    }
//                }
//            }
//            return listConversations;
//        }

//        private List<IdentityConversation> GetFirstByListConversations(List<IdentityConversation> listConversations)
//        {


//            if (listConversations != null && listConversations.Count > 0)
//            {
//                var listId = listConversations.Select(s => s.Id).ToList();
           
//                if (listId.Count > 0)
//                {
//                    var listConversationReplys = _storeConversationReply.GetFirstByListId(listId);

//                    if (listConversationReplys != null && listConversationReplys.Count > 0)
//                    {
//                        foreach (var item in listConversations)
//                        {
//                            var identity = listConversationReplys.FirstOrDefault(s => s.ConversationId == item.Id);
//                            if (identity != null)
//                            {
//                                item.FirstConversationReply = identity;
//                            }
//                        }
//                    }
//                }
//            }
//            return listConversations;
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

//        [Route("getcurrent")]
//        [HttpPost]
//        public async Task<IHttpActionResult> GetCurrent(ApiConversationModel model)
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
//                logger.Debug("Begin GetCurrent request");

//                //Check and register from database
//                var result = _storeConversation.GetCurrentConversation(new IdentityConversation
//                {
//                    UserOne = model.UserOne,
//                    UserTwo = model.UserTwo,
//                    Ip = model.Ip,
//                    Status = model.Status
//                });

//                await Task.FromResult(result);

//                //if (Code == EnumCommonCode.Success)
//                //{
//                //    //Send message
//                //    NotificationHelper.CommentPostAction(model.UserId, model.PostId, model.OwnerId, model.Content);
//                //}

//                returnModel.Code = EnumCommonCode.Success;
//                returnModel.Msg = ApiResourceHelper.GetMessageByCode(returnModel.Code);
//                returnModel.Data = result;

//                jsonString = JsonConvert.SerializeObject(returnModel);

//                logger.DebugFormat("GetCurrent Returned Model encrypted = {0}", jsonString);
//            }
//            catch (Exception ex)
//            {
//                string strError = "Failed for GetCurrent request: " + ex.Message;
//                logger.ErrorException(strError, ex);
//                returnModel.Code = EnumCommonCode.Error;
//                returnModel.Msg = strError;

//                jsonString = JsonConvert.SerializeObject(returnModel);
//                return new JsonActionResult(HttpStatusCode.BadRequest, jsonString);
//            }
//            finally
//            {
//                logger.Debug("Ended GetCurrent request");
//            }

//            return new JsonActionResult(HttpStatusCode.OK, jsonString);
//        }

//        [Route("delete")]
//        [HttpPost]
//        public async Task<IHttpActionResult> Delete(ApiConversationModel model)
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
//                logger.Debug("Begin Delete conversation request");

//                var identityComment = new IdentityConversation()
//                {
//                    Id = model.Id,
//                    OwnerId = model.OwnerId
//                };

//                var result = _storeConversation.Delete(identityComment);

//                await Task.FromResult(result);

//                returnModel.Code = result;
//                returnModel.Msg = ApiResourceHelper.GetMessageByCode(returnModel.Code);

//                jsonString = JsonConvert.SerializeObject(returnModel);

//                logger.DebugFormat("Delete conversation Returned Model encrypted = {0}", jsonString);
//            }
//            catch (Exception ex)
//            {
//                string strError = "Failed for Delete conversation request: " + ex.Message;
//                logger.ErrorException(strError, ex);
//                returnModel.Code = EnumCommonCode.Error;
//                returnModel.Msg = strError;

//                jsonString = JsonConvert.SerializeObject(returnModel);
//                return new JsonActionResult(HttpStatusCode.BadRequest, jsonString);
//            }
//            finally
//            {
//                logger.Debug("Ended Delete conversation  request");
//            }

//            return new JsonActionResult(HttpStatusCode.OK, jsonString);
//        }
//    }
//}
