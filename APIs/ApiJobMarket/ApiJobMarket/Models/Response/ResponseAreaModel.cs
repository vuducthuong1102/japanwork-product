namespace ApiJobMarket.Models
{
    public class ApiResponseRegionItemModel
    {
       public int id { get; set; }
       public string region { get; set; }
       public string furigana { get; set; }
    }

    public class ApiResponsePrefectureItemModel
    {
        public int id { get; set; }
        public string prefecture { get; set; }
        public string furigana { get; set; }
        public int region_id { get; set; }
    }

    public class ApiResponseCityItemModel
    {
        public int id { get; set; }
        public string city { get; set; }
        public string furigana { get; set; }
        public int prefecture_id { get; set; }
    }

    public class ApiResponseStationItemModel
    {
        public int id { get; set; }
        public string station { get; set; }
        public string furigana { get; set; }

    }    
}