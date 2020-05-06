namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentitySearchPlace
    {
        public int id { get; set; }
        public int parent_id { get; set; }
        public string name { get; set; }
        public string furigana { get; set; }
        public string type { get; set; }
    }
}
