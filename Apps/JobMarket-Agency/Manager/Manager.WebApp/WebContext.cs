using System;
using System.Web;
using Autofac;
using Manager.WebApp.Settings;
using Manager.WebApp.Services;
using MsSql.AspNet.Identity.Entities;
using System.Collections.Generic;

namespace Manager.WebApp
{
    /// <summary>
    /// Working with data in HttpContext.Current
    /// </summary>
    public static class WebContext
    {
        #region SiteMap/Menu

        #endregion

        #region System Settings

        public static SiteSettings GetSiteSettings()
        {
            return GetSiteSettings(true);
        }


        public static SiteSettings GetSiteSettings(bool useHttpContext)
        {
            if (useHttpContext)
            {
                return GetSiteSettingsFromContext();
            }
            else
            {
                return GetSiteSettingsFromCache();
            }
        }


        private static SiteSettings GetSiteSettingsFromContext()
        {
            if (HttpContext.Current == null) return GetSiteSettingsFromCache();

            string contextKey = "SiteSettings";

            SiteSettings siteSettings = HttpContext.Current.Items[contextKey] as SiteSettings;
            if (siteSettings == null)
            {
                siteSettings = GetSiteSettingsFromCache();
                if (siteSettings != null)
                    HttpContext.Current.Items[contextKey] = siteSettings;
            }
            return siteSettings;
        }

        private static Settings.SiteSettings GetSiteSettingsFromCache()
        {                        
            try
            {
                ISettingsService settingService = GlobalContainer.IocContainer.Resolve<ISettingsService>();
                SiteSettings siteSettings = new SiteSettings();
                siteSettings.Load(settingService, true);                                
                return siteSettings;
            }
            catch (Exception ex)
            {                
                return new SiteSettings();
            }

        }

        #endregion
    }

    /// <summary>
    /// Working with data in HttpContext.Current
    /// </summary>
    public static class FrontEndWebContext
    {
        #region System Settings

        public static SiteFrontEndSettings GetSiteSettings()
        {
            return GetSiteSettings(true);
        }


        public static SiteFrontEndSettings GetSiteSettings(bool useHttpContext)
        {
            if (useHttpContext)
            {
                return GetSiteSettingsFromContext();
            }
            else
            {
                return GetSiteSettingsFromCache();
            }
        }


        private static SiteFrontEndSettings GetSiteSettingsFromContext()
        {
            if (HttpContext.Current == null) return GetSiteSettingsFromCache();

            string contextKey = "SiteFrontEndSettings";

            SiteFrontEndSettings siteSettings = HttpContext.Current.Items[contextKey] as SiteFrontEndSettings;
            if (siteSettings == null)
            {
                siteSettings = GetSiteSettingsFromCache();
                if (siteSettings != null)
                    HttpContext.Current.Items[contextKey] = siteSettings;
            }
            return siteSettings;
        }

        private static Settings.SiteFrontEndSettings GetSiteSettingsFromCache()
        {
            try
            {
                IFrontEndSettingsService settingService = GlobalContainer.IocContainer.Resolve<IFrontEndSettingsService>();
                SiteFrontEndSettings siteSettings = new SiteFrontEndSettings();
                siteSettings.Load(settingService, true);
                return siteSettings;
            }
            catch (Exception ex)
            {
                return new SiteFrontEndSettings();
            }

        }

        #endregion
    }
}
