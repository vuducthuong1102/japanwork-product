using System;
using System.Collections.Generic;

namespace Manager.WebApp.Models
{
    //public class FileUploadResponseModel
    //{
    //    public string FilePath { get; set; }
    //    public string Cover { get; set; }
    //}

    public class FileUploadResponseModel
    {
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public string Path { get; set; }
    }

    public class FilesDeleteModel
    {
        public List<string> FilesPath { get; set; }
    }
}