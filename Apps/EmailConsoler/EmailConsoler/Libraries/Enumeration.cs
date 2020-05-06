namespace EmailConsoler
{
    public enum EnumNotifStatus
    {
        Success = 1,
        Error = -1        
    }

    public static class MessageConstant
    {
        public const string Message_Success_XMLCreated = "XML file has been created successfully.";
        public const string Message_Error_XMLCreation = "An error occurred when generate XML file";

        public const string Message_Success_UpdateStatus = "The VodData status has been updatedd successfully.";
        public const string Message_Error_UpdateStatus = "An error occurred when update VodData status";

        public const string Message_Error_FTPUploadFile = "An error occurred when upload XML file status";


        public const string Label_XMLGenerateBegin = "________ Beginning generate XML files ________";
        public const string Label_XMLGenerateEnd = "________ Generate XML files finished.________";
        public const string Label_UpdatingDataBegin = "________ Beginning update VodData status ________";
        public const string Label_UpdatingDataEnd = "________ Update VodData status finished.________";
        public const string Label_FtpUploadBegin = "________Beginning upload files by FTP________";
        public const string Label_FtpUploadEnd = "________Upload files by FTP finished.________";

        public const string Label_NoDataFound = "There are no data found.";
    }
}