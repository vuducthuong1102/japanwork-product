using Manager.WebApp.Caching;
using Manager.WebApp.Resources;
using System.Collections.Generic;

namespace Manager.WebApp
{
    public class CompanyNoteTypeProvider
    {
        public static string _defaultLang = "vi-VN";

        //public static readonly List<string> _supportedCompanyNoteType = new List<string> { "vi-VN", "en-US" };

        public static List<CompanyNoteType> GetListCompanyNoteType()
        {
            return new List<CompanyNoteType> {
                //new CompanyNoteType {
                //    LanguageFullName = ManagerResource.LB_LANG_ENGLISH, LanguageCultureName = "en-US", Icon = ""
                //},
                 new CompanyNoteType {
                    TypeName = ManagerResource.LB_COMPANY_NOTE_MEETING, LanguageCultureName = "ja-JP", id = 1
                },
                new CompanyNoteType {
                    TypeName = ManagerResource.LB_COMPANY_NOTE_EMAIL, LanguageCultureName = "vi-VN", id = 2
                },
                new CompanyNoteType {
                    TypeName = ManagerResource.LB_COMPANY_NOTE_PHONE, LanguageCultureName = "ja-JP", id = 3
                }
            };
        }

        public class CompanyNoteType
        {
            public string TypeName
            {
                get;
                set;
            }
            public string LanguageCultureName
            {
                get;
                set;
            }

            public int id
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