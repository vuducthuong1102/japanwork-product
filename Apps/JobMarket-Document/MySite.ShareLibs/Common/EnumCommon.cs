
namespace MySite.ShareLibs.Common
{
    public class EnumCommon
    {
        //public enum EnumStatus
        //{
        //    KhongKhaDung=0,
        //    DangHoatDong=1,
        //    XoaLogic=9
        //}
       public enum EnumAction
        {
            LIKE,
            SHARE,
            REPORT,
            RATINGSCORE
        }

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
        public static string NotAcceptFriend = "NOT_ACCEPT_FRIEND";
        public static string Remove_Notif = "REMOVE_NOTIF";
        public static string Remove_Friend = "REMOVE_FRIEND";
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
