using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailConsoler.Models
{
    public class Log4netFile
    {
        public Log4netFile()
        {
            DateCreated = DateTime.Now;
            LastUpdated = DateTime.Now;
            FileId = (Guid.NewGuid()).ToString();
        }

        public string FileId { get; set; }

        public string FolderPath { get; set; }

        public string FileName { get; set; }

        public string FullPath { get; set; }

        public DateTime DateCreated { get; set; }

        public int CurrentItem { get; set; }

        public int FileStatus { get; set; }

        public string MachineName { get; set; }

        public string MachineIp { get; set; }

        public DateTime LastUpdated { get; set; }

        public string Comments { get; set; }

        public string AppName { get; set; }
    }
}
