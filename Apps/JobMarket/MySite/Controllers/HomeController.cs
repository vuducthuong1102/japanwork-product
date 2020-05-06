using System.Web.Mvc;
using MySite.Logging;
using MySite.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using MySite.Helpers;
using MySite.Resources;
using MySite.Settings;
using System.Threading.Tasks;
using MySite.Caching;
using MySite.ShareLibs;
using MySite.Services;
using System.Linq;
using MySite.Models.External.OpenWeather;
using System.Web;
using System.Globalization;

namespace MySite.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILog logger = LogProvider.For<HomeController>();


        public HomeController()
        {
            //Constructor
            //var u = Request.ServerVariables["HTTP_USER_AGENT"];
            //var b = new Regex("(android|bb\\d+|meego).+mobile|avantgo|bada\\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase);
            //var v = new Regex("1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\\-(n|u)|c55\\/|capi|ccwa|cdm\\-|cell|chtm|cldc|cmd\\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\\-s|devi|dica|dmob|do(c|p)o|ds(12|\\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\\-|_)|g1 u|g560|gene|gf\\-5|g\\-mo|go(\\.w|od)|gr(ad|un)|haie|hcit|hd\\-(m|p|t)|hei\\-|hi(pt|ta)|hp( i|ip)|hs\\-c|ht(c(\\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\\-(20|go|ma)|i230|iac( |\\-|\\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\\/)|klon|kpt |kwc\\-|kyo(c|k)|le(no|xi)|lg( g|\\/(k|l|u)|50|54|\\-[a-w])|libw|lynx|m1\\-w|m3ga|m50\\/|ma(te|ui|xo)|mc(01|21|ca)|m\\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\\-2|po(ck|rt|se)|prox|psio|pt\\-g|qa\\-a|qc(07|12|21|32|60|\\-[2-7]|i\\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\\-|oo|p\\-)|sdk\\/|se(c(\\-|0|1)|47|mc|nd|ri)|sgh\\-|shar|sie(\\-|m)|sk\\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\\-|v\\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\\-|tdg\\-|tel(i|m)|tim\\-|t\\-mo|to(pl|sh)|ts(70|m\\-|m3|m5)|tx\\-9|up(\\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\\-|your|zeto|zte\\-", RegexOptions.IgnoreCase);
            ////If b.IsMatch(u) Or v.IsMatch(Left(u, 4)) Then Response.Redirect("http://detectmobilebrowser.com/mobile")
            //if (b.IsMatch(u) || v.IsMatch(u.Substring(4)))
            //{
            //    Response.Redirect("http://detectmobilebrowser.com/mobile");
            //}
        }

        public ActionResult SetLanguage(string lang)
        {
            HttpRuntime.Close();
            return RedirectToAction("Index");
        }

        //[OutputCache(CacheProfile = "DefaultOutputCache", VaryByParam = "lang")]
        [HttpGet]
        public ActionResult Index()
        {            
            if (Request["lang"] != null)
            {
                return ChangeLanguage(Request["lang"]);
            }

            //if(Request.UrlReferrer.)
            //if (HttpExtensions.fBrowserIsMobile())
            //{
            //    return View("MobileDetected");
            //}
            //if (model.CurrentPage <= 0)
            //    model.CurrentPage = 1;

            //if (string.IsNullOrEmpty(model.sorting_date))
            //    model.sorting_date = "desc";

            //try
            //{
            //    model.EmploymentTypes = CommonHelpers.GetListEmploymentTypes();
            //}
            //catch (Exception ex)
            //{
            //    var strError = string.Format("Failed when display Home Index because: {0}", ex.ToString());
            //    logger.Error(strError);
            //}
            //return View(model);

            return View();
        }

        //[HttpPost]
        //public ActionResult Index(FormCollection form)
        //{
        //    if (form["lang"] != null)
        //    {
        //        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //        Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
        //        Response.Cache.SetNoStore();
        //    }

        //    return View();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetListProductInDropdown()
        {
            var strError = string.Empty;
            try
            {
                var keyword = (Request["query"] != null) ? Request["query"].ToString() : string.Empty;
                int currentPage = 1;
                int pageSize = SystemSettings.DefaultPageSize;

                //var filter = new IdentityProduct
                //{
                //    Keyword = !string.IsNullOrEmpty(keyword) ? keyword.Trim() : null
                //};

                //var listProducts = _mainStore.GetActiveForChoosen(filter, currentPage, pageSize);

                //List<ProductItemInDropdownListModel> returnList = new List<ProductItemInDropdownListModel>();

                //var listUnits = CommonHelpers.GetListUnit();
                //if (listProducts.HasData())
                //{
                //    foreach (var prd in listProducts)
                //    {
                //        var item = new ProductItemInDropdownListModel();
                //        item.Id = prd.Id;
                //        item.Name = prd.Name;
                //        item.Code = prd.Code;
                //        item.WarehouseNum = Utils.DoubleToStringFormat(prd.WarehouseNum);

                //        var currentUnit = listUnits.Where(x => x.Id == prd.UnitId).FirstOrDefault();

                //        if (currentUnit != null)
                //            item.UnitName = currentUnit.Name;

                //        returnList.Add(item);
                //    }
                //}

                var returnList = new List<ProductItemInDropdownListModel>();
                returnList.Add(new ProductItemInDropdownListModel {Id = 1,Name = "Hokaido", Code = "xxx"});
                returnList.Add(new ProductItemInDropdownListModel {Id = 2,Name = "Suzumoto", Code = "4234"});
                returnList.Add(new ProductItemInDropdownListModel {Id = 3,Name = "Kimono", Code = "fhfgh"});
                returnList.Add(new ProductItemInDropdownListModel {Id = 4,Name = "Hisuke", Code = "bvhwr"});
                returnList.Add(new ProductItemInDropdownListModel {Id = 6,Name = "Honda", Code = "yjxvf"});
                
                return Json(new { success = true, data = returnList });

            }
            catch (Exception ex)
            {
              
                logger.Error("Failed to GetListProductInDropdown because: " + ex.ToString());

                return Json(new { success = false, data = string.Empty, message = strError });
            }
        }

        public ActionResult TermUse()
        {
            return View();
        }

        public ActionResult GetList()
        {
            return Content("[{\"id\":1,\"text\":\"Asia\",\"population\":null,\"flagUrl\":null,\"checked\":false,\"hasChildren\":false,\"children\":[{\"id\":2,\"text\":\"China\",\"population\":1373541278,\"flagUrl\":\"https://code.gijgo.com/flags/24/China.png\",\"checked\":false,\"hasChildren\":false,\"children\":[]},{\"id\":3,\"text\":\"Japan\",\"population\":126730000,\"flagUrl\":\"https://code.gijgo.com/flags/24/Japan.png\",\"checked\":false,\"hasChildren\":false,\"children\":[]},{\"id\":4,\"text\":\"Mongolia\",\"population\":3081677,\"flagUrl\":\"https://code.gijgo.com/flags/24/Mongolia.png\",\"checked\":false,\"hasChildren\":false,\"children\":[]}]},{\"id\":5,\"text\":\"North America\",\"population\":null,\"flagUrl\":null,\"checked\":false,\"hasChildren\":false,\"children\":[{\"id\":6,\"text\":\"USA\",\"population\":325145963,\"flagUrl\":\"https://code.gijgo.com/flags/24/United%20States%20of%20America(USA).png\",\"checked\":false,\"hasChildren\":false,\"children\":[{\"id\":7,\"text\":\"California\",\"population\":39144818,\"flagUrl\":null,\"checked\":false,\"hasChildren\":false,\"children\":[]},{\"id\":8,\"text\":\"Florida\",\"population\":20271272,\"flagUrl\":null,\"checked\":false,\"hasChildren\":false,\"children\":[]}]},{\"id\":9,\"text\":\"Canada\",\"population\":35151728,\"flagUrl\":\"https://code.gijgo.com/flags/24/canada.png\",\"checked\":false,\"hasChildren\":false,\"children\":[]},{\"id\":10,\"text\":\"Mexico\",\"population\":119530753,\"flagUrl\":\"https://code.gijgo.com/flags/24/mexico.png\",\"checked\":false,\"hasChildren\":false,\"children\":[]}]},{\"id\":11,\"text\":\"South America\",\"population\":null,\"flagUrl\":null,\"checked\":false,\"hasChildren\":false,\"children\":[{\"id\":12,\"text\":\"Brazil\",\"population\":207350000,\"flagUrl\":\"https://code.gijgo.com/flags/24/brazil.png\",\"checked\":false,\"hasChildren\":false,\"children\":[]},{\"id\":13,\"text\":\"Argentina\",\"population\":43417000,\"flagUrl\":\"https://code.gijgo.com/flags/24/argentina.png\",\"checked\":false,\"hasChildren\":false,\"children\":[]},{\"id\":14,\"text\":\"Colombia\",\"population\":49819638,\"flagUrl\":\"https://code.gijgo.com/flags/24/colombia.png\",\"checked\":false,\"hasChildren\":false,\"children\":[]}]},{\"id\":15,\"text\":\"Europe\",\"population\":null,\"flagUrl\":null,\"checked\":false,\"hasChildren\":false,\"children\":[{\"id\":16,\"text\":\"England\",\"population\":54786300,\"flagUrl\":\"https://code.gijgo.com/flags/24/england.png\",\"checked\":false,\"hasChildren\":false,\"children\":[]},{\"id\":17,\"text\":\"Germany\",\"population\":82175700,\"flagUrl\":\"https://code.gijgo.com/flags/24/germany.png\",\"checked\":false,\"hasChildren\":false,\"children\":[]},{\"id\":18,\"text\":\"Bulgaria\",\"population\":7101859,\"flagUrl\":\"https://code.gijgo.com/flags/24/bulgaria.png\",\"checked\":false,\"hasChildren\":false,\"children\":[]},{\"id\":19,\"text\":\"Poland\",\"population\":38454576,\"flagUrl\":\"https://code.gijgo.com/flags/24/poland.png\",\"checked\":false,\"hasChildren\":false,\"children\":[]}]}]");
            //return View();
        }

        public async Task<ActionResult> GetWeather()
        {
            var model = new WeatherInputModel();
            var returnModel = new ShowWeatherModel();
            var htmlReturn = string.Empty;
            var isSuccess = false;
            var msg = string.Empty;
            //var hasTimeout = new Ref<bool>();
            //hasTimeout.Value = false;
            try
            {
                model.city = "Hà Nội";
                if (Request["city"] != null)
                    model.city = Request["city"].ToString();     
                
                model.country = "vn";
                var apiReturned = await OpenWeatherService.GetWeather(model);
                returnModel.ListData = new List<WeatherItem>();
                returnModel.city = model.city;
                returnModel.country = model.country;
                if (apiReturned != null)
                {
                    if (apiReturned.data != null)
                    {
                        if (apiReturned.data.list != null && apiReturned.data.list.Count > 0)
                        {
                            isSuccess = true;

                            var currentWeek = GetCurrentWeek();
                            foreach (var today in currentWeek)
                            {
                                var weatherByDate = new WeatherItem();
                                weatherByDate.CurrentDate = today;
                                foreach (var item in apiReturned.data.list)
                                {
                                    var currentDate = EpochTime.DateTime(item.dt);
                                    if (currentDate.Date == today.Date)
                                    {
                                        weatherByDate.DailyData.Add(item);
                                    }
                                }

                                returnModel.ListData.Add(weatherByDate);
                            }

                            htmlReturn = PartialViewAsString("../Widgets/_Weather", returnModel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying GetWeather: {0}", ex.ToString());
                logger.Error(strError);
                isSuccess = false;
                msg = UserWebResource.COMMON_EXCEPTION_NOTIF;
            }

            return Json(new { success = isSuccess, message = msg, html = htmlReturn }, JsonRequestBehavior.AllowGet);
        }

        private List<DateTime> GetCurrentWeek()
        {
            DateTime today = DateTime.Today;
            int currentDayOfWeek = (int)today.DayOfWeek;
            DateTime sunday = today.AddDays(-currentDayOfWeek);
            DateTime monday = sunday.AddDays(1);
            // If we started on Sunday, we should actually have gone *back*
            // 6 days instead of forward 1...
            if (currentDayOfWeek == 0)
            {
                monday = monday.AddDays(-7);
            }
            var dates = Enumerable.Range(0, 7).Select(days => monday.AddDays(days)).ToList();

            return dates;
        }

        public JsonResult GetResources()
        {
            var resources = ResourceSerialiser.ToJson(typeof(UserWebResource), CultureInfo.CurrentUICulture);
            return Json(resources, JsonRequestBehavior.AllowGet);
        }

        #region Helper

       

        #endregion

    }

    public class ProductItemInDropdownListModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string WarehouseNum { get; set; }
        public string UnitName { get; set; }
    }
}
