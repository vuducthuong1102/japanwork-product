//using System;
//using System.Threading.Tasks;

//using ApiJobMarket.Models;
//using ApiJobMarket.Helpers;
//using ApiJobMarket.Settings;
//using ApiJobMarket.Logging;
//using StackExchange.Redis.Extensions.Core;

//namespace ApiJobMarket.Services
//{
//    public class WebAuthRedisService : ServiceBase, IWebAuthRedisService
//    {
        
//        private readonly ILog logger = LogProvider.For<WebAuthRedisService>();

//        private ICacheClient _redisClient;
//        public WebAuthRedisService(ICacheClient redisClient)
//        {
//            _redisClient = redisClient;
//        }

//        public ResponseWebAuthLoginModel GetWebAuthLoginModel(string myKey, string domain)
//        {
//            var surfix = myKey + domain;
//            var LoginRedisKey = Utility.GenerateRedisKeyWithPrefixAndSurfix(Constant.PREFIX_WEB_LOGIN_MODEL_REDIS_KEY, surfix);
//            var returnedModel = _redisClient.Get<ResponseWebAuthLoginModel>(LoginRedisKey);

//            return returnedModel;
//        }

//        public async Task<ResponseWebAuthLoginModel> GetWebAuthLoginModelAsync(string myKey, string domain)
//        {
//            var surfix = myKey + domain;
//            var LoginRedisKey = Utility.GenerateRedisKeyWithPrefixAndSurfix(Constant.PREFIX_WEB_LOGIN_MODEL_REDIS_KEY, surfix);
//            var returnedModel = await _redisClient.GetAsync<ResponseWebAuthLoginModel>(LoginRedisKey);

//            return returnedModel;
//        }

//        public bool SetResponseWebAuthLoginModel(ResponseWebAuthLoginModel model, string myKey, string domain)
//        {
//            var surfix = myKey + domain;
//            var LoginRedisKey = Utility.GenerateRedisKeyWithPrefixAndSurfix(Constant.PREFIX_WEB_LOGIN_MODEL_REDIS_KEY, surfix);
//            var cachedDurations = ApiJobMarketSettings.DefaultCachedTimeout;
//            if (model.Data != null)
//            {
//                if (model.Data.LoginDurations > 0)
//                {
//                    cachedDurations = model.Data.LoginDurations;
//                }
//            }

//            return _redisClient.Replace<ResponseWebAuthLoginModel>(LoginRedisKey, model, DateTimeOffset.Now.AddMinutes(cachedDurations));
//        }

//        public async Task<bool> SetResponseWebAuthLoginModelAsync(ResponseWebAuthLoginModel model, string myKey, string domain)
//        {
//            var surfix = myKey + domain;
//            var LoginRedisKey = Utility.GenerateRedisKeyWithPrefixAndSurfix(Constant.PREFIX_WEB_LOGIN_MODEL_REDIS_KEY, surfix);
//            var cachedDurations = ApiJobMarketSettings.DefaultCachedTimeout;
//            var expiredTime = new DateTimeOffset();
//            var dtNow = DateTime.Now;
//            if (model.Data != null)
//            {
//                if (model.Data.LoginDurations > 0)
//                {
//                    cachedDurations = model.Data.LoginDurations;
//                }

//                //If the token doesnt exists -> Set expired date to Now + cacheduration (Default is 30 minutes)
//                var tokenExpired = model.Data.TokenExpiredDate ?? dtNow.AddMinutes(cachedDurations);

//                //If the token existed but has been expired -> Set expired date to Now + cacheduration (Default is 30 minutes)
//                if (tokenExpired <= dtNow)
//                {
//                    tokenExpired = dtNow.AddMinutes(cachedDurations);
//                }
//                expiredTime = new DateTimeOffset(tokenExpired);
//            }
//            else
//            {
//                expiredTime = DateTimeOffset.Now.AddMinutes(cachedDurations);
//            }           

//            var result = false;
//            result = await _redisClient.ReplaceAsync<ResponseWebAuthLoginModel>(LoginRedisKey, model, expiredTime);
//            return result;
//        }

//        public async Task<bool> ClearCachedDataByKeyAndDomain(string myKey, string domain)
//        {
//            var surfix = myKey + domain;
//            var result = false;
//            var LoginApiRedisKey = Utility.GenerateRedisKeyWithPrefixAndSurfix(Constant.PREFIX_API_LOGIN_MODEL_REDIS_KEY, surfix);
//            var LoginWebRedisKey = Utility.GenerateRedisKeyWithPrefixAndSurfix(Constant.PREFIX_WEB_LOGIN_MODEL_REDIS_KEY, surfix);
//            try
//            {
//                await _redisClient.RemoveAsync(LoginApiRedisKey);
//                await _redisClient.RemoveAsync(LoginWebRedisKey);

//                result = true;
//            }
//            catch
//            {
//            }

//            return result;
//        }
//    }

//    public interface IWebAuthRedisService
//    {
//        ResponseWebAuthLoginModel GetWebAuthLoginModel(string myKey, string domain);
//        Task<ResponseWebAuthLoginModel> GetWebAuthLoginModelAsync(string myKey, string domain);

//        bool SetResponseWebAuthLoginModel(ResponseWebAuthLoginModel model, string myKey, string domain);
//        Task<bool> SetResponseWebAuthLoginModelAsync(ResponseWebAuthLoginModel model, string myKey, string domain);

//        Task<bool> ClearCachedDataByKeyAndDomain(string myKey, string domain);
//    }
//}
