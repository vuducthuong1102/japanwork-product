using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySite.Models.Account
{
    public class SocialProviderModel
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }
    }
}