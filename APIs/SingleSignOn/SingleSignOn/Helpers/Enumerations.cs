namespace SingleSignOn.Helpers
{
    public static class EnumApiStatusCode
    {
        public static int OK = 200;
        public static int Created = 201;
        public static int BadRequest = 400;
        public static int Unauthorized = 401;
        public static int Forbidden = 403;
        public static int NotFound = 404;
        public static int MethodNotAllowed = 405;
        public static int Conflict = 409;
        public static int InternalServerError = 500;
        public static int ServiceUnavailable = 503;
    }

    public static class EnumCommonCode
    {
        public static int Success = 200;
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
        public static int Error_InactiveAccount = 113;
    }

    public static class EnumLoginStatus
    {
        public static int LoginErrorUserInfoIncorrect = 201;
    }

    public static class EnumCreateAndVerifyOTPCodeStatus
    {
        public static int Error_OTPCodeNotExistsOrUsed = 301;
        public static int Error_OTPCodeExpired = 302;
        public static int Error_Pwd2NotCorrect = 303;
    }

    public static class EnumPostActionCode
    {
        public static int Error_NotFoundAction = 404;
    }
}