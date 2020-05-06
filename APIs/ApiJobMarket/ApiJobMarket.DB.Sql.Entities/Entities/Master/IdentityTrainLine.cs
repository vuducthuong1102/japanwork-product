using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityTrainLine : IdentityCommon
    {
        public int id { get; set; }    
        
        public int translation_id { get; set; }

        public string train_line { get; set; }

        public string furigana { get; set; }

        public int prefecture_id { get; set; }

        public List<IdentityStation> Stations { get; set; }

        public int city_id { get; set; }

        [JsonIgnore]
        public bool IsSelected { get; set; }

        [JsonIgnore]
        public DateTime? created_at { get; set; }

        [JsonIgnore]
        public DateTime? updated_at { get; set; }
    }
}
