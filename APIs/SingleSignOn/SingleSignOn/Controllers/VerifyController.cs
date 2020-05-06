using System;
using System.Web.Mvc;

using SingleSignOn.Logging;
using SingleSignOn.DB.Sql.Stores;
using SingleSignOn.Models;
using System.Threading.Tasks;
using SingleSignOn.Services;
using SingleSignOn.DB.Sql.Entities;
using SingleSignOn.Helpers;
using SingleSignOn.Settings;
using SingleSignOn.Resources;
using System.Linq;

namespace SingleSignOn.Controllers
{    
    public class VerifyController : BaseController
    {
        private readonly ILog logger = LogProvider.For<WebAuthController>();
        private IWebAuthRedisService _redisService;
        private readonly IStoreUser _userStore;

        public VerifyController(IWebAuthRedisService redisService, IStoreUser userStore)
        {
            _redisService = redisService;
            _userStore = userStore;
        }

        [HttpGet]
        public async Task<ActionResult> Index(string token)
        {
            var result = EnumCommonCode.Success;
            var returnModel = new ResponseApiModel();

            ApiActiveAccountResponseModel viewModel = new ApiActiveAccountResponseModel();
            viewModel.IsSuccess = false;

            if (string.IsNullOrEmpty(token))
            {
                viewModel.Message = UserWebResource.COMMON_ERROR_DATA_INVALID;
            }

            try
            {
                var dataArr = token.Split('.');
                if (dataArr.Count() < 3)
                {
                    viewModel.Message = UserWebResource.COMMON_ERROR_DATA_INVALID;

                    return View(viewModel);
                }

                var rawData = Utility.DecryptText(dataArr[1], SystemSettings.EncryptKey);

                var userData = rawData.Split('|');
                if (userData.Count() >= 2)
                {
                    //result = _userStore.RecoverPasswordStep2(userData[0], userData[1]);
                    //var apiModel = new ApiActiveAccountModel();

                    //apiModel.ActiveMethod = ActionType.ActiveAccount;
                    //apiModel.UserName = userData[0];
                    //apiModel.HashingData = token;
                    //apiModel.Time = DateTime.Now.ToString(Constant.DATEFORMAT_yyyyMMddHHmmss);

                    var info = new IdentityActiveAccount();
                    info.UserName = userData[0];
                    info.HashingData = token;

                    var code = _userStore.ActiveAccountByEmail(info);
                    await Task.FromResult(code);

                    returnModel.Code = code;
                }

                if (returnModel.Code == 1)
                {
                    ModelState.Clear();
                    viewModel.IsSuccess = true;
                    viewModel.Message = UserWebResource.SUCCESS_ACCOUNT_ACTIVED;
                    return View(viewModel);
                }
                else
                {
                    //Thong bao that bai
                    viewModel.Message = UserWebResource.COMMON_ERROR_DATA_INVALID;
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying to ActiveAccount due to: {0}", ex.ToString());
                logger.Error(strError);

                viewModel.Message = UserWebResource.COMMON_ERROR_DATA_INVALID;
            }

            return View(viewModel);
        }

        #region Helpers
        
        
        #endregion
    }
}
