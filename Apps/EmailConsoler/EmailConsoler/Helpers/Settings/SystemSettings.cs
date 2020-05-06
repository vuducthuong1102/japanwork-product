using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;

namespace EmailConsoler.Helpers
{
    public class SystemSettings
    {
        #region Email Configs

        public static string EmailSender
        {
            get
            {
                return ConfigurationManager.AppSettings["System:Email_Sender"];
            }
        }

        public static string EmailSenderPwd
        {
            get
            {
                return ConfigurationManager.AppSettings["System:Email_SenderPwd"];
            }
        }

        public static string EmailHost
        {
            get
            {
                return ConfigurationManager.AppSettings["System:Email_Host"];
            }
        }

        public static string EmailServerPort
        {
            get
            {
                return ConfigurationManager.AppSettings["System:Email_ServerPort"];
            }
        }

        public static bool EmailIsUseSSL
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["System:Email_IsUseSSL"]);
            }
        }

        public static string Email_ActiveLink
        {
            get
            {
                return ConfigurationManager.AppSettings["System:Email_ActiveLink"].ToString();
            }
        }

        public static string Email_AccepFriendInvitationLink
        {
            get
            {
                return ConfigurationManager.AppSettings["System:Email_AccepFriendInvitationLink"].ToString();
            }
        }

        #endregion

        public static double ExternalServiceTimeout
        {
            get
            {
                return Convert.ToDouble(ConfigurationManager.AppSettings["System:ExternalServiceTimeout"]);
            }
        }

        public static int WorkerMetadataBuilderSpeedMS
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["System:MetadataBuilderSpeedMS"]);
            }
        }

        public static int WorkerFileCreatorSpeedMS
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["System:FileCreatorSpeedMS"]);
            }
        }

        public static int WorkerFileSenderSpeedMS
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["System:FileSenderSpeedMS"]);
            }
        }

        public static bool StorageFilesInSameFolder
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["System:StorageInSameFolder"]);
            }
        }        

        public static string XMLStorageFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["System:XMLStorageFolder"];
            }
        }

        public static string EmailSentFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["System:EmailSentFolder"];
            }
        }

        public static string EmailToSendFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["System:EmailToSendFolder"];
            }
        }

        public static string EmailFailedFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["System:EmailFailedFolder"];
            }
        }

        public static bool DeleteFileAfterSent
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["System:DeleteFileAfterSent"]);
            }
        }

        public static bool EnableFtpLogging
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["System:EnableFtpLogging"]);
            }
        }

        public static string ImagePath
        {
            get
            {
                return ConfigurationManager.AppSettings["System:ImagePath"];
            }
        }
    }

    public class FtpSettings
    {
        public static string FtpServer
        {
            get
            {
                return ConfigurationManager.AppSettings["System:FtpServer"];
            }
        }

        public static string FtpUserName
        {
            get
            {
                return ConfigurationManager.AppSettings["System:FtpUserName"];
            }
        }

        public static string FtpPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["System:FtpPassword"];
            }
        }

        public static bool UsingProxy
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["System:UsingProxy"]);
            }
        }        
    }    

    public class NotificationSettings
    {
        public static string SendNotifUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["Notification:SendNotifUrl"];
            }
        }
    }
}