using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using EmailConsoler.Logging;

namespace EmailConsoler.Libraries
{
    public class Utility
    {
        private static string _dateTimeXMLFormat = "yyyy-MM-ddTHH:mm:ssZ";
        public static string GetStringLimited(string str, int limitNumber)
        {
            if (str.Length < limitNumber)
            {
                return str;
            }

            return str.Substring(0, limitNumber);
        }

        public static string FormatStringToKey(string str, bool isUpperCase = true)
        {
            if (!string.IsNullOrEmpty(str))
            {
                str = ConvertToUnsign(str);

                str = str.Replace(" ", "_");
                if (isUpperCase)
                {
                    str = str.ToUpper();
                }
                else
                {
                    str = str.ToLower();
                }
            }

            return str;
        }

        public static string FormatTitleStringToKey(string titleEN, string titleVI, int lineNumber, bool isUpperCase = true)
        {
            var titleKey = string.Empty;
            if (!string.IsNullOrEmpty(titleEN))
            {
                titleKey = ConvertToUnsign(titleEN);                
            }
            else
            {
                titleKey = ConvertToUnsign(titleVI);
            }

            //Replace all space to _
            titleKey = titleKey.Replace(" ", "_");

            //Replace all / to -
            titleKey = titleKey.Replace("/", "-");

            //Replace all \ to -
            titleKey = titleKey.Replace("\\", "-");

            if (isUpperCase)
            {
                titleKey = titleKey.ToUpper();
            }
            else
            {
                titleKey = titleKey.ToLower();
            }

            if (lineNumber != 0)
            {
                titleKey = string.Format("{0}_{1}", lineNumber, titleKey);
            }
            else
            {
                titleKey = string.Empty;
            }

            return titleKey;
        }

        public static string ConvertDateTimeToXMLFormat(DateTime? datetime)
        {
            var result = string.Empty;
            try
            {
                result = (datetime != null) ? datetime.Value.ToString(_dateTimeXMLFormat) : string.Empty;
            }
            catch (Exception ex)
            {
                var strError = string.Format("Cannot convert datetime to string. Excetion message: {0}", ex.ToString());
            }

            return result;
        }

        public static string ConvertToUnsign(string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;

            string[] signs = new string[] { 
                            "aAeEoOuUiIdDyY",
                            "áàạảãâấầậẩẫăắằặẳẵ",
                            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
                            "éèẹẻẽêếềệểễ",
                            "ÉÈẸẺẼÊẾỀỆỂỄ",
                            "óòọỏõôốồộổỗơớờợởỡ",
                            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
                            "úùụủũưứừựửữ",
                            "ÚÙỤỦŨƯỨỪỰỬỮ",
                            "íìịỉĩ",
                            "ÍÌỊỈĨ",
                            "đ",
                            "Đ",
                            "ýỳỵỷỹ",
                            "ÝỲỴỶỸ"
            };

            for (int i = 1; i < signs.Length; i++)
            {
                for (int j = 0; j < signs[i].Length; j++)
                {
                    str = str.Replace(signs[i][j], signs[0][i - 1]);
                }
            }
            return str;
        }


        public static string CreateXmlFile(IXmlSerializable xmlObject, Type xmlObjectType, string filePath)
        {            
            var strError = string.Empty;            
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                ConformanceLevel = ConformanceLevel.Document,
                OmitXmlDeclaration = false,
                CloseOutput = true,
                Indent = true,
                IndentChars = "  ",
                NewLineHandling = NewLineHandling.Replace
            };

            try
            {

                /*
                using (StreamWriter sw = File.CreateText(filePath))
                using (XmlWriter writer = XmlWriter.Create(sw, settings))
                {
                    xmlObject.WriteXml(writer);
                    writer.Close();
                }
                */


                /*
                XmlSerializer serializer = new XmlSerializer(xmlObjectType);              
                var sw = File.Create(filePath);                
                serializer.Serialize(sw, xmlObject);
                sw.Close();
                */

                var serializer = new XmlSerializer(xmlObjectType);
                string utf8;
                using (StringWriter writer = new Utf8StringWriter())
                {
                    serializer.Serialize(writer, xmlObject);
                    utf8 = writer.ToString();
                }

                File.WriteAllText(filePath, utf8);

                //Rename to xml extentions
                var newFilePath = Path.ChangeExtension(filePath, "xml");
                File.Move(filePath, newFilePath);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to create XML file due to: {0}", ex.ToString());
            }

            return strError;
        }
    }


    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }
    }
}
