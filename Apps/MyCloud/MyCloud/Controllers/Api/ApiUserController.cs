//using MyCloud.ActionResults;
//using MyCloud.Helpers;
//using MyCloud.Models;
//using MyCloud.SharedLib;
//using MyCloud.SharedLib.Logging;
//using MyCloud.SharedLibs;
//using Newtonsoft.Json;
//using System;
//using System.Linq;
//using System.Net;
//using System.Web.Http;

//namespace MyCloud.Controllers
//{
//    [RoutePrefix("api/user")]
//    public class ApiUserController : BaseApiController
//    {
//        private readonly ILog logger = LogProvider.For<ApiUserController>();

//        [Route("checklistonline")]
//        [HttpPost]
//        public IHttpActionResult CheckListOnline(CheckUserOnlineModel model)
//        {
//            var returnModel = new ApiResponseCommonModel();
//            var jsonString = string.Empty;
//            var currentDomain = Request.RequestUri.Host;

//            try
//            {
//                logger.Debug("Begin CheckListOnline request");
//                var checkList = new UserOnlineCheckListModel();
//                if (!string.IsNullOrEmpty(model.ListUsers))
//                {
//                    var listIds = model.ListUsers.Split(',');
//                    var listUser = MessengerHelpers.GetAllUsersFromCache();
//                    if (listIds != null && listIds.Length > 0)
//                    {
//                        foreach (var id in listIds)
//                        {
//                            var intId = Utils.ConvertToInt32(id);
//                            var user = new UserOnlineInfoModel();
//                            user.UserId = intId;

//                            var currentUser = listUser.Where(x => x.UserId == intId).FirstOrDefault();
//                            if (currentUser != null)
//                            {
//                                user.IsOnline = true;
//                            }
//                            else
//                            {
//                                user.IsOnline = false;
//                            }

//                            checkList.UsersList.Add(user);
//                        }
//                    }
//                }

//                returnModel.Data = checkList;
//                returnModel.Code = EnumCommonCode.Success;

//                jsonString = JsonConvert.SerializeObject(returnModel);
//                logger.DebugFormat("CheckListOnline Returned Model encrypted = {0}", jsonString);
//            }
//            catch (Exception ex)
//            {
//                string strError = "Failed for CheckListOnline request: " + ex.Message;
//                logger.ErrorException(strError, ex);
//                returnModel.Code = EnumCommonCode.Error;
//                returnModel.Msg = strError;

//                jsonString = JsonConvert.SerializeObject(returnModel);
//                return new JsonActionResult(HttpStatusCode.BadRequest, jsonString);
//            }
//            finally
//            {
//                logger.Debug("Ended CheckListOnline request");
//            }

//            return new JsonActionResult(HttpStatusCode.OK, jsonString);
//        }       
//    }
//}