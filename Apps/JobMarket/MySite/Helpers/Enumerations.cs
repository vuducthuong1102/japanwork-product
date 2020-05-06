using MySite.Resources;
using System;
using System.ComponentModel;
using System.Resources;

namespace MySite.Helpers
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
        public static int Error_Account_Inactive = 113;

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

    public static class EnumStatus
    {
        public static int NoActive = 0;

        public static int Active = 1;

        public static int Block = 7;

        public static int RemoveLogic = 9;
    }

    public static class EnumFormatDate
    {
        public static string DDMMYYYHHMM = "dd/MM/yyyy HH:mm";
    }

    public static class EnumActionType
    {
        public static string COMMENT = "COMMENT";
        public static string COMMENT_REPLY = "COMMENT_REPLY";
    }

    public static class EnumMessageTypes
    {
        public static int Text = 1;
        public static int Image = 2;
        public static int File = 3;
    }

    public static class EnumListCacheKeys
    {
        public static string Regions = "REGIONS";
        public static string Cities = "CITIES";
        public static string Prefectures = "PREFECTURES";
        public static string Stations = "STATIONS";
        public static string TrainLines = "TRAIN_LINES";
        public static string EmploymentTypes = "EMPLOYMENT_TYPES";
        public static string JapaneseLevels = "JAPANESE_LEVELS";
        public static string SalaryTypes = "SALARY_TYPES";
        public static string Qualifications = "QUALIFICATIONS";
        public static string Majors = "MAJORS";
        public static string Fields = "FIELDS";
        public static string FieldCounts = "FIELD_COUNTS";
        public static string Industries = "INDUSTRIES";
        public static string Agencies = "AGENCIES";
    }

    public static class EnumFormatInfoCacheKeys
    {
        //Detail info
        public static string Job = "JOB_{0}";
        public static string JobInfoByLang = "JOB_{0}_{1}";
        public static string City = "CITY_{0}";
        public static string Region = "REGION_{0}";
        public static string Station = "STATION_{0}";

        //Childrens
        public static string PrefecturesByRegion = "PREFECTURES_{0}";
        public static string CitiesByPrefecture = "CITIES_{0}";
        public static string TrainLinesByPrefecture = "TRAIN_LINES_PREFECTURE_{0}";
        public static string CompaniesByAgency = "COMPANIES_{0}";
        public static string JobsByCompany = "JOBS_{0}";
    }

    public enum EnumJobStatus
    {
        [LocalizedDescription("JOB_STATUS_DRAFT", typeof(UserWebResource))]
        Draft = 0,

        [LocalizedDescription("JOB_STATUS_PUBLISHED", typeof(UserWebResource))]
        Published = 1,

        [LocalizedDescription("JOB_STATUS_CLOSED", typeof(UserWebResource))]
        Closed = 2
    }

    public enum EnumEducationStatus
    {
        [LocalizedDescription("EDUCATION_STATUS_STUDYING", typeof(UserWebResource))]
        Studying = 0,

        [LocalizedDescription("EDUCATION_STATUS_GRADUATED", typeof(UserWebResource))]
        Graduated = 1,

        [LocalizedDescription("EDUCATION_STATUS_FLUNKED_OUT", typeof(UserWebResource))]
        FlunkedOut = -1
    }

    public enum EnumCertificateStatus
    {
        [LocalizedDescription("CERTIFICATE_STATUS_NOT_PASSED", typeof(UserWebResource))]
        NotPassed = 0,

        [LocalizedDescription("CERTIFICATE_STATUS_PASSED", typeof(UserWebResource))]
        Passed = 1
    }

    public enum EnumWorkStatus
    {
        [LocalizedDescription("WORK_STATUS_WORKING", typeof(UserWebResource))]
        Working = 0,

        [LocalizedDescription("WORK_STATUS_RESIGN", typeof(UserWebResource))]
        Resign = -1
    }

    public enum EnumCountry
    {
        //[LocalizedDescription("JOB_STATUS_DRAFT", typeof(UserWebResource))]
        Japan = 81,

        VietNam = 84
    }

    public enum EnumApplicationStatus
    {
        Applied = 0,

        Interview = 1,

        Ignore = -1,

        Cancelled = -2
    }

    public enum EnumApplicationInvitationStatus
    {
        [LocalizedDescription("INVITATION_STATUS_AWAITING", typeof(UserWebResource))]
        Awating = 0,

        [LocalizedDescription("INVITATION_STATUS_ACCEPTED", typeof(UserWebResource))]
        Accepted = 1,

        [LocalizedDescription("INVITATION_STATUS_IGNORED", typeof(UserWebResource))]
        Ignore = -1,
    }

    public enum EnumEmploymentCalculateBy
    {
        //[LocalizedDescription("JOB_STATUS_DRAFT", typeof(UserWebResource))]
        Month = 0,

        //[LocalizedDescription("JOB_STATUS_PUBLISHED", typeof(UserWebResource))]
        Hour = 1
    }

    public enum EnumCvCreationMethod
    {
        //[LocalizedDescription("JOB_STATUS_DRAFT", typeof(UserWebResource))]
        CreateNew = 0,

        //[LocalizedDescription("JOB_STATUS_PUBLISHED", typeof(UserWebResource))]
        Clone = 1
    }

    public enum EnumCsCreationMethod
    {
        //[LocalizedDescription("JOB_STATUS_DRAFT", typeof(UserWebResource))]
        CreateNew = 0,

        //[LocalizedDescription("JOB_STATUS_PUBLISHED", typeof(UserWebResource))]
        Clone = 1
    }

    public enum EnumNotifActionTypeForJobSeeker
    {
        System = 0,

        #region Application

        Application_Accepted = 1000,

        Application_Rejected = 1001,

        #endregion

        #region Invitation

        Invitation_Received = 2000

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