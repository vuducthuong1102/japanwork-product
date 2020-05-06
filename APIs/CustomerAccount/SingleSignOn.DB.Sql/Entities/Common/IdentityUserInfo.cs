using Newtonsoft.Json;

namespace SingleSignOn.DB.Sql.Entities
{
    public class IdentityUserInfo : IdentityCommon
    { 
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string DisplayName { get; set; }
        
        [JsonIgnore]
        public string TokenKey { get; set; }

        public string Avatar { get; set; }

        //For get data of an user
        public int OwnerId { get; set; }
    }
}
