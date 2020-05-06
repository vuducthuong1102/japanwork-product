using System;
using System.Web.Mvc;
using Manager.WebApp.Helpers;
using MsSql.AspNet.Identity;
using Manager.WebApp.Models;
using Manager.SharedLibs.Logging;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Net;
using MsSql.AspNet.Identity.Entities;
using Manager.WebApp.Settings;
using Manager.WebApp.Models.System;
using Manager.WebApp.Resources;
using Manager.SharedLibs;

namespace Manager.WebApp.Controllers
{
    public class FunctionController : BaseAuthedController
    {
        private readonly IAccessRolesStore _identityStore;
        private readonly IActivityStore _activityStore;
        private readonly ILog logger = LogProvider.For<FunctionController>();

        public FunctionController(IAccessRolesStore identityStore, IActivityStore activityStore)
        {
            _identityStore = identityStore;
            _activityStore = activityStore;
        }

        [AccessRoleChecker(AdminRequired = true)]
        public ActionResult Index(FunctionViewModel model)
        {
            ModelState.Clear();
            var strError = string.Empty;
            try
            {
                model.AllAccesses = _identityStore.GetAllAccess();
                model.AllOperations = _identityStore.GetAllOperations();
            }
            catch (Exception ex)
            {
                strError = string.Format("Could not GetAllAccess because: {0}", ex.ToString());
                logger.Error(strError);
                this.AddNotification(strError, NotificationType.ERROR);
            }

            return View(model);
        }

        [HttpGet]
        public JsonResult GetOperationsByControllerName(string ControllerName)
        {
            var myOps = Constant.GetAllControllerOperations().Where(m => m.ControllerName == ControllerName).SingleOrDefault();
            if (myOps != null && myOps.AllOperations != null)
            {
                myOps.AllOperations.OrderBy(m => m.ActionName);
            }

            return Json(new { data = myOps }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AccessRoleChecker(AdminRequired = true)]
        public ActionResult Create(FunctionViewModel model)
        {
            var result = false;
            var strError = string.Empty;
            var identity = new IdentityOperation { AccessId = model.AccessId, ActionName = model.ActionName, OperationName = model.OperationName, IndexOrder = model.IndexOrder };
            try
            {
                var isDuplicated = _identityStore.CheckOperationDuplicate(identity);
                if (isDuplicated)
                {
                    this.AddNotification(string.Format("Could not create function due to the function [{0} of {1}] is existed", model.ActionName, model.AccessName), NotificationType.ERROR);
                    return RedirectToAction("Index", "Function");
                }

                result = _identityStore.CreateOperation(identity);
                if (result)
                {
                    this.AddNotification(string.Format("The function [{0} of {1}] is created succesfully", model.ActionName, model.AccessName), NotificationType.SUCCESS);

                    //Write log
                    var activityText = "Create new function [{0} of {1}]";
                    activityText = string.Format(activityText, model.ActionName, model.AccessName);
                    WriteActivityLog(activityText, ActivityLogType.CreateFunction, model.Id.ToString(), TargetObjectType.Function);

                    return RedirectToAction("Index", "Function");
                }
                else
                {
                    this.AddNotification("Could not create function due to database exception occurred", NotificationType.ERROR);
                }
            }
            catch (Exception ex)
            {
                strError = string.Format("Could not CreateFunction because: {0}", ex.ToString());
                logger.Error(strError);
                this.AddNotification(strError, NotificationType.ERROR);
            }


            return RedirectToAction("Index", "Function");
        }

        [HttpPost]
        [AccessRoleChecker(AdminRequired = true)]
        public ActionResult Update(FunctionViewModel model)
        {
            var result = false;
            if (ModelState.IsValid)
            {
                var strError = string.Empty;
                var identity = new IdentityOperation { Id = model.Id, AccessId = model.AccessId, ActionName = model.ActionName, OperationName = model.OperationName, IndexOrder = model.IndexOrder };
                try
                {
                    var isDuplicated = _identityStore.CheckOperationDuplicate(identity);
                    if (isDuplicated)
                    {
                        this.AddNotification(string.Format("Could not update due to the function [{0} of {1}] is existed", model.ActionName, model.AccessName), NotificationType.ERROR);
                        return RedirectToAction("Index", "Function");
                    }

                    result = _identityStore.UpdateOperation(identity);
                    if (result)
                    {
                        this.AddNotification(string.Format("The function [{0} of {1}] is updated succesfully", model.ActionName, model.AccessName), NotificationType.SUCCESS);

                        //Write log
                        var activityText = "Updated the function [{0} of {1}]";
                        activityText = string.Format(activityText, model.ActionName, model.AccessName);
                        WriteActivityLog(activityText, ActivityLogType.UpdateFunction, model.Id.ToString(), TargetObjectType.Function);

                        return RedirectToAction("Index", "Function");
                    }
                    else
                    {
                        this.AddNotification("Could not update function due to database exception occurred", NotificationType.ERROR);
                    }
                }
                catch (Exception ex)
                {
                    strError = string.Format("Could not UpdateFunction because: {0}", ex.ToString());
                    logger.Error(strError);
                    this.AddNotification(strError, NotificationType.ERROR);
                }
            }

            return RedirectToAction("Index", "Function");
        }

        //Show popup confirm delete
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            FunctionViewModel record = new FunctionViewModel();
            record.Id = id;

            return PartialView("_DeleteFunctionInfo", record);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AccessRoleChecker(AdminRequired = true)]
        public ActionResult AcceptDeleteFunction(string id)
        {
            var strError = string.Empty;
            var result = false;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            result = _identityStore.DeleteOperation(Convert.ToInt32(id));
            if (result)
            {
                MenuHelper.ClearAllMenuCache();

                //Write log
                var activityText = "Delete the function [Id: {0}]";
                activityText = string.Format(activityText, id);
                WriteActivityLog(activityText, ActivityLogType.DeleteFunction, id, TargetObjectType.Function);

                return Json(new { success = true });
            }
            else
            {
                throw new Exception("Failed to delete this function");
            }
        }

        public ActionResult ShowOperationLang(int id)
        {
            ManageOperationLangModel model = new ManageOperationLangModel();
            try
            {
                model.OperationId = id;
                model.Languages = LanguagesProvider.GetListLanguages();
                model.OperationInfo = _identityStore.GetOperationDetail(id);
            }
            catch (Exception ex)
            {
                this.AddNotification("Failed to get data because: " + ex.ToString(), NotificationType.ERROR);
                PartialView("_Detail", model);
            }
            return PartialView("_Detail", model);
        }

        public ActionResult UpdateLang()
        {
            ManageOperationLangModel model = new ManageOperationLangModel();
            var id = Utils.ConvertToInt32(Request["Id"]);
            var groupId = Utils.ConvertToInt32(Request["OperationId"]);

            if (groupId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                model.Languages = LanguagesProvider.GetListLanguages();
                model.OperationId = groupId;

                //Begin db transaction
                var info = _identityStore.GetOperationLangDetail(id);

                if (info != null)
                {
                    model.OperationId = groupId;
                    model.Id = info.Id;
                    model.LangCode = info.LangCode;
                    model.OperationName = info.OperationName;
                }
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for Show UpdateLang form request: " + ex.ToString());
            }

            return PartialView("~/Views/Operation/_UpdateLang.cshtml", model);
        }

        [HttpPost]
        public ActionResult UpdateLang(ManageOperationLangModel model)
        {
            var msg = ManagerResource.LB_OPERATION_SUCCESS;
            var isSuccess = false;

            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage + x.Exception));
                this.AddNotification(messages, NotificationType.ERROR);

                return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = messages });
            }

            try
            {
                var code = 0;

                //Begin db transaction
                var data = new IdentityOperationLang();
                data.Id = model.Id;
                data.OperationId = model.OperationId;
                data.OperationName = model.OperationName;
                data.LangCode = model.LangCode;

                if (model.Id > 0)
                {
                    //Update
                    code = _identityStore.UpdateOperationLang(data);

                    if (code == EnumCommonCode.Error)
                    {
                        return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = ManagerResource.LB_DUPLICATE_DATA, clientcallback = string.Format(" ShowOperationLang({0})", model.OperationId) });
                    }

                    //Clear cache
                    //MenuHelper.ClearAllMenuCache();
                }
                else
                {
                    //Add new
                    code = _identityStore.AddOperationLang(data);

                    if (code == EnumCommonCode.Error)
                    {
                        return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = ManagerResource.LB_DUPLICATE_DATA, clientcallback = string.Format(" ShowOperationLang({0})", model.OperationId) });
                    }

                    //Clear cache
                    //MenuHelper.ClearAllMenuCache();
                }

                isSuccess = true;
            }
            catch (Exception ex)
            {
                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

                logger.Error("Failed for UpdateLang request: " + ex.ToString());

                return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = NotifSettings.Error_SystemBusy });
            }

            return Json(new { success = isSuccess, title = ManagerResource.LB_NOTIFICATION, message = msg, clientcallback = string.Format("ShowOperationLang({0})", model.OperationId) });
        }

        public ActionResult DeleteLang()
        {
            ManageOperationLangModel model = new ManageOperationLangModel();
            var id = Utils.ConvertToInt32(Request["Id"]);
            var groupId = Utils.ConvertToInt32(Request["OperationId"]);
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            model.OperationId = groupId;
            model.Id = id;

            return PartialView("~/Views/Operation/_DeleteLangInfo.cshtml", model);
        }

        [HttpPost, ActionName("DeleteLang")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteLang_Confirm(ManageOperationLangModel model)
        {
            var strError = string.Empty;
            if (model.Id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                _identityStore.DeleteOperationLang(model.Id);

                //Clear cache
                //MenuHelper.ClearAllMenuCache();
            }
            catch (Exception ex)
            {
                strError = ManagerResource.LB_SYSTEM_BUSY;

                logger.Error("Failed to get Delete Operation Lang because: " + ex.ToString());

                return Json(new { success = false, message = strError });
            }

            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = string.Format(" ShowOpeartionLang({0})", model.OperationId) });
        }
        #region Helpers

        private void WriteActivityLog(string activityText, ActivityLogType activityType, string targetId, TargetObjectType targetType)
        {
            var logData = new ActivityLog
            {
                UserId = User.Identity.GetUserId(),
                ActivityText = activityText,
                ActivityType = activityType.ToString(),
                TargetId = targetId,
                TargetType = targetType.ToString(),
                IPAddress = Request.UserHostAddress
            };

            _activityStore.WriteActivityLog(logData);
        }

        #endregion
    }
}