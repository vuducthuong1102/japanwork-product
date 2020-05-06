//using System;
//using System.Linq;
//using System.Net;
//using System.Web.Mvc;
//using Manager.SharedLibs.Logging;
//using Manager.WebApp.Helpers;
//using Manager.WebApp.Settings;
//using MsSql.AspNet.Identity.Entities;
//using Manager.WebApp.Resources;
//using MsSql.AspNet.Identity.MsSqlStores;
//using Manager.WebApp.Models;
//using Manager.WebApp.Services;
//using Autofac;
//using System.Collections.Generic;
//using Manager.SharedLibs;
//using Manager.SharedLibs;

//namespace Manager.WebApp.Controllers
//{
//    public class WarehouseController : BaseAuthedController
//    {
//        private readonly IStoreWarehouse _mainStore;
//        private readonly ILog logger = LogProvider.For<WarehouseController>();

//        public WarehouseController(IStoreWarehouse mainStore)
//        {
//            _mainStore = mainStore;
//        }

//        [AccessRoleChecker]
//        public ActionResult Index(ManageWarehouseModel model)
//        {
//            int currentPage = 1;
//            int pageSize = SystemSettings.DefaultPageSize;

//            if (string.IsNullOrEmpty(model.SearchExec))
//            {
//                model.SearchExec = "Y";
//                if (!ModelState.IsValid)
//                {
//                    ModelState.Clear();
//                }
//            }

//            if (Request["Page"] != null)
//            {
//                currentPage = Utils.ConvertToInt32(Request["Page"], 1);
//            }

//            var filter = new IdentityWarehouse
//            {
//                ProductCode = !string.IsNullOrEmpty(model.Code) ? model.Code.Trim() : null,
//                Keyword = !string.IsNullOrEmpty(model.Name) ? model.Name.Trim() : null,
//                IsStockTakeQTY = model.IsConfirmStockTakeQTY == null ? -1 : (int)model.IsConfirmStockTakeQTY,
//                IsStockOut = model.IsStockOut == null ? -1 : (int)model.IsStockOut,
//                PropertyCategoryId = model.PropertyCategoryId == null ? 0 : (int)model.PropertyCategoryId
//            };

//            if (!string.IsNullOrEmpty(model.PropertyList))
//                filter.SelectedProperties = model.PropertyList.Split(',');

//            try
//            {
//                model.Units = CommonHelpers.GetListUnit();

//                model.PropertyCategories = CommonHelpers.GetListPropertyCategory();

//                //if (model.PropertyCategories.HasData())
//                //{
//                //    foreach (var item in model.PropertyCategories)
//                //    {
//                //        item.Properties = CommonHelpers.GetListPropertyByParent(item.Id);
//                //    }
//                //}
//                var selectedSeperators = string.Empty;
//                if (model.PropertyCategories.HasData())
//                {
//                    foreach (var item in model.PropertyCategories)
//                    {
//                        item.Properties = CommonHelpers.GetListPropertyByParent(item.Id);
//                    }

//                    var copiedAllCategories = model.PropertyCategories.DeepCopy();
//                    var selectedPropertyCats = CommonHelpers.GetSelectedPropertyCategory(copiedAllCategories, filter.SelectedProperties);

//                    if (selectedPropertyCats.HasData())
//                    {
//                        var counter = 0;
//                        foreach (var cat in selectedPropertyCats)
//                        {
//                            var subStrProperty = "";
//                            if (cat.Properties.HasData())
//                                subStrProperty = string.Join(",", cat.Properties.Select(x => x.Id));

//                            if (!string.IsNullOrEmpty(subStrProperty))
//                            {
//                                if (counter == 0)
//                                    selectedSeperators = subStrProperty;
//                                else
//                                    selectedSeperators = selectedSeperators + "#" + subStrProperty;
//                                counter++;
//                            }
//                        }
//                    }
//                }
//                filter.PropertyList = selectedSeperators;

//                model.SearchResults = _mainStore.GetByPage(filter, currentPage, SystemSettings.DefaultPageSize);
//                if (model.SearchResults != null && model.SearchResults.Count > 0)
//                {
//                    model.TotalCount = model.SearchResults[0].TotalCount;
//                    model.CurrentPage = currentPage;
//                    model.PageSize = pageSize;
//                }
//            }
//            catch (Exception ex)
//            {
//                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

//                logger.Error("Failed to get data because: " + ex.ToString());

//                return View(model);
//            }

//            return View(model);
//        }

//        public ActionResult GoodsReceipt()
//        {
//            WarehouseActionModel model = new WarehouseActionModel();
//            var id = Utils.ConvertToInt32(Request["WarehouseId"]);
//            var productId = Utils.ConvertToInt32(Request["ProductId"]);
          
//            try
//            {
//                model.WarehouseId = id;
//                model.ProductId = productId;

//                var productStore = GlobalContainer.IocContainer.Resolve<IStoreProduct>();

//                if (productId > 0)
//                    model.ProductInfo = productStore.GetDetail(productId);              
                
//                if(model.ProductInfo != null)
//                {
//                    model.ItemCode = model.ProductInfo.Code;
//                    model.ItemName = model.ProductInfo.Name;
//                }

//                model.Units = CommonHelpers.GetListUnit();
//            }
//            catch (Exception ex)
//            {
//                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

//                logger.Error("Failed for Show Goods Receipt form: " + ex.ToString());
//            }

//            if (model.ProductInfo != null)
//            {
//                return PartialView("_GoodsReceipt", model);
//            }
//            else
//            {
//                var tempItem = new WarehouseAddProductItemModel();
//                tempItem.WarehouseNum = "0";

//                model.AddProductsList.Add(tempItem);

//                return PartialView("_GoodsReceiptMultiple", model);
//            }
//        }

//        public ActionResult GoodsIssue()
//        {
//            WarehouseActionModel model = new WarehouseActionModel();
//            var id = Utils.ConvertToInt32(Request["WarehouseId"]);
//            var productId = Utils.ConvertToInt32(Request["ProductId"]);

//            try
//            {
//                model.WarehouseId = id;
//                model.ProductId = productId;

//                var productStore = GlobalContainer.IocContainer.Resolve<IStoreProduct>();

//                if (productId > 0)
//                    model.ProductInfo = productStore.GetDetail(productId);

//                if (model.ProductInfo != null)
//                {
//                    model.ItemCode = model.ProductInfo.Code;
//                    model.ItemName = model.ProductInfo.Name;
//                }

//                model.Units = CommonHelpers.GetListUnit();
//            }
//            catch (Exception ex)
//            {
//                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

//                logger.Error("Failed for Show Goods Issue form: " + ex.ToString());
//            }

//            if (model.ProductInfo != null)
//            {
//                return PartialView("_GoodsIssue", model);
//            }
//            else
//            {
//                var tempItem = new WarehouseAddProductItemModel();
//                tempItem.WarehouseNum = "0";

//                model.AddProductsList.Add(tempItem);

//                return PartialView("_GoodsIssueMultiple", model);
//            }
//        }

//        public ActionResult ReflectStockTake()
//        {
//            WarehouseActionModel model = new WarehouseActionModel();
//            var id = Utils.ConvertToInt32(Request["WarehouseId"]);
//            var productId = Utils.ConvertToInt32(Request["ProductId"]);         

//            try
//            {
//                //model.WarehouseId = id;
//                model.ProductId = productId;

//                //var productStore = GlobalContainer.IocContainer.Resolve<IStoreProduct>();

//                //if (productId > 0)
//                //    model.ProductInfo = productStore.GetDetail(productId);

//                //if (model.ProductInfo != null)
//                //{
//                //    model.ItemCode = model.ProductInfo.Code;
//                //    model.ItemName = model.ProductInfo.Name;
//                //}

//                //model.Units = CommonHelpers.GetListUnit();
//            }
//            catch (Exception ex)
//            {
//                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

//                logger.Error("Failed for Show Reflect Stock-take form: " + ex.ToString());
//            }

//            return PartialView("_ReflectStockTake", model);
//        }

//        [HttpPost]
//        [ActionName("GoodsReceipt")]
//        [ValidateAntiForgeryToken]
//        public ActionResult GoodsReceipt(WarehouseActionModel model)
//        {
//            var strError = string.Empty;
//            try
//            {                
//                var productInfo = new IdentityProduct();
//                var warehouseInfo = new IdentityWarehouse();

//                productInfo.Id = model.ProductId;
//                productInfo.Code = model.ItemCode;

//                var qty = Utils.ConvertToDouble(model.WarehouseNum);
//                if (qty <= 0)
//                    return Json(new { success = false, message = ManagerResource.ERROR_QTY_MUST_LARGE_THAN_0, title = ManagerResource.LB_NOTIFICATION, clientcallback = " ShowMyModalAgain();" });

//                if (model.ProductId <= 0)
//                {
//                    if (!string.IsNullOrEmpty(model.ItemCode))
//                    {
//                        var productStore = GlobalContainer.IocContainer.Resolve<IStoreProduct>();
//                        var productInfoCheck = productStore.GetByCode(model.ItemCode);
                        
//                        if(productInfoCheck == null || (productInfoCheck != null && productInfoCheck.Id <= 0))
//                            return Json(new { success = false, message = string.Format(ManagerResource.ERROR_PRODUCT_ITEM_CODE_NOTFOUND_FORMAT, model.ItemCode), title = ManagerResource.LB_NOTIFICATION, clientcallback = " ShowMyModalAgain();" });
//                        else
//                            productInfo = productInfoCheck;
//                    }
//                }
                
//                productInfo.WarehouseNum = qty;
//                warehouseInfo.ProductList.Add(productInfo);

//                var activityInfo = new IdentityWarehouseActivity();
//                activityInfo.ActivityType = (int)EnumWarehouseActivityType.GoodsReceipt;
//                activityInfo.DeviceId = RegisterNewDevice();
//                activityInfo.StaffId = GetCurrentStaffId();

//                //Database execute
//                _mainStore.GoodsReceipt(warehouseInfo, activityInfo);
//            }
//            catch (Exception ex)
//            {
//                strError = ManagerResource.LB_SYSTEM_BUSY;

//                logger.Error("Failed to exec GoodsReceipt because: " + ex.ToString());

//                return Json(new { success = false, message = strError });
//            }

//            return Json(new { success = true, message = ManagerResource.LB_GOODS_RECEIPT_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = " location.reload()" });
//        }

//        [HttpPost]
//        [ActionName("GoodsIssue")]
//        [ValidateAntiForgeryToken]
//        public ActionResult GoodsIssue(WarehouseActionModel model)
//        {
//            var strError = string.Empty;
//            try
//            {
//                var productInfo = new IdentityProduct();
//                var warehouseInfo = new IdentityWarehouse();

//                productInfo.Id = model.ProductId;
//                productInfo.Code = model.ItemCode;

//                var qty = Utils.ConvertToDouble(model.WarehouseNum);
//                if (qty <= 0)
//                    return Json(new { success = false, message = ManagerResource.ERROR_QTY_MUST_LARGE_THAN_0, title = ManagerResource.LB_NOTIFICATION, clientcallback = " ShowMyModalAgain();" });

//                if (model.ProductId <= 0)
//                {
//                    if (!string.IsNullOrEmpty(model.ItemCode))
//                    {
//                        var productStore = GlobalContainer.IocContainer.Resolve<IStoreProduct>();
//                        var productInfoCheck = productStore.GetByCode(model.ItemCode);

//                        if (productInfoCheck == null || (productInfoCheck != null && productInfoCheck.Id <= 0))
//                            return Json(new { success = false, message = string.Format(ManagerResource.ERROR_PRODUCT_ITEM_CODE_NOTFOUND_FORMAT, model.ItemCode), title = ManagerResource.LB_ERROR_OCCURED, clientcallback = " ShowMyModalAgain();" });
//                        else
//                            productInfo = productInfoCheck;
//                    }
//                }                               

//                productInfo.WarehouseNum = qty;

//                warehouseInfo.ProductList.Add(productInfo);

//                var activityInfo = new IdentityWarehouseActivity();
//                activityInfo.ActivityType = (int)EnumWarehouseActivityType.GoodsIssue;
//                activityInfo.DeviceId = RegisterNewDevice();
//                activityInfo.StaffId = GetCurrentStaffId();               

//                //Database execute
//                var numCode = _mainStore.GoodsIssue(warehouseInfo, activityInfo);
//                if(numCode == -1)
//                {
//                    var numberHighlight = string.Format("<b>{0}</b>", Utils.DoubleToStringFormat(Utils.ConvertToDouble(model.WarehouseNum)));
//                    return Json(new { success = false, message = string.Format(ManagerResource.ERROR_GOODS_ISSUE_NOT_ENOUGH_FORMAT, numberHighlight), title = ManagerResource.LB_ERROR_OCCURED, clientcallback = " ShowMyModalAgain();" });
//                }
//            }
//            catch (Exception ex)
//            {
//                strError = ManagerResource.LB_SYSTEM_BUSY;

//                logger.Error("Failed to exec GoodsIssue because: " + ex.ToString());

//                return Json(new { success = false, message = strError });
//            }

//            return Json(new { success = true, message = ManagerResource.LB_GOODS_ISSUE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = " location.reload()" });
//        }

//        [HttpPost]
//        [ActionName("ReflectStockTake")]
//        [ValidateAntiForgeryToken]
//        public ActionResult ReflectStockTake(WarehouseActionModel model)
//        {
//            var strError = string.Empty;
//            try
//            {
//                var productInfo = new IdentityProduct();
//                var warehouseInfo = new IdentityWarehouse();

//                productInfo.Id = model.ProductId;
//                productInfo.Code = model.ItemCode;

//                var productStore = GlobalContainer.IocContainer.Resolve<IStoreProduct>();
//                var productInfoCheck = productStore.GetById(model.ProductId);
//                if (productInfoCheck == null || (productInfoCheck != null && productInfoCheck.Id <= 0))
//                {
//                    return Json(new { success = false, message = ManagerResource.ERROR_PRODUCT_ITEM_NOTFOUND, title = ManagerResource.LB_NOTIFICATION });
//                }
//                else
//                {
//                    productInfo = productInfoCheck;
//                }
//                //if (model.ProductId <= 0)
//                //{
//                //    if (!string.IsNullOrEmpty(model.ItemCode))
//                //    {
//                //        var productStore = GlobalContainer.IocContainer.Resolve<IStoreProduct>();
//                //        var productInfoCheck = productStore.GetByCode(model.ItemCode);

//                //        if (productInfoCheck == null || (productInfoCheck != null && productInfoCheck.Id <= 0))
//                //            return Json(new { success = false, message = string.Format(ManagerResource.ERROR_PRODUCT_ITEM_CODE_NOTFOUND_FORMAT, model.ItemCode), title = ManagerResource.LB_NOTIFICATION, clientcallback = " ShowMyModalAgain();" });
//                //        else
//                //            productInfo = productInfoCheck;
//                //    }
//                //}

//                //productInfo.WarehouseNum = model.WarehouseNum;
//                warehouseInfo.ProductList.Add(productInfo);

//                var activityInfo = new IdentityWarehouseActivity();
//                activityInfo.ActivityType = (int)EnumWarehouseActivityType.ReflectStockTake;
//                activityInfo.DeviceId = RegisterNewDevice();
//                activityInfo.StaffId = GetCurrentStaffId();

//                //Database execute
//                _mainStore.ReflectStockTake(warehouseInfo, activityInfo);
//            }
//            catch (Exception ex)
//            {
//                strError = ManagerResource.LB_SYSTEM_BUSY;

//                logger.Error("Failed to exec ReflectStockTake because: " + ex.ToString());

//                return Json(new { success = false, message = strError });
//            }

//            return Json(new { success = true, message = ManagerResource.LB_GOODS_REFLECT_STOCK_TAKE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = " location.reload()" });
//        }

//        private string CheckValidItems(WarehouseActionModel model)
//        {
//            var strError = string.Empty;
//            if (model.AddProductsList == null)
//                strError = ManagerResource.ERROR_ITEM_LIST_EMPTY;

//            var totalItems = model.AddProductsList.Count;

//            if (string.IsNullOrEmpty(strError))
//            {
//                if (totalItems > 1)
//                {
//                    if (totalItems > 0)
//                    {
//                       var realItemCount = model.AddProductsList.Count(x => x.ProductId > 0);
//                       if(realItemCount == 0)
//                            strError = ManagerResource.ERROR_ITEM_LIST_EMPTY;
//                    }                       

//                    if (string.IsNullOrEmpty(strError))
//                    {
//                        foreach (var item in model.AddProductsList)
//                        {
//                            var itemCount = model.AddProductsList.Count(x => x.ProductId == item.ProductId && x.ProductId > 0);
//                            if (itemCount > 1)
//                            {
//                                strError += string.Format("<b>- {0} </b>: {1} <br />", item.Name, ManagerResource.ERROR_ITEM_DUPLICATED_ON_LIST);
//                                break;
//                            }
//                        }
//                    }                                       
//                }
//                else if(totalItems == 1)
//                {
//                    if (string.IsNullOrEmpty(model.AddProductsList[0].Code))
//                    {
//                        strError = ManagerResource.ERROR_ITEM_LIST_EMPTY;
//                    }
//                }
//                else
//                {
//                    strError = ManagerResource.ERROR_ITEM_LIST_EMPTY;
//                }
//            }
            

//            if (string.IsNullOrEmpty(strError))
//            {
//                if(totalItems > 0)
//                {
//                    foreach (var item in model.AddProductsList)
//                    {
//                        var qty = Utils.ConvertToDouble(item.WarehouseNum);
//                        if (qty <= 0 && !string.IsNullOrEmpty(item.Code))
//                        {
//                            strError += string.Format("<b>- {0}</b>: {1} <br />", item.Name, ManagerResource.ERROR_QTY_MUST_LARGE_THAN_0);
//                        }
//                    }
//                }
//            }

//            return strError;
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult GoodsReceiptMultiple(WarehouseActionModel model)
//        {
//            var strError = string.Empty;
//            try
//            {
//                strError = CheckValidItems(model);

//                if (!string.IsNullOrEmpty(strError))
//                    return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION, clientcallback = " ShowMyModalAgain();" });

//                var warehouseInfo = new IdentityWarehouse();
//                foreach (var item in model.AddProductsList)
//                {
//                    var productInfo = new IdentityProduct();
//                    productInfo.Id = item.ProductId;
//                    productInfo.Code = item.Code;

//                    var qty = Utils.ConvertToDouble(item.WarehouseNum);
//                    productInfo.WarehouseNum = qty;

//                    warehouseInfo.ProductList.Add(productInfo);
//                }

//                var activityInfo = new IdentityWarehouseActivity();
//                activityInfo.ActivityType = (int)EnumWarehouseActivityType.GoodsReceipt;
//                activityInfo.DeviceId = RegisterNewDevice();
//                activityInfo.StaffId = GetCurrentStaffId();

//                //Database execute
//                _mainStore.GoodsReceipt(warehouseInfo, activityInfo);                
//            }
//            catch (Exception ex)
//            {
//                strError = ManagerResource.LB_SYSTEM_BUSY;

//                logger.Error("Failed to exec GoodsReceipt Multiple because: " + ex.ToString());

//                return Json(new { success = false, message = strError });
//            }

//            return Json(new { success = true, message = ManagerResource.LB_GOODS_RECEIPT_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = " location.reload()" });
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult GoodsIssueMultiple(WarehouseActionModel model)
//        {
//            var strError = string.Empty;
//            try
//            {
//                strError = CheckValidItems(model);

//                if (!string.IsNullOrEmpty(strError))
//                    return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION, clientcallback = " ShowMyModalAgain();" });

//                var warehouseInfo = new IdentityWarehouse();
//                var listIds = new List<int>();
//                foreach (var item in model.AddProductsList)
//                {
//                    var productInfo = new IdentityProduct();
//                    productInfo.Id = item.ProductId;
//                    productInfo.Code = item.Code;

//                    var qty = Utils.ConvertToDouble(item.WarehouseNum);
//                    productInfo.WarehouseNum = qty;

//                    warehouseInfo.ProductList.Add(productInfo);

//                    listIds.Add(item.ProductId);
//                }

//                var productStore = GlobalContainer.IocContainer.Resolve<IStoreProduct>();

//                var listProductForIssue = productStore.GetByListIds(listIds);
//                if (listProductForIssue.HasData())
//                {
//                    foreach (var item in model.AddProductsList)
//                    {                        
//                        var qty = Utils.ConvertToDouble(item.WarehouseNum);
//                        var matchedProduct = listProductForIssue.Where(x => x.Id == item.ProductId).FirstOrDefault();
//                        if(matchedProduct != null)
//                        {
//                            if(qty > matchedProduct.WarehouseNum)
//                            {
//                                var strFormat = string.Format(ManagerResource.ERROR_GOODS_ISSUE_NOT_ENOUGH_FORMAT, string.Format("{0:n0}", qty));
//                                strError += string.Format("<b>- {0}</b>: {1} <br />", matchedProduct.Name, strFormat);
//                            }
//                        }
//                    }
//                }

//                if (!string.IsNullOrEmpty(strError))
//                    return Json(new { success = false, message = strError, title = ManagerResource.LB_NOTIFICATION, clientcallback = " ShowMyModalAgain();" });

//                var activityInfo = new IdentityWarehouseActivity();
//                activityInfo.ActivityType = (int)EnumWarehouseActivityType.GoodsIssue;
//                activityInfo.DeviceId = RegisterNewDevice();
//                activityInfo.StaffId = GetCurrentStaffId();

//                //Database execute
//                _mainStore.GoodsIssue(warehouseInfo, activityInfo);                
//            }
//            catch (Exception ex)
//            {
//                strError = ManagerResource.LB_SYSTEM_BUSY;

//                logger.Error("Failed to exec GoodsIssue Multiple because: " + ex.ToString());

//                return Json(new { success = false, message = strError });
//            }

//            return Json(new { success = true, message = ManagerResource.LB_GOODS_ISSUE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = " location.reload()" });
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult ReflectStockTakeMultiple(WarehouseActionModel model)
//        {
//            var strError = string.Empty;
//            try
//            {
//                //var productInfo = new IdentityProduct();
//                //var warehouseInfo = new IdentityWarehouse();

//                //productInfo.Id = model.ProductId;
//                //productInfo.Code = model.ItemCode;

//                //var productStore = GlobalContainer.IocContainer.Resolve<IStoreProduct>();
//                //var productInfoCheck = productStore.GetById(model.ProductId);
//                //if (productInfoCheck == null || (productInfoCheck != null && productInfoCheck.Id <= 0))
//                //{
//                //    return Json(new { success = false, message = ManagerResource.ERROR_PRODUCT_ITEM_NOTFOUND, title = ManagerResource.LB_NOTIFICATION });
//                //}
//                //else
//                //{
//                //    productInfo = productInfoCheck;
//                //}
//                //if (model.ProductId <= 0)
//                //{
//                //    if (!string.IsNullOrEmpty(model.ItemCode))
//                //    {
//                //        var productStore = GlobalContainer.IocContainer.Resolve<IStoreProduct>();
//                //        var productInfoCheck = productStore.GetByCode(model.ItemCode);

//                //        if (productInfoCheck == null || (productInfoCheck != null && productInfoCheck.Id <= 0))
//                //            return Json(new { success = false, message = string.Format(ManagerResource.ERROR_PRODUCT_ITEM_CODE_NOTFOUND_FORMAT, model.ItemCode), title = ManagerResource.LB_NOTIFICATION, clientcallback = " ShowMyModalAgain();" });
//                //        else
//                //            productInfo = productInfoCheck;
//                //    }
//                //}

//                //productInfo.WarehouseNum = model.WarehouseNum;
//                //warehouseInfo.ProductList.Add(productInfo);

//                //var activityInfo = new IdentityWarehouseActivity();
//                //activityInfo.ActivityType = (int)EnumWarehouseActivityType.ReflectStockTake;
//                //activityInfo.DeviceId = RegisterNewDevice();
//                //activityInfo.StaffId = GetCurrentStaffId();

//                //Database execute
//                //_mainStore.ReflectStockTake(warehouseInfo, activityInfo);

//                var warehouseInfo = new IdentityWarehouse();
//                if (model.AddProductsList.HasData())
//                {
//                    foreach (var item in model.AddProductsList)
//                    {
//                        var productInfo = new IdentityProduct();
//                        productInfo.Id = item.ProductId;
//                        productInfo.Code = item.Code;
//                        productInfo.WarehouseNum = Utils.ConvertToDouble(item.WarehouseNum);

//                        warehouseInfo.ProductList.Add(productInfo);
//                    }
//                }

//                var activityInfo = new IdentityWarehouseActivity();
//                activityInfo.ActivityType = (int)EnumWarehouseActivityType.ReflectStockTake;
//                activityInfo.DeviceId = RegisterNewDevice();
//                activityInfo.StaffId = GetCurrentStaffId();

//                //Database execute
//                _mainStore.ReflectStockTake(warehouseInfo, activityInfo);
//            }
//            catch (Exception ex)
//            {
//                strError = ManagerResource.LB_SYSTEM_BUSY;

//                logger.Error("Failed to exec ReflectStockTake Multiple because: " + ex.ToString());

//                return Json(new { success = false, message = strError });
//            }

//            return Json(new { success = true, message = ManagerResource.LB_GOODS_REFLECT_STOCK_TAKE_SUCCESS, title = ManagerResource.LB_NOTIFICATION, clientcallback = " location.reload()" });
//        }

//        [HttpPost]
//        public ActionResult RemoveReflection(int id)
//        {
//            var strError = string.Empty;
//            if (id <= 0)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }

//            try
//            {
//                _mainStore.RemoveReflection(id);
//            }
//            catch (Exception ex)
//            {
//                strError = ManagerResource.LB_SYSTEM_BUSY;

//                logger.Error("Failed to get RemoveReflection because: " + ex.ToString());

//                return Json(new { success = false, message = strError });
//            }

//            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
//        }

//        //Show popup confirm delete        
//        //[AccessRoleChecker]
//        public ActionResult Delete(int id)
//        {
//            if (id <= 0)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }

//            return PartialView("_PopupDelete", id);
//        }

//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult Delete_Confirm(int id)
//        {
//            var strError = string.Empty;
//            if (id <= 0)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }

//            try
//            {
//                _mainStore.Delete(id);

//                //Clear cache
//                //CachingHelpers.ClearWarehouseCache(id);
//            }
//            catch (Exception ex)
//            {
//                strError = ManagerResource.LB_SYSTEM_BUSY;

//                logger.Error("Failed to get Delete Warehouse because: " + ex.ToString());

//                return Json(new { success = false, message = strError });
//            }

//            return Json(new { success = true, message = ManagerResource.LB_DELETE_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
//        }

//        public ActionResult GetListProductStockOut(ManageWarehouseModel model)
//        {
//            int currentPage = 1;
//            int pageSize = SystemSettings.DefaultPageSize;

//            if (string.IsNullOrEmpty(model.SearchExec))
//            {
//                model.SearchExec = "Y";
//                if (!ModelState.IsValid)
//                {
//                    ModelState.Clear();
//                }
//            }

//            currentPage = model.CurrentPage;
//            if (currentPage == 0)
//                currentPage = 1;

//            if (model.PageSize > 0)
//                pageSize = model.PageSize;

//            if (pageSize <= 0 || pageSize > 100)
//                pageSize = SystemSettings.DefaultPageSize;

//            var filter = new IdentityWarehouse
//            {
//                //ProductCode = !string.IsNullOrEmpty(model.Code) ? model.Code.Trim() : null,
//                //Keyword = !string.IsNullOrEmpty(model.Name) ? model.Name.Trim() : null,
//            };

//            try
//            {
//                model.Units = CommonHelpers.GetListUnit();
//                var warehouseStore = GlobalContainer.IocContainer.Resolve<IStoreWarehouse>();
//                model.SearchResults = warehouseStore.GetProductStockOutByPage(filter, currentPage, SystemSettings.DefaultPageSize);
//                if (model.SearchResults != null && model.SearchResults.Count > 0)
//                {
//                    model.TotalCount = model.SearchResults[0].TotalCount;
//                    model.CurrentPage = currentPage;
//                    model.PageSize = pageSize;
//                }
//            }
//            catch (Exception ex)
//            {
//                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

//                logger.Error("Failed to get GetProductStockOutByPage because: " + ex.ToString());
//            }

//            return PartialView("~/Views/Warehouse/Partials/_ProductStockOutList.cshtml", model);
//        }
            
//        [AccessRoleChecker]
//        public ActionResult History(WarehouseHistoryModel model)
//        {
//            int currentPage = 1;
//            int pageSize = SystemSettings.DefaultPageSize;

//            if (string.IsNullOrEmpty(model.SearchExec))
//            {
//                model.SearchExec = "Y";
//                if (!ModelState.IsValid)
//                {
//                    ModelState.Clear();
//                }
//            }

//            if (Request["Page"] != null)
//            {
//                currentPage = Utils.ConvertToInt32(Request["Page"], 1);
//            }

//            if (model.PageSize > 0)
//            {
//                pageSize = model.PageSize;
//            }

//            if (pageSize == 0 || pageSize > 100)
//                pageSize = SystemSettings.DefaultPageSize;

//            try
//            {
//                var filter = new IdentityWarehouseActivity
//                {
//                    ProductId = model.ProductId,
//                    Keyword = model.Name,
//                    StaffId = model.StaffId,
//                    ActivityType = model.ActivityType,      
//                    DeviceId = model.DeviceId
//                };

//                filter.FromDate = Utils.ConvertStringToDateTimeByFormat(model.FromDate);
//                if(filter.FromDate != null)
//                    filter.FromDate = Utils.GetBeginOfDate(filter.FromDate.Value);

//                filter.ToDate = Utils.ConvertStringToDateTimeByFormat(model.ToDate);
//                if (filter.ToDate != null)
//                    filter.ToDate = Utils.GetEndOfDate(filter.ToDate.Value);

//                if (filter.FromDate > filter.ToDate)
//                {
//                    this.AddNotification(ManagerResource.ERROR_DATE_RANGE_IN_PAST, NotificationType.ERROR);

//                    return View(model);
//                }

//                //if (filter.FromDate == null && filter.ToDate == null)
//                //{
//                //    filter.FromDate = Utils.GetBeginOfDate(DateTime.Now);
//                //    filter.ToDate = Utils.GetEndOfDate(DateTime.Now);
//                //}

//                model.Units = CommonHelpers.GetListUnit();
//                model.Devices = CommonHelpers.GetListDevice();
//                model.Users = CommonHelpers.GetListUser();

//                model.SearchResults = _mainStore.GetHistoryByPage(filter, currentPage, pageSize);
//                if (model.SearchResults.HasData())
//                {
//                    model.TotalCount = model.SearchResults[0].TotalCount;
//                    model.CurrentPage = currentPage;
//                    model.PageSize = pageSize;
//                }
//            }
//            catch (Exception ex)
//            {
//                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

//                logger.Error("Failed to get data because: " + ex.ToString());

//                return View(model);
//            }

//            return View(model);
//        }

//        public ActionResult GetWarehouseActivityList(WarehouseHistoryModel model)
//        {
//            int currentPage = 1;
//            int pageSize = SystemSettings.DefaultPageSize;

//            if (string.IsNullOrEmpty(model.SearchExec))
//            {
//                model.SearchExec = "Y";
//                if (!ModelState.IsValid)
//                {
//                    ModelState.Clear();
//                }
//            }

//            currentPage = model.CurrentPage;
//            if (currentPage == 0)
//                currentPage = 1;

//            if (model.PageSize > 0)
//            {
//                pageSize = model.PageSize;
//            }

//            if (pageSize <= 0 || pageSize > 100)
//                pageSize = SystemSettings.DefaultPageSize;

//            try
//            {
//                var filter = new IdentityWarehouseActivity
//                {
//                    ProductId = model.ProductId,
//                    ActivityType = model.ActivityType,
//                    DeviceId = model.DeviceId
//                };

//                filter.FromDate = Utils.ConvertStringToDateTimeByFormat(model.FromDate);
//                if (filter.FromDate != null)
//                    filter.FromDate = Utils.GetBeginOfDate(filter.FromDate.Value);

//                filter.ToDate = Utils.ConvertStringToDateTimeByFormat(model.ToDate);
//                if (filter.ToDate != null)
//                    filter.ToDate = Utils.GetEndOfDate(filter.ToDate.Value);

//                if (filter.FromDate > filter.ToDate)
//                {
//                    this.AddNotification(ManagerResource.ERROR_DATE_RANGE_IN_PAST, NotificationType.ERROR);

//                    return View(model);
//                }

//                //if (filter.FromDate == null && filter.ToDate == null)
//                //{
//                //    filter.FromDate = Utils.GetBeginOfDate(DateTime.Now);
//                //    filter.ToDate = Utils.GetEndOfDate(DateTime.Now);
//                //}

//                model.Units = CommonHelpers.GetListUnit();
//                model.Devices = CommonHelpers.GetListDevice();

//                model.SearchResults = _mainStore.GetHistoryByPage(filter, currentPage, pageSize);
//                if (model.SearchResults.HasData())
//                {
//                    model.TotalCount = model.SearchResults[0].TotalCount;
//                    model.CurrentPage = currentPage;
//                    model.PageSize = pageSize;
//                }
//            }
//            catch (Exception ex)
//            {
//                this.AddNotification(NotifSettings.Error_SystemBusy, NotificationType.ERROR);

//                logger.Error("Failed to get GetWarehouseActivityList because: " + ex.ToString());

//                return View(model);
//            }

//            return PartialView("~/Views/Warehouse/Partials/_ActivityHistoryList.cshtml", model);
//        }

//        #region Helpers

//        private int RegisterNewDevice()
//        {
//            var deviceId = 0;
//            var isNew = false;
//            try
//            {
//                var deviceInfo = new IdentityDevice();
//                //deviceInfo.Name = System.Environment.MachineName;
//                deviceInfo.Name = "PC";
//                deviceId = CommonHelpers.RegisterNewDevice(deviceInfo, out isNew);
//            }
//            catch (Exception ex)
//            {
//                logger.Error("Failed to RegisterNewDevice because: " + ex.ToString());
//            }

//            return deviceId;
//        }

//        #endregion

//    }
//}