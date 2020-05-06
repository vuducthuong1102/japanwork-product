using System.Collections.Generic;

namespace SingleSignOn
{
    public class LanguagesProvider
    {
        public static string _defaultLang = "vi-VN";

        //public static readonly List<string> _supportedLanguages = new List<string> { "vi-VN", "en-US" };

        public static List<Languages> GetListLanguages()
        {
            return new List<Languages> {
                new Languages {
                    LanguageFullName = "English", LanguageCultureName = "en-US"
                },
                //new Languages
                //{
                //    LanguageFullName = "French", LanguageCultureName = "fr-FR"
                //},
                new Languages
                {
                    LanguageFullName = "Japanese", LanguageCultureName = "ja-JP"
                },
                new Languages {
                    LanguageFullName = "VietNam", LanguageCultureName = "vi-VN"
                },
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
        }
    }
}