using System.Collections.Generic;
using System.Threading.Tasks;

using SingleSignOn.Models;
using SingleSignOn.Helpers;
using SingleSignOn.Settings;
using SingleSignOn.Logging;
using StackExchange.Redis.Extensions.Core;
using System;

namespace SingleSignOn.Services
{
    public class ApiAuthRedisService : ServiceBase, IApiAuthRedisService
    {
        private readonly ILog logger = LogProvider.For<ApiAuthRedisService>();

        private ICacheClient _redisClient;
        public ApiAuthRedisService(ICacheClient redisClient)
        {
            _redisClient = redisClient;
        }


        public ResponseApiAuthLoginModel GetApiAuthLoginModel(string myKey, string domain)
        {
            var surfix = myKey + domain;
            var LoginRedisKey = Utility.GenerateRedisKeyWithPrefixAndSurfix(Constant.PREFIX_API_LOGIN_MODEL_REDIS_KEY, surfix);
            var returnedModel = _redisClient.Get<ResponseApiAuthLoginModel>(LoginRedisKey);

            return returnedModel;
        }


        public async Task<ResponseApiAuthLoginModel> GetApiAuthLoginModelAsync(string myKey, string domain)
        {
            var surfix = myKey + domain;
            var LoginRedisKey = Utility.GenerateRedisKeyWithPrefixAndSurfix(Constant.PREFIX_API_LOGIN_MODEL_REDIS_KEY, surfix);
            var returnedModel = await _redisClient.GetAsync<ResponseApiAuthLoginModel>(LoginRedisKey);

            return returnedModel;
        }

        public bool SetResponseApiAuthLoginModel(ResponseApiAuthLoginModel model, string myKey, string domain)
        {
            var surfix = myKey + domain;
            var LoginRedisKey = Utility.GenerateRedisKeyWithPrefixAndSurfix(Constant.PREFIX_API_LOGIN_MODEL_REDIS_KEY, surfix);
            var cachedDurations = SingleSignOnSettings.DefaultCachedTimeout;
            if (model.Data != null)
            {
                if (model.Data.LoginDurations > 0)
                {
                    cachedDurations = model.Data.LoginDurations;
                }
            }

            return _redisClient.Replace<ResponseApiAuthLoginModel>(LoginRedisKey, model, DateTimeOffset.Now.AddMinutes(cachedDurations));
        }

        public async Task<bool> SetResponseApiAuthLoginModelAsync(ResponseApiAuthLoginModel model, string myKey, string domain)
        {
            var surfix = myKey + domain;
            var LoginRedisKey = Utility.GenerateRedisKeyWithPrefixAndSurfix(Constant.PREFIX_API_LOGIN_MODEL_REDIS_KEY, surfix);
            var cachedDurations = SingleSignOnSettings.DefaultCachedTimeout;
            var expiredTime = new DateTimeOffset();
            var dtNow = DateTime.Now;
            if (model.Data != null)
            {
                if (model.Data.LoginDurations > 0)
                {
                    cachedDurations = model.Data.LoginDurations;
                }

                //If the token doesnt exists -> Set expired date to Now + cacheduration (Default is 30 minutes)
                var tokenExpired = model.Data.TokenExpiredDate ?? dtNow.AddMinutes(cachedDurations);

                //If the token existed but has been expired -> Set expired date to Now + cacheduration (Default is 30 minutes)
                if (tokenExpired <= dtNow)
                {
                    tokenExpired = dtNow.AddMinutes(cachedDurations);
                }
                expiredTime = new DateTimeOffset(tokenExpired);
            }
            else
            {
                expiredTime = DateTimeOffset.Now.AddMinutes(cachedDurations);
            }

            var result = false;
            result = await _redisClient.ReplaceAsync<ResponseApiAuthLoginModel>(LoginRedisKey, model, expiredTime);            
            return result;
        }

        public async Task<bool> ClearCachedDataByKeyAndDomain(string myKey, string domain)
        {
            var surfix = myKey + domain;
            var result = false;
            var LoginApiRedisKey = Utility.GenerateRedisKeyWithPrefixAndSurfix(Constant.PREFIX_API_LOGIN_MODEL_REDIS_KEY, surfix);
            var LoginWebRedisKey = Utility.GenerateRedisKeyWithPrefixAndSurfix(Constant.PREFIX_WEB_LOGIN_MODEL_REDIS_KEY, surfix);
            try
            {
                await _redisClient.RemoveAsync(LoginApiRedisKey);
                await _redisClient.RemoveAsync(LoginWebRedisKey);

                result = true;
            }
            catch
            {
            }

            return result;
        }
    }

    public interface IApiAuthRedisService
    {
        ResponseApiAuthLoginModel GetApiAuthLoginModel(string myKey, string domain);
        Task<ResponseApiAuthLoginModel> GetApiAuthLoginModelAsync(string myKey, string domain);

        bool SetResponseApiAuthLoginModel(ResponseApiAuthLoginModel model, string myKey, string domain);
        Task<bool> SetResponseApiAuthLoginModelAsync(ResponseApiAuthLoginModel model, string myKey, string domain);

        Task<bool> ClearCachedDataByKeyAndDomain(string myKey, string domain);
    }
}
