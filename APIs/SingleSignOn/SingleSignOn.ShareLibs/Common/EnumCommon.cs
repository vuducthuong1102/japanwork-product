using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleSignOn.ShareLibs.Common
{
    public static class EnumCommon
    {
        public enum EnumStatus
        {
            KhongKhaDung = 0,
            DangHoatDong = 1,
            XoaLogic = 9
        }
        public enum EnumPostAction
        {
            LIKE,
            SHARE,
            REPORT,
            RATINGSCORE
        }

        public enum EnumCode
        {
            Success = 1,

            Error = -1,

            [Description("COMMON_INFO_NOTFOUND")]
            Error_Info_NotFound = -2,


            [Description("COMMON_ERROR_HASH_INCORRECT")]
            ErrorHashing = 101,

            [Description("COMMON_ERROR_TIME_INVALID")]
            ErrorTimeInvalid = 102,


            [Description("value 1")]
            Error_UserOrTokenNotFound = 103,

            [Description("REGISTER_ERROR_USERNAME_USED")]
            Error_UserNameAlreadyUsed = 104,

            [Description("REGISTER_ERROR_EMAIL_USED")]
            Error_EmailAlreadyUsed = 105,

            [Description("REGISTER_ERROR_PHONE_USED")]
            Error_PhoneAlreadyUsed = 106,

            [Description("REGISTER_ERROR_IDCARD_USED")]
            Error_IDCardAlreadyUsed = 107,

            [Description("COMMON_ERROR_PWDTYPE_INVALID")]
            Error_PwdTypeInvalid = 108,

            [Description("COMMON_ERROR_OLDPWD1_NOTCORRECT")]
            Error_OldPwd1NotCorrect = 109,

            [Description("COMMON_ERROR_OLDPWD2_NOTCORRECT")]
            Error_OldPwd2NotCorrect = 110,

            [Description("COMMON_ERROR_OLDPWD_EQUAL_NEWPWD")]
            Error_NewPwdEqualOldPwd = 111,

            [Description("COMMON_ERROR_RECOVERPWD_METHOD_INVALID")]
            Error_RecoverPwdMethod_Invalid = 112
        }

        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            string description = null;

            if (e is Enum)
            {
                Type type = e.GetType();
                Array values = System.Enum.GetValues(type);

                foreach (int val in values)
                {
                    if (val == e.ToInt32(CultureInfo.InvariantCulture))
                    {
                        var memInfo = type.GetMember(type.GetEnumName(val));
                        var descriptionAttributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                        if (descriptionAttributes.Length > 0)
                        {
                            // we're only getting the first description we find
                            // others will be ignored
                            description = ((DescriptionAttribute)descriptionAttributes[0]).Description;
                        }

                        break;
                    }
                }
            }

            return description;
        }
    }

    //public class PostAction
    //{
    //    public static string Like = "LIKE";

    //    public static string Share = "SHARE";

    //    public static string Report = "REPORT";

    //    public static string RatingScore = "RATINGSCORE";
    //}

}
