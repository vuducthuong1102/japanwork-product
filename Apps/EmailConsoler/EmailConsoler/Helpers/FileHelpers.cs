using EmailConsoler.Logging;
using Newtonsoft.Json;
using System;
using System.IO;

namespace EmailConsoler.Helpers
{
    public class FileHelpers<T>
    {
        private static readonly ILog logger = LogProvider.For<FileHelpers<T>>();

        public static T ReadJsonFileToObject(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                    return JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
            }
            catch (Exception ex)
            {
                var strError = string.Format("Could not ReadJsonFileToObject because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return default(T);
        }

        public static void WriteObjectToJsonFile(T model, string filePath)
        {
            try
            {
                File.WriteAllText(filePath, JsonConvert.SerializeObject(model));
            }
            catch (Exception ex)
            {
                var strError = string.Format("Could not WriteObjectToJsonFile because: {0}", ex.ToString());
                logger.Error(strError);
            }
        }
    }

    public class FileHelper
    {
        private static readonly ILog logger = LogProvider.For<FileHelper>();
        public static string GetFileNameFromUrl(string url)
        {
            var fileName = string.Empty;
            try
            {
                Uri SomeBaseUri = new Uri("http://canbeanything");
                Uri uri;
                if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
                    uri = new Uri(SomeBaseUri, url);

                fileName = Path.GetFileName(uri.LocalPath);
            }
            catch (Exception ex)
            {
                var strError = string.Format("Could not GetFileNameFromUrl because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return fileName;
        }
    }
}
