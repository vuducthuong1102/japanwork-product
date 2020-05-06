using Autofac;
using MyCloud.Models;
using MyCloud.Services;
using MyCloud.Settings;
using MyCloud.SharedLib.Caching.Providers;
using MyCloud.SharedLib.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyCloud.Helpers
{
    public class CommonGateHelpers
    {
        private static ICacheProvider _myCache;
        private static ILog logger = LogProvider.For<MessengerHelpers>();
        private static string _onlineUsersCacheKey = string.Format("{0}{1}", SystemSettings.DefaultCacheKeyPrefix, "ONLINE_USERS");
        //private static string _offlineDeviceCacheKey = string.Format("{0}{1}", SystemSettings.DefaultCacheKeyPrefix, "OFFLINE_DEVICES");
        private static int _cacheExpiredTime = 60; //One week

        public static List<Connector> GetAllUsersFromCache()
        {
            var strError = string.Empty;
            List<Connector> listUser = null;

            try
            {
                _myCache = GlobalContainer.IocContainer.Resolve<ICacheProvider>();

                _myCache.Get(_onlineUsersCacheKey, out listUser);
            }
            catch (Exception ex)
            {
                strError = string.Format("Could not GetAllUsersFromCache because: {0}", ex.ToString());
                logger.Error(strError);
            }

            if (listUser == null)
                listUser = new List<Connector>();

            return listUser;
        }

        public static List<JobSeekerDevice> GetDevicesByUser(int userId)
        {
            var strError = string.Empty;
            List<JobSeekerDevice> devices = null;

            try
            {
                var apiResponse = MemberServices.GetAllDevicesAsync(new ApiUserModel { UserId = userId }).Result;
                if(apiResponse != null)
                {
                    if(apiResponse.value != null)
                    devices = JsonConvert.DeserializeObject<List<JobSeekerDevice>>(apiResponse.value.ToString());
                }
            }
            catch (Exception ex)
            {
                strError = string.Format("Could not GetDevicesByUser because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return devices;
        }

        public static void NewDeviceConnected(JobSeekerDevice device)
        {
            var strError = string.Empty;

            try
            {
                var apiModel = new ApiJobSeekerDeviceModel();

                apiModel.job_seeker_id = device.job_seeker_id;
                apiModel.device_name = device.device_name;
                apiModel.device_id = device.device_id;
                apiModel.device_type = device.device_type;
                apiModel.registration_id = device.registration_id;
                apiModel.language_code = device.language_code;


                var apiResponse = MemberServices.UpdateDeviceAsync(apiModel).Result;
            }
            catch (Exception ex)
            {
                strError = string.Format("Could not exec NewDeviceConnected because: {0}", ex.ToString());
                logger.Error(strError);
            }
        }

        public static List<Connector> AddUserToCache(Connector newUser)
        {
            var strError = string.Empty;
            var existed = false;

            var listUser = GetAllUsersFromCache();
            try
            {
                if (listUser != null && listUser.Count > 0)
                {
                    //Remove if any
                    //listUser.RemoveAll(x => x.UserId == newUser.UserId);
                    foreach (var item in listUser)
                    {
                        if (newUser.id == item.id)
                        {
                            item.ConnectionId = newUser.ConnectionId;
                            //item.DisplayName = newUser.DisplayName;
                            //item.Avatar = newUser.Avatar;
                            item.last_connected = newUser.last_connected;
                            item.Connections.AddRange(newUser.Connections);
                            existed = true;
                            break;
                        }
                    }
                }

                if (!existed)
                    //Add new user
                    listUser.Add(newUser);

                //Save users
                _myCache.Set(_onlineUsersCacheKey, listUser, _cacheExpiredTime);
            }
            catch (Exception ex)
            {
                strError = string.Format("Could not AddUserToCache because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return listUser;
        }

        public static void StorageListUsersToCache(List<Connector> listUsers)
        {
            var strError = string.Empty;
            try
            {
                _myCache = GlobalContainer.IocContainer.Resolve<ICacheProvider>();

                //Save to cache
                _myCache.Set(_onlineUsersCacheKey, listUsers, _cacheExpiredTime);
            }
            catch (Exception ex)
            {
                strError = string.Format("Could not StorageListUsersToCache because: {0}", ex.ToString());
                logger.Error(strError);
            }
        }
    }
}