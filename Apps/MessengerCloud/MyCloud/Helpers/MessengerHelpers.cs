using Autofac;
using MyCloud.Models;
using MyCloud.Settings;
using MyCloud.SharedLib.Caching.Providers;
using MyCloud.SharedLib.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyCloud.Helpers
{
    public class MessengerHelpers
    {
        private static ICacheProvider _myCache;
        private static ILog logger = LogProvider.For<MessengerHelpers>();
        private static string _allUsersCacheKey = string.Format("{0}{1}", SystemSettings.DefaultCacheKeyPrefix, "MESSENGER_USERS");
        private static int _cacheExpiredTime = 10080; //One week

        public static List<Connector> GetAllUsersFromCache()
        {
            var strError = string.Empty;
            List<Connector> listUser = null;

            try
            {
                _myCache = GlobalContainer.IocContainer.Resolve<ICacheProvider>();

                _myCache.Get(_allUsersCacheKey, out listUser);
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
                        if (newUser.id == item.id && newUser.t == item.t)
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
                _myCache.Set(_allUsersCacheKey, listUser, _cacheExpiredTime);
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
                _myCache.Set(_allUsersCacheKey, listUsers, _cacheExpiredTime);
            }
            catch (Exception ex)
            {
                strError = string.Format("Could not StorageListUsersToCache because: {0}", ex.ToString());
                logger.Error(strError);
            }
        }
    }
}