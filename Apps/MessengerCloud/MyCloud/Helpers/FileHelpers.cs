using MyCloud.SharedLib.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MyCloud.Helpers
{
    public class NotificationFileHelper<T>
    {
        private static readonly ILog logger = LogProvider.For<NotificationFileHelper<T>>();

        public static T ReadJsonFile(string filePath)
        {
            try
            {
                if(File.Exists(filePath))
                    return JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
            }
            catch (Exception ex)
            {
                var strError = string.Format("Could not ReadFile because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return default(T);
        }

        public static void WriteToJsonFile(T model, string filePath)
        {
            try
            {
                var dirPath = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                File.WriteAllText(filePath, JsonConvert.SerializeObject(model));
            }
            catch (Exception ex)
            {
                var strError = string.Format("Could not WriteToFile because: {0}", ex.ToString());
                logger.Error(strError);
            }
        }
    }
}