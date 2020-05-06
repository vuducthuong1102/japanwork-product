using Manager.SharedLibs.Logging;
using MsSql.AspNet.Identity;
using System;
using Manager.SharedLibs.Caching.Providers;
using Autofac;
using System.Web;
using Microsoft.AspNet.Identity;

namespace Manager.WebApp.Helpers
{
    public class AccountHelper
    {
        private static readonly ILog logger = LogProvider.For<AccountHelper>();

        public static IdentityUser GetCurrentUser()
        {
            try
            {
                var userId = HttpContext.Current.User.Identity.GetUserId();
                if (!string.IsNullOrEmpty(userId))
                    return AccountHelper.GetUserById(userId);

                return null;
            }
            catch (Exception ex)
            {
                logger.Error("Could not GetCurrentUser: " + ex.ToString());
                return null;
            }
        }

        public static IdentityUser GetUserById(string userId)
        {
            var myKey = string.Format("{0}_{1}", "USER", userId);
            IdentityUser info = null;

            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();

                cacheProvider.Get<IdentityUser>(myKey, out info);

                if (info == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IIdentityStore>();
                    info = myStore.GetUserByID(userId);

                    //Storage to cache
                    if (info != null)
                        cacheProvider.Set(myKey, info);
                }

            }
            catch (Exception ex)
            {
                logger.Error("Could not GetUserById: " + ex.ToString());
            }

            return info;
        }

        public static IdentityUser GetByStaffId(int staffId)
        {
            var myKey = string.Format("{0}_{1}", "USER", staffId);
            IdentityUser info = null;

            try
            {
                //Check from cache first
                var cacheProvider = GlobalContainer.IocContainer.Resolve<ICacheProvider>();

                cacheProvider.Get<IdentityUser>(myKey, out info);

                if (info == null)
                {
                    var myStore = GlobalContainer.IocContainer.Resolve<IIdentityStore>();
                    info = myStore.GetByStaffId(staffId);

                    //Storage to cache
                    if (info != null)
                        cacheProvider.Set(myKey, info);
                }

            }
            catch (Exception ex)
            {
                logger.Error("Could not GetByStaffId: " + ex.ToString());
            }

            return info;
        }

        public static bool CurrentUserIsAdmin()
        {
            try
            {
                var currentUserId = HttpContext.Current.User.Identity.GetUserId();
                var currentUser = GetUserById(currentUserId);
                if (currentUser != null)
                {
                    var currentUserName = currentUser.UserName.ToLower();
                    if (currentUser.UserName == "admin" || currentUser.UserName == "bangvl")
                        return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not check CurrentUserIsAdmin: " + ex.ToString());
            }

            return false;
        }
        public static bool CurrentUserIsAgency()
        {
            try
            {
                var currentUserId = HttpContext.Current.User.Identity.GetUserId();
                var currentUser = GetUserById(currentUserId);
                if (currentUser != null)
                {
                    if (currentUser.ParentId == 0)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not check CurrentUserIsAgency: " + ex.ToString());
            }

            return false;
        }
    }
}