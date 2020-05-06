using Newtonsoft.Json;
using System.Collections.Generic;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityAddress : IdentityCommon
    {
        public int id { get; set; }
        public int country_id { get; set; }
        public int region_id { get; set; }
        public int prefecture_id { get; set; }
        public int city_id { get; set; }
        public string detail { get; set; }
        public string furigana { get; set; }
        public string note { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string postal_code { get; set; }
        public int train_line_id { get; set; }
        public string train_line { get; set; }
        public string train_line_furigana { get; set; }
        public string address { get; set; }
        public string address_furigana { get; set; }

        //Extends
        public IdentityRegion region_info { get; set; }
        public IdentityPrefecture prefecture_info { get; set; }
        public IdentityCity city_info { get; set; }
    }

    public class IdentityJobAddress : IdentityAddress
    {
        public List<IdentityJobAddressStation> Stations { get; set; }        
        public List<IdentityStation> ListStations { get; set; }
        public int job_id { get; set; }

        public IdentityJobAddress()
        {
            Stations = new List<IdentityJobAddressStation>();
        }
    }

    public class IdentityJobSeekerAddress : IdentityAddress
    {
        public int job_seeker_id { get; set; }        
        public bool is_contact_address { get; set; }
        public int train_line_id { get; set; }
        public int station_id { get; set; }
    }
}
