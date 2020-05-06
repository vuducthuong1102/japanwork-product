﻿using System;
using System.Web.Mvc;
using Manager.WebApp.Helpers;
using Manager.WebApp.Models;

namespace Manager.WebApp.Controllers
{
    public class DemoController : Controller
    {

        // GET: Demo
        //public ActionResult Index(string licenseKey)
        //{
        //    var validKey = false;
        //    var message = string.Empty;

        //    if (licenseKey == "1qazxcbn")
        //        validKey = true;
        //    else
        //        message = "Gogingo notif: Key của bạn không hợp lệ hoặc đã hết hạn.";

        //    return Json(new AjaxResponseModel { Success = validKey, Message = message }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ImageTool()
        {
            return View();
        }

        public ActionResult ClearCaches(string prefix)
        {
            MenuHelper.ClearAllMenuCache();

            return Content("Success");
        }

        public ActionResult TableFixed()
        {
            return View();
        }

        public JsonResult Test1(int id)
        {
            var abc = 0;
            for (int i = 0; i < 100000000; i++)
            {
                abc = 45367 + i;
            }
            return Json(new { success = true, message = "it is ok now !!!" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Test2(DemoModel model)
        {
            return Json(new { success = true, message = "it is ok now !!!", result = model }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create(DemoModel model)
        {
            return View(model);
        }

        public ActionResult Embedded()
        {
            return View();
        }

        #region Rewrite connection method


        #endregion
    }
}