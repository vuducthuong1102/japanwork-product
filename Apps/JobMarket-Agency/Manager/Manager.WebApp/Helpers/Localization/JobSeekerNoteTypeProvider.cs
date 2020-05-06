using Manager.WebApp.Caching;
using Manager.WebApp.Resources;
using System.Collections.Generic;

namespace Manager.WebApp
{
    public class JobSeekerNoteTypeProvider
    {
        public static string _defaultLang = "vi-VN";

        //public static readonly List<string> _supportedJobSeekerNoteType = new List<string> { "vi-VN", "en-US" };

        public static List<JobSeekerNoteType> GetListJobSeekerNoteType()
        {
            return new List<JobSeekerNoteType> {
                //new JobSeekerNoteType {
                //    LanguageFullName = ManagerResource.LB_LANG_ENGLISH, LanguageCultureName = "en-US", Icon = ""
                //},
                new JobSeekerNoteType {
                    TypeName = ManagerResource.LB_JOBSEEKER_NOTE_EMAIL, LanguageCultureName = "vi-VN", id = 1
                },
                new JobSeekerNoteType {
                    TypeName = ManagerResource.LB_JOBSEEKER_NOTE_PHONE, LanguageCultureName = "ja-JP", id = 2
                },
                new JobSeekerNoteType {
                    TypeName = ManagerResource.LB_JOBSEEKER_NOTE_DISCUSSION, LanguageCultureName = "ja-JP", id = 3
                }
            };
        }

        public class JobSeekerNoteType
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