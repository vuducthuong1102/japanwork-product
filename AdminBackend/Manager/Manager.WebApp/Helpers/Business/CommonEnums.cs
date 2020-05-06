using Manager.WebApp.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Web;

namespace Manager.WebApp.Helpers
{
    public enum EnumStatus
    {
        [LocalizedDescription("LB_ACTIVE", typeof(ManagerResource))]
        Activated = 1,

        [LocalizedDescription("LB_LOCKED", typeof(ManagerResource))]
        Locked = 0
    }

    public enum EnumProjectStatus
    {
        [LocalizedDescription("LB_LOCKED", typeof(ManagerResource))]
        Locked = 0,

        [LocalizedDescription("LB_ACTIVE", typeof(ManagerResource))]
        Activated = 1
    }

    public enum EnumWarehouseActivityType
    {
        [LocalizedDescription("LB_GOODS_RECEIPT", typeof(ManagerResource))]
        GoodsReceipt = 1,

        [LocalizedDescription("LB_GOODS_ISSUE", typeof(ManagerResource))]
        GoodsIssue = 2,

        [LocalizedDescription("LB_REFLECT_STOCK_TAKE", typeof(ManagerResource))]
        ReflectStockTake = 3
    }

    public enum EnumSlideType
    {
        [Description("Tĩnh")]
        Static = 1,

        [Description("Linh hoạt")]
        Dynamic = 2
    }

    public enum EnumLinkActionClick
    {
        [Description("Mở tab mới")]
        Blank = 1,

        [Description("Mở trực tiếp")]
        Self = 2
    }

    public enum EnumPostType
    {
        [LocalizedDescription("LB_ARTICLE", typeof(ManagerResource))]
        Article = 1,

        [LocalizedDescription("LB_TOP_BANNER", typeof(ManagerResource))]
        TopBanner = 2,

        [LocalizedDescription("LB_PRODUCT_SERVICE", typeof(ManagerResource))]
        WhatWeDo = 3,

        [LocalizedDescription("LB_NEW_ACTIVITY", typeof(ManagerResource))]
        HowWeWork = 4,

        //[LocalizedDescription("LB_ABOUT_US", typeof(ManagerResource))]
        //AboutUs = 5,

        //[LocalizedDescription("LB_TESIMONIAL", typeof(ManagerResource))]
        //Tesimonial = 6
    }

    public enum EnumShopCategory
    {
        SmallShop = 1,

        Agency = 2
    }

    public enum EnumBarCode
    {
        [Description("UNDEFINED")]
        UNDEFINED = 0,

        [Description("BT_SCAN_CODE_ANY")]
        ANY = 999,

        [Description("BT_SCAN_CODE_OCR")]
        ORC = 99,

        [Description("BT_SCAN_CODE_COMPOSI")]
        COMPOSITE = 16,

        [Description("BT_SCAN_CODE_MC")]
        MC = 14,

        [Description("BT_SCAN_CODE_DM")]
        DM = 13,

        [Description("BT_SCAN_CODE_PDF")]
        PDF = 12,

        [Description("BT_SCAN_CODE_QR")]
        QR = 11,

        [Description("BT_SCAN_CODE_GS1_DB")]
        GS1_DB = 10,

        [Description("BT_SCAN_CODE_COOP")]
        COOP = 9,

        [Description("BT_SCAN_CODE_TOF")]
        Composite = 8,

        [Description("BT_SCAN_CODE_C93")]
        C93 = 7,

        [Description("BT_SCAN_CODE_NW7")]
        NW7 = 6,

        [Description("BT_SCAN_CODE_ITF")]
        ITF = 5,

        [Description("BT_SCAN_CODE_C128")]
        C128 = 4,

        [Description("BT_SCAN_CODE_GS1_128")]
        GS1_128 = 3,

        [Description("BT_SCAN_CODE_C39")]
        C39 = 2,

        [Description("BT_SCAN_CODE_JAN")]
        JAN = 1
    }


    public static class EnumCommonCode
    {
        public static int Success = 1;
        public static int Error = -1;
        public static int Error_Info_NotFound = -2;
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
        public static string CompanySizes = "COMPANY_SIZES";
        public static string SalaryTypes = "SALARY_TYPES";
        public static string Qualifications = "QUALIFICATIONS";
        public static string Majors = "MAJORS";
        public static string Fields = "FIELDS";
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
        public static string Footer = "FOOTER_{0}";

        //Childrens
        public static string PrefecturesByRegion = "PREFECTURES_{0}";
        public static string CitiesByPrefecture = "CITIES_{0}";
        public static string TrainLinesByPrefecture = "TRAIN_LINES_PREFECTURE_{0}";
        public static string CompaniesByAgency = "COMPANIES_{0}";
        public static string JobsByCompany = "JOBS_{0}";
    }

    public enum EnumJobStatus
    {
        [LocalizedDescription("LB_AWAITING_APPROVAL", typeof(ManagerResource))]
        Draft = 0,

        [LocalizedDescription("LB_ACTIVE", typeof(ManagerResource))]
        Published = 1,

        [LocalizedDescription("LB_CLOSED", typeof(ManagerResource))]
        Closed = 2,

        [LocalizedDescription("LB_SAVED", typeof(ManagerResource))]
        Saved = 3,

        [LocalizedDescription("LB_CANCELED", typeof(ManagerResource))]
        Canceled = 5
    }

    public enum EnumTranslateStatus
    {
        [LocalizedDescription("LB_NOT_TRANSLATED_YET", typeof(ManagerResource))]
        NotTranslatedYet = 0,

        [LocalizedDescription("LB_TRANSLATED", typeof(ManagerResource))]
        Translated = 1
    }
    public enum EnumFormCv
    {
        [LocalizedDescription("LB_OFFICIAL", typeof(ManagerResource))]

        Official = 0,

        [LocalizedDescription("LB_OVERTIME", typeof(ManagerResource))]
        Overtime = 2,
    }
    public enum EnumEducationStatus
    {
        [LocalizedDescription("EDUCATION_STATUS_STUDYING", typeof(ManagerResource))]
        Studying = 0,

        [LocalizedDescription("EDUCATION_STATUS_GRADUATED", typeof(ManagerResource))]
        Graduated = 1,

        [LocalizedDescription("EDUCATION_STATUS_FLUNKED_OUT", typeof(ManagerResource))]
        FlunkedOut = -1
    }

    public enum EnumCertificateStatus
    {
        [LocalizedDescription("CERTIFICATE_STATUS_NOT_PASSED", typeof(ManagerResource))]
        NotPassed = 0,

        [LocalizedDescription("CERTIFICATE_STATUS_PASSED", typeof(ManagerResource))]
        Passed = 1
    }

    public enum EnumWorkStatus
    {
        [LocalizedDescription("WORK_STATUS_WORKING", typeof(ManagerResource))]
        Working = 0,

        [LocalizedDescription("WORK_STATUS_RESIGN", typeof(ManagerResource))]
        Resign = -1
    }

    public enum EnumCountry
    {
        //[LocalizedDescription("JOB_STATUS_DRAFT", typeof(ManagerResource))]
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

    public enum EnumEmploymentCalculateBy
    {
        //[LocalizedDescription("JOB_STATUS_DRAFT", typeof(ManagerResource))]
        Month = 0,

        //[LocalizedDescription("JOB_STATUS_PUBLISHED", typeof(ManagerResource))]
        Hour = 1
    }

    public enum EnumCvCreationMethod
    {
        //[LocalizedDescription("JOB_STATUS_DRAFT", typeof(ManagerResource))]
        CreateNew = 0,

        //[LocalizedDescription("JOB_STATUS_PUBLISHED", typeof(ManagerResource))]
        Clone = 1
    }

    public static class EnumMessageTypes
    {
        public static int Text = 1;
        public static int Image = 2;
        public static int File = 3;
    }

    public enum EnumMessengerObjectType
    {
        JobSeeker = 0,
        Agency = 1,
        Admin = 2
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