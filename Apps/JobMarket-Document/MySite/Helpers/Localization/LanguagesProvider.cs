using MySite.Caching;
using MySite.Resources;
using System;
using System.Collections.Generic;

namespace MySite
{
    public class LanguagesProvider
    {
        public static string _defaultLang = "vi-VN";

        //public static readonly List<string> _supportedLanguages = new List<string> { "vi-VN", "en-US" };

        public static List<Languages> GetListLanguages()
        {
            return new List<Languages> {                
                new Languages
                {
                    LanguageFullName = UserWebResource.LB_LANG_JAPANESE, LanguageCultureName = "ja-JP"
                },
                new Languages {
                    LanguageFullName = UserWebResource.LB_LANG_VIETNAMESE, LanguageCultureName = "vi-VN", Icon = ""
                },
                //new Languages {
                //    LanguageFullName = UserWebResource.LB_LANG_ENGLISH, LanguageCultureName = "en-US", Icon = ""
                //}
            };
        }

        public class Languages
        {
            public string LanguageFullName
            {
                get;
                set;
            }
            public string LanguageCultureName
            {
                get;
                set;
            }

            public string Icon
            {
                get;
                set;
            }
        }

        public static string GetGoogleMapLanguage()
        {
            var paramsFormat = "&language={0}&region={1}";
            try
            {
                var currentLanguage = UserCookieManager.GetCurrentLanguageOrDefault();
                var arr = currentLanguage.Split('-');
                if (arr.Length > 0)
                {
                    paramsFormat = string.Format(paramsFormat, arr[0], arr[1]);
                }
            }
            catch
            {
                paramsFormat = "&language=en&region=US";
            }

            return paramsFormat;
        }
    }
}