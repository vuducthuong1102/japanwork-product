using MySite.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySite.Helpers.Resource
{
    public class ApiResourceHelper
    {
        public static  string GetMessageByCode(int code)
        {
            if (code == EnumCommonCode.Error_UserOrTokenNotFound)
            {
                return UserWebResource.COMMON_ERROR_USERTOKEN_NOTFOUND;
            }            

            return string.Empty;
        }
    }

    public class WebResourceHelper
    {
        public static string GetMessageByCode(int code)
        {
            if (code == EnumCommonCode.Error_UserOrTokenNotFound)
            {
                return UserWebResource.COMMON_ERROR_USERTOKEN_NOTFOUND;
            }

            return string.Empty;
        }
    }
}