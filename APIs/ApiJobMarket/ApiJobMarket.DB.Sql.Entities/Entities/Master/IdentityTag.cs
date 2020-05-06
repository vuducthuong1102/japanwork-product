namespace ApiJobMarket.DB.Sql.Entities
{
    public class IdentityTag : IdentityCommon
    {
        public int id { get; set; }
        public string tag { get; set; }
    }

    public class IdentityJobTag : IdentityTag
    {
        public int job_id { get; set; }
        public int tag_id { get; set; }
    }
}
