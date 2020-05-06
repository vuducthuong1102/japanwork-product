using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityTypeSuggest
    {
        public int id { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public string language_code { get; set; }
        public int form_id { get; set; }
        public string icon { get; set; }

        public List<IdentityTypeSuggestLang> ListLang { get; set; }

        public IdentityTypeSuggest()
        {
            ListLang = new List<IdentityTypeSuggestLang>();
        }
    }

    public class IdentityTypeSuggestLang
    {
        public int id { get; set; }
        public string type { get; set; }
        public string language_code { get; set; }

        public string description { get; set; }

        public int type_suggest_id { get; set; }
    }
}
