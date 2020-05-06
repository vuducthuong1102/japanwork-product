using ApiJobMarket.Resources;
using System;
using System.ComponentModel;
using System.Resources;

namespace ApiJobMarket.Helpers
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

    public static class EnumListCacheKeys
    {
        public static string Countries = "COUNTRIES";
        public static string Regions = "REGIONS";
        public static string Cities = "CITIES";
        public static string Prefectures = "PREFECTURES";
        public static string Stations = "STATIONS";
        public static string TrainLines = "TRAIN_LINES";
        public static string EmploymentTypes = "EMPLOYMENT_TYPES";
        public static string JapaneseLevels = "JAPANESE_LEVELS";
        public static string CompanySizes = "COMPANY_SIZES";
        public static string SalaryTypes = "SALARY_TYPES";
        public static string Qualifications = "QUALIFICATIONS";
        public static string Majors = "MAJORS";
        public static string Visas = "VISAS";
        public static string Fields = "FIELDS";
        public static string SubFields = "SUB_FIELDS";
        public static string FieldCounts = "FIELD_COUNT";
        public static string Industries = "INDUSTRIES";
        public static string SubIndustries = "SUB_INDUSTRIES";
        public static string Agencies = "AGENCIES";
        public static string Suggests = "SUGGESTS";
        public static string SalaryFilters = "SALARYFILTERS";
        public static string ProcessStatus = "PROCESSSTATUS";
        public static string TypeSuggests = "TYPESUGGESTS";
        public static string Navigations = "NAVIGATIONS";
    }

    public static class EnumFormatInfoCacheKeys
    {
        //Detail info
        public static string Cv = "CV_{0}";
        public static string Cs = "CS_{0}";
        public static string Job = "JOB_{0}";
        public static string JobSeekerInfo = "JOB_SEEKER_{0}";
        public static string JobSeekerConfig = "JOB_SEEKER_CONFIG_{0}";
        public static string JobSeekerAgencyInfo = "JOB_SEEKER_AGENCY_{0}";
        public static string JobInfoByLang = "JOB_{0}_{1}";
        public static string JobHots = "JOB_HOTS";
        public static string Company = "COMPANY_{0}_";
        public static string Region = "REGION_{0}";
        public static string Prefecture = "PREFECTURE_{0}";
        public static string City = "CITY_{0}";
        public static string Station = "STATION_{0}";
        public static string Agency = "AGENCY_{0}";
        public static string Footer = "FOOTER_{0}";

        //Childrens
        public static string RegionByCountry = "REGIONS_{0}";
        public static string PrefecturesByRegion = "PREFECTURES_{0}";
        public static string CitiesByPrefecture = "CITIES_{0}";
        public static string TrainLinesByPrefecture = "TRAIN_LINES_PREFECTURE_{0}";
        public static string JobsByCompany = "JOBS_{0}";
        public static string MajorsByQualification = "MAJORS_{0}";
        public static string SubsByIndustry = "SUB_INDUSTRIES_{0}";
        public static string SubsByField = "SUB_FIELDS_{0}";
    }

    public enum EnumErrorCode
    {
        #region Common error
        //E000101: "Lỗi khi kết nối đến Server!",
        [LocalizedDescription("ERROR_E000101", typeof(UserApiResource))]
        E000101 = 000101,

        //E000102: "Interal server error",
        [LocalizedDescription("ERROR_E000102", typeof(UserApiResource))]
        E000102 = 000102,

        //E000103: "Error token",
        [LocalizedDescription("ERROR_E000103", typeof(UserApiResource))]
        E000103 = 000103,

        //E000104: "No Permission",
        [LocalizedDescription("ERROR_E000104", typeof(UserApiResource))]
        E000104 = 000104,

        //E000105: "User not found",
        [LocalizedDescription("ERROR_E000105", typeof(UserApiResource))]
        E000105 = 000105,

        //E000106: "Unsuccessfully",
        [LocalizedDescription("ERROR_E000106", typeof(UserApiResource))]
        E000106 = 000106,

        //E000107: "Bad request",
        [LocalizedDescription("ERROR_E000107", typeof(UserApiResource))]
        E000107 = 000107,

        //E000108: "Foreign key does not exist",
        [LocalizedDescription("ERROR_E000108", typeof(UserApiResource))]
        E000108 = 000108,

        //E000109: "Wrong data type or range",
        [LocalizedDescription("ERROR_E000109", typeof(UserApiResource))]
        E000109 = 000109,

        //E000110: "Thiếu điều kiện tìm kiếm",
        [LocalizedDescription("ERROR_E000110", typeof(UserApiResource))]
        E000110 = 000110,

        //E000111: "Lack of data fields",
        [LocalizedDescription("ERROR_E000111", typeof(UserApiResource))]
        E000111 = 000111,

        //E000112: "This data does not exist",
        [LocalizedDescription("ERROR_E000112", typeof(UserApiResource))]
        E000112 = 000112,

        #endregion

        #region Sign in signup error

        #region Sign in
        //E010101: "Email hoặc password không đúng.", // username or password is incorect
        [LocalizedDescription("ERROR_E010101", typeof(UserApiResource))]
        E010101 = 010101,

        //E010102: "Tài khoản chưa được kích hoạt", // Account is not active
        [LocalizedDescription("ERROR_E010102", typeof(UserApiResource))]
        E010102 = 010102,

        //E010103: "Không có quyền admin để đăng nhập", // Not admin to sign in admin page
        [LocalizedDescription("ERROR_E010103", typeof(UserApiResource))]
        E010103 = 010103,

        //E010104: "Không có quyền truy cập email của tài khoản Facebook", // Not allow permissions email with facebook
        [LocalizedDescription("ERROR_E010104", typeof(UserApiResource))]
        E010104 = 010104,

        //E010105: "Không có quyền truy cập email của tài khoản Google", // not allow permissions email with google_plus
        [LocalizedDescription("ERROR_E010105", typeof(UserApiResource))]
        E010105 = 010105,

        #endregion

        #region Sign up
        //E010201: "Email sai định dạng", // Type of email is incorrect
        [LocalizedDescription("ERROR_E010201", typeof(UserApiResource))]
        E010201 = 010201,

        //E010202: "Email đã tồn tại trong hệ thống", // Username already exist
        [LocalizedDescription("ERROR_E010202", typeof(UserApiResource))]
        E010202 = 010202,

        //E010203: "Không thể gửi email kích hoạt", // Email is incorrect to send verification
        [LocalizedDescription("ERROR_E010203", typeof(UserApiResource))]
        E010203 = 010203,

        #endregion

        #region Verify email
        //E010301: "Mã xác thực không đúng vui lòng kiểm tra lại đường dẫn!",
        [LocalizedDescription("ERROR_E010301", typeof(UserApiResource))]
        E010301 = 010301,

        //E010302: "Tài khoản này đã được kích hoạt!", 
        [LocalizedDescription("ERROR_E010302", typeof(UserApiResource))]
        E010302 = 010302,

        //E010303: "Tài khoản của bạn đã được kích hoạt thành công. Vui lòng đăng nhập lại trên ứng dụng", 
        [LocalizedDescription("ERROR_E010303", typeof(UserApiResource))]
        E010303 = 010303,

        #endregion

        #region Change pass
        //E010401: "Bạn đã đổi mật khẩu rồi",
        [LocalizedDescription("ERROR_E010401", typeof(UserApiResource))]
        E010401 = 010401,

        //E010402: "Mã xác thực không đúng vui lòng kiểm tra lại đường dẫn!",
        [LocalizedDescription("ERROR_E010402", typeof(UserApiResource))]
        E010402 = 010402,

        //E010403: "Xác nhận ok",
        [LocalizedDescription("ERROR_E010403", typeof(UserApiResource))]
        E010403 = 010403,

        #endregion

        #region Reset Pass
        //E010501: "Không tồn tại tài khoản này"
        [LocalizedDescription("ERROR_E010501", typeof(UserApiResource))]
        E010501 = 010501,

        //E010502: "Tài khoản này chưa được kích hoạt"
        [LocalizedDescription("ERROR_E010502", typeof(UserApiResource))]
        E010502 = 010502,

        //E010503: "Có lỗi khi gửi email"
        [LocalizedDescription("ERROR_E010503", typeof(UserApiResource))]
        E010503 = 010503,

        //E010504: "Gửi thành công vui lòng kiểm tra lại hộp thư!"
        [LocalizedDescription("ERROR_E010504", typeof(UserApiResource))]
        E010504 = 010504,

        #endregion

        #region Re-verify
        //E010601: "Định dạng email không đúng, vui lòng kiểm tra lại"
        [LocalizedDescription("ERROR_E010601", typeof(UserApiResource))]
        E010601 = 010601,

        //E010602: "Gửi xác thực thành công, vui lòng kiểm tra lại hộp thư!"
        [LocalizedDescription("ERROR_E010602", typeof(UserApiResource))]
        E010602 = 010602,
        #endregion

        #endregion

        #region Admin error
        //E020101: "Mô tả đã tồn tại" // Description already exists
        [LocalizedDescription("ERROR_E020101", typeof(UserApiResource))]
        E020101 = 020101,

        #endregion

        #region CV
        //E030101: "CV không tồn tại"
        [LocalizedDescription("ERROR_E030101", typeof(UserApiResource))]
        E030101 = 030101,

        //E030102: "code_id đã tồn tại",
        [LocalizedDescription("ERROR_E030102", typeof(UserApiResource))]
        E030102 = 030102,

        //"Đã lưu code_id"
        [LocalizedDescription("ERROR_E030103", typeof(UserApiResource))]
        E030103 = 030103,

        //E030104: "Không có code_id"
        [LocalizedDescription("ERROR_E030104", typeof(UserApiResource))]
        E030104 = 030104,

        //E030105: "File ảnh không được vượt quá 50MB"
        [LocalizedDescription("ERROR_E030105", typeof(UserApiResource))]
        E030105 = 030105,

        #endregion

        #region User (Job seeker, user, Agency)

        #region JobSeeker
        //E040101: "user_id hoặc work_status sai"
        [LocalizedDescription("ERROR_E040101", typeof(UserApiResource))]
        E040101 = 040101,

        #endregion

        #endregion

        #region Job
        //E050101: "Công việc này không tồn tại"
        [LocalizedDescription("ERROR_E050101", typeof(UserApiResource))]
        E050101 = 050101,

        //E050102: "Bạn đã ứng tuyển vào công việc này"
        [LocalizedDescription("ERROR_E050102", typeof(UserApiResource))]
        E050102 = 050102,

        //E050103: "Bạn đã lưu công việc này"
        [LocalizedDescription("ERROR_E050103", typeof(UserApiResource))]
        E050103 = 050103,

        #endregion

        #region Company
        //E060101: "Công ty này không tồn tại"
        [LocalizedDescription("ERROR_E060101", typeof(UserApiResource))]
        E060101 = 060101,

        #endregion

        #region Career sheet
        //E070101: "Bảng nghề nghiệp không tồn tại"
        [LocalizedDescription("ERROR_E070101", typeof(UserApiResource))]
        E070101 = 070101

        #endregion
    }

    public enum EnumApplicationStatus
    {

        [LocalizedDescription("APPLICATION_STATUS_ACTIVE", typeof(UserApiResource))]
        Applied = 0,

        [LocalizedDescription("APPLICATION_STATUS_INTERVIEW", typeof(UserApiResource))]
        Interview = 1,

        [LocalizedDescription("APPLICATION_STATUS_IGNORE", typeof(UserApiResource))]
        Ignore = -1,

        [LocalizedDescription("APPLICATION_STATUS_CANCEL", typeof(UserApiResource))]
        Cancelled = -2,
    }

    public enum EnumApplicationInvitationStatus
    {
        [LocalizedDescription("INVITATION_STATUS_AWAITING", typeof(UserWebResource))]
        Awating = 0,

        [LocalizedDescription("INVITATION_STATUS_ACCEPTED", typeof(UserWebResource))]
        Accepted = 1,

        [LocalizedDescription("INVITATION_STATUS_IGNORED", typeof(UserWebResource))]
        Ignore = -1
    }

    public enum EnumJobStatus
    {
        [LocalizedDescription("JOB_STATUS_DRAFT", typeof(UserApiResource))]
        Draft = 0,

        [LocalizedDescription("JOB_STATUS_PUBLISHED", typeof(UserApiResource))]
        Published = 1,

        [LocalizedDescription("JOB_STATUS_CLOSED", typeof(UserApiResource))]
        Closed = 2,

        [LocalizedDescription("JOB_STATUS_SAVED", typeof(UserApiResource))]
        Saved = 3,

       [LocalizedDescription("JOB_STATUS_CANCELED", typeof(UserApiResource))]
        Canceled = 5
    }

    public enum EnumNotifActionTypeForJobSeeker
    {
        System = 0,

        #region Application

        Application_Accepted = 1000,

        Application_Rejected = 1001,

        #endregion

        #region Invitation

        Invitation_Received = 2000,

        Invitation_Accepted = 2001,

        Invitation_Canceled = 2002,
        #endregion
    }

    public enum EnumNotifJob
    {
        #region Public job

        Approval = 3000,

        Accepted = 3001,

        Canceled = 3002

        #endregion
    }
    public enum EnumNotifActionTypeForAgency
    {
        System = 0,

        #region Application

        Application_Apply = 1000,

        Application_Cancel = 1001,

        #endregion

        #region Invitation

        Invitation_Accepted = 2000,

        Invitation_Rejected = 2001

        #endregion
    }

    public enum EnumNotifTargetType
    {
        System = 0,

        JobSeeker = 1,

        Job = 2,

        Cv = 3,

        Agency = 4,

        Application = 5,

        Invitation = 6
    }

    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        private readonly string _resourceKey;
        private readonly ResourceManager _resource;
        public LocalizedDescriptionAttribute(string resourceKey, Type resourceType)
        {
            _resource = new ResourceManager(resourceType);
            _resourceKey = resourceKey;
        }

        public override string Description
        {
            get
            {
                string displayName = _resource.GetString(_resourceKey);

                return string.IsNullOrEmpty(displayName)
                    ? string.Format("[[{0}]]", _resourceKey)
                    : displayName;
            }
        }
    }

    public static class EnumExtensions
    {
        public static string GetEnumDescription(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}