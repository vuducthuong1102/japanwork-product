using SingleSignOn.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SingleSignOn.Helpers.Resource
{
    public class ApiResourceHelper
    {
        public static  string GetMessageByCode(int code)
        {
            if (code == EnumCommonCode.Error_UserOrTokenNotFound)
            {
                return UserApiResource.COMMON_ERROR_USERTOKEN_NOTFOUND;
            }
            else if (code == EnumPostActionCode.Error_NotFoundAction)
            {
                return UserApiResource.COMMON_ERROR_POST_ACTION_NOTFOUND;
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