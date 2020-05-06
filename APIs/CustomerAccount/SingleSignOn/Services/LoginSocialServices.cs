using Newtonsoft.Json;
using SingleSignOn.Logging;
using SingleSignOn.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace SingleSignOn.Services
{
    public class LoginSocialServices
    {
        private static readonly ILog logger = LogProvider.For<LoginSocialServices>();

        public static async Task<string> GetAvatarAsync(string providerName, string userName)
        {
            var avatar = string.Empty;
            if (providerName.ToLower() == "facebook")
            {
                avatar = "https://graph.facebook.com/" + userName + "/picture?type=normal";
            }
            else if (providerName.ToLower() == "google")
            {
                var strError = string.Empty;
                var _baseUrl = string.Format("{0}/{1}", "https://picasaweb.google.com/data/entry/api/user", userName);

                try
                {
                    var client = new HttpClient();
                    client.Timeout = TimeSpan.FromSeconds(SystemSettings.ExternalServiceTimeout);

                    //Begin calling
                    var response = new HttpResponseMessage();

                    // Post to the Server and parse the response.
                    response = client.GetAsync(_baseUrl).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        response.EnsureSuccessStatusCode();

                        // Parsing the returned result                    
                        var responseString = await response.Content.ReadAsStringAsync();

                        var buffer = Encoding.UTF8.GetBytes(responseString);
                        using (var stream = new MemoryStream(buffer))
                        {
                            var serializer = new XmlSerializer(typeof(Entry));
                            var googleEntry = (Entry)serializer.Deserialize(stream);

                            //then do whatever you want
                            if (googleEntry != null)
                            {
                                avatar = googleEntry.Thumbnail;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    strError = string.Format("Failed when calling api to GetAvatarAsync - {0} because: {1}", _baseUrl, ex.ToString());
                    logger.Error(strError);
                }
            }

            await Task.FromResult(avatar);

            return avatar;
        }

        [XmlRoot(ElementName = "category", Namespace = "http://www.w3.org/2005/Atom")]
        public class Category
        {
            [XmlAttribute(AttributeName = "scheme")]
            public string Scheme { get; set; }
            [XmlAttribute(AttributeName = "term")]
            public string Term { get; set; }
        }

        [XmlRoot(ElementName = "title", Namespace = "http://www.w3.org/2005/Atom")]
        public class Title
        {
            [XmlAttribute(AttributeName = "type")]
            public string Type { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "summary", Namespace = "http://www.w3.org/2005/Atom")]
        public class Summary
        {
            [XmlAttribute(AttributeName = "type")]
            public string Type { get; set; }
        }

        [XmlRoot(ElementName = "link", Namespace = "http://www.w3.org/2005/Atom")]
        public class Link
        {
            [XmlAttribute(AttributeName = "rel")]
            public string Rel { get; set; }
            [XmlAttribute(AttributeName = "type")]
            public string Type { get; set; }
            [XmlAttribute(AttributeName = "href")]
            public string Href { get; set; }
        }

        [XmlRoot(ElementName = "author", Namespace = "http://www.w3.org/2005/Atom")]
        public class Author
        {
            [XmlElement(ElementName = "name", Namespace = "http://www.w3.org/2005/Atom")]
            public string Name { get; set; }
            [XmlElement(ElementName = "uri", Namespace = "http://www.w3.org/2005/Atom")]
            public string Uri { get; set; }
        }

        [XmlRoot(ElementName = "entry", Namespace = "http://www.w3.org/2005/Atom")]
        public class Entry
        {
            [XmlElement(ElementName = "id", Namespace = "http://www.w3.org/2005/Atom")]
            public string Id { get; set; }
            [XmlElement(ElementName = "published", Namespace = "http://www.w3.org/2005/Atom")]
            public string Published { get; set; }
            [XmlElement(ElementName = "updated", Namespace = "http://www.w3.org/2005/Atom")]
            public string Updated { get; set; }
            [XmlElement(ElementName = "category", Namespace = "http://www.w3.org/2005/Atom")]
            public Category Category { get; set; }
            [XmlElement(ElementName = "title", Namespace = "http://www.w3.org/2005/Atom")]
            public Title Title { get; set; }
            [XmlElement(ElementName = "summary", Namespace = "http://www.w3.org/2005/Atom")]
            public Summary Summary { get; set; }
            [XmlElement(ElementName = "link", Namespace = "http://www.w3.org/2005/Atom")]
            public List<Link> Link { get; set; }
            [XmlElement(ElementName = "author", Namespace = "http://www.w3.org/2005/Atom")]
            public Author Author { get; set; }
            [XmlElement(ElementName = "user", Namespace = "http://schemas.google.com/photos/2007")]
            public string User { get; set; }
            [XmlElement(ElementName = "nickname", Namespace = "http://schemas.google.com/photos/2007")]
            public string Nickname { get; set; }
            [XmlElement(ElementName = "thumbnail", Namespace = "http://schemas.google.com/photos/2007")]
            public string Thumbnail { get; set; }
            [XmlElement(ElementName = "quotalimit", Namespace = "http://schemas.google.com/photos/2007")]
            public string Quotalimit { get; set; }
            [XmlElement(ElementName = "quotacurrent", Namespace = "http://schemas.google.com/photos/2007")]
            public string Quotacurrent { get; set; }
            [XmlElement(ElementName = "maxPhotosPerAlbum", Namespace = "http://schemas.google.com/photos/2007")]
            public string MaxPhotosPerAlbum { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlAttribute(AttributeName = "gphoto", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Gphoto { get; set; }
        }
    }
}