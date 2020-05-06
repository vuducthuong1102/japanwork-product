using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCloud.SharedLib
{
    public static class EnumCommonCode
    {
        public static int Success = 1;
        public static int Error = -1;
        public static int Error_Info_NotFound = -2;

        //For validation
        public static int ErrorHashing = 101;
        public static int ErrorTimeInvalid = 102;

        public static int Error_UserOrTokenNotFound = 103;
        public static int Error_UserNameAlreadyUsed = 104;
        public static int Error_EmailAlreadyUsed = 105;
        public static int Error_PhoneAlreadyUsed = 106;
        public static int Error_IDCardAlreadyUsed = 107;
        public static int Error_PwdTypeInvalid = 108;
        public static int Error_OldPwd1NotCorrect = 109;
        public static int Error_OldPwd2NotCorrect = 110;
        public static int Error_NewPwdEqualOldPwd = 111;
        public static int Error_RecoverPwdMethod_Invalid = 112;

        public static int Error_Action_Not_Found = 113;
    }

    public class CommonAction
    {
        public static string Like = "LIKE";
        public static string UnLike = "UNLIKE";
        public static string Share = "SHARE";
        public static string Report = "REPORT";
        public static string Save = "SAVE";
        public static string UnSave = "UNSAVE";
        public static string RatingScore = "RATINGSCORE";
        public static string Comment = "COMMENT";
        public static string CommentReply = "COMMENT_REPLY";
        public static string Follow = "FOLLOW";

        public static string SendRequestFriend = "SEND_REQUEST_FRIEND";
        public static string AcceptFriend = "ACCEPT_FRIEND";
    }

    public class CommonObject
    {
        public static string Post = "POST";
        public static string Member = "MEMBER";
        public static string Comment = "COMMENT";
        public static string Video = "VIDEO";
        public static string Photo = "PHOTO";
        public static string UserAction = "USER_ACTION";
    }
}
