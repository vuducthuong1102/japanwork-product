using MsSql.AspNet.Identity.Entities;

namespace MsSql.AspNet.Identity
{
    public class IdentityWidget : CommonIdentity
    {
        public int Id { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public int Status { get; set; }
    }
}
