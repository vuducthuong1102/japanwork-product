using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace ApiJobMarket.Helpers
{
    public static class UrlFriendlyHelpers
    {
        //public static void BindPostLink(ref IdentityPost post)
        //{
        //    post.PostLink = GetFriendlyUrl(post);
        //}

        //public static void BindPostLink(ref List<IdentityPost> ListPost)
        //{
        //    if(ListPost != null && ListPost.Count > 0)
        //    {
        //        foreach (var item in ListPost)
        //        {
        //            item.PostLink = SystemSettings.FrontendURL + GetFriendlyUrl(item);
        //        }
        //    }
        //}

        //public static void BindImageLink(ref IdentityPost post)
        //{
        //    if (post != null)
        //    {
        //        if (post.Images != null && post.Images.Count > 0)
        //        {
        //            foreach (var img in post.Images)
        //            {
        //                img.Url = CdnHelper.SocialGetFullImgPath(img.Url);
        //            }
        //        }
        //    }
        //}

        //public static void BindImageLink(ref List<IdentityPost> listPost)
        //{
        //    if (listPost != null && listPost.Count > 0)
        //    {
        //        foreach (var item in listPost)
        //        {
        //            if (item.Images != null && item.Images.Count > 0)
        //            {
        //                foreach (var img in item.Images)
        //                {
        //                    img.Url = CdnHelper.SocialGetFullImgPath(img.Url);
        //                }
        //            }
        //        }
        //    }
        //}

        //public static void BindImageLink(ref List<IdentityImage> listImage)
        //{
        //    if (listImage != null && listImage.Count > 0)
        //    {
        //        foreach (var item in listImage)
        //        {
        //            item.Url = CdnHelper.SocialGetFullImgPath(item.Url);
        //        }
        //    }
        //}

        #region helpers

        private static string Encrypt(string inputText, DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMdd") + inputText;
        }

        //public static string GetFriendlyUrl(IdentityPost post)
        //{
        //    var urlFormat = "{0}{1}.{2}";
        //    var url = string.Empty;
        //    if (post != null)
        //    {
        //        url = string.Format(urlFormat, "/post/u/", ConvertToUrlFriendly(post.Title), Encrypt(post.Id.ToString(), post.CreatedDate));
        //    }

        //    return url;
        //}


        private static readonly string[] VietnameseSigns = new string[]

       {

        "aAeEoOuUiIdDyY",

        "áàạảãâấầậẩẫăắằặẳẵ",

        "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

        "éèẹẻẽêếềệểễ",

        "ÉÈẸẺẼÊẾỀỆỂỄ",

        "óòọỏõôốồộổỗơớờợởỡ",

        "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

        "úùụủũưứừựửữ",

        "ÚÙỤỦŨƯỨỪỰỬỮ",

        "íìịỉĩ",

        "ÍÌỊỈĨ",

        "đ",

        "Đ",

        "ýỳỵỷỹ",

        "ÝỲỴỶỸ"

       };

        public static string ConvertToUrlFriendly(this string str)
        {
            //Tiến hành thay thế , lọc bỏ dấu cho chuỗi
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }

            //str = str.Replace(" ", "-").Replace(",", "-").Replace(".", "-").Replace("–","-").Replace("&","-").ToLower();
            //while (str.Contains("--"))
            //{
            //    str = str.Replace("--", "-");
            //}

            str = URLFriendly(str);

            if (string.IsNullOrEmpty(str))
            {
                str = "invalid";
            }

            return str;
        }

        public static string URLFriendly(string title)
        {
            if (title == null) return "";

            const int maxlen = 80;
            int len = title.Length;
            bool prevdash = false;
            var sb = new StringBuilder(len);
            char c;

            for (int i = 0; i < len; i++)
            {
                c = title[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    prevdash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    // tricky way to convert to lowercase
                    sb.Append((char)(c | 32));
                    prevdash = false;
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' ||
                    c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevdash && sb.Length > 0)
                    {
                        sb.Append('-');
                        prevdash = true;
                    }
                }
                else if ((int)c >= 128)
                {
                    int prevlen = sb.Length;
                    sb.Append(RemapInternationalCharToAscii(c));
                    if (prevlen != sb.Length) prevdash = false;
                }
                if (i == maxlen) break;
            }

            if (prevdash)
                return sb.ToString().Substring(0, sb.Length - 1);
            else
                return sb.ToString();
        }

        public static string RemapInternationalCharToAscii(char c)
        {
            string s = c.ToString().ToLowerInvariant();
            if ("àåáâäãåą".Contains(s))
            {
                return "a";
            }
            else if ("èéêëę".Contains(s))
            {
                return "e";
            }
            else if ("ìíîïı".Contains(s))
            {
                return "i";
            }
            else if ("òóôõöøőð".Contains(s))
            {
                return "o";
            }
            else if ("ùúûüŭů".Contains(s))
            {
                return "u";
            }
            else if ("çćčĉ".Contains(s))
            {
                return "c";
            }
            else if ("żźž".Contains(s))
            {
                return "z";
            }
            else if ("śşšŝ".Contains(s))
            {
                return "s";
            }
            else if ("ñń".Contains(s))
            {
                return "n";
            }
            else if ("ýÿ".Contains(s))
            {
                return "y";
            }
            else if ("ğĝ".Contains(s))
            {
                return "g";
            }
            else if (c == 'ř')
            {
                return "r";
            }
            else if (c == 'ł')
            {
                return "l";
            }
            else if (c == 'đ')
            {
                return "d";
            }
            else if (c == 'ß')
            {
                return "ss";
            }
            else if (c == 'Þ')
            {
                return "th";
            }
            else if (c == 'ĥ')
            {
                return "h";
            }
            else if (c == 'ĵ')
            {
                return "j";
            }
            else
            {
                return "";
            }
        }
        #endregion

        public static string GetBaseUrl()
        {
            var request = HttpContext.Current.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl != "/")
                appUrl = "/" + appUrl;

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            return baseUrl;
        }
    }
}