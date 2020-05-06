namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityEmailSetting
    {
        public int Id { get; set; }
        public int AgencyId { get; set; }
        public int StaffId { get; set; }
        public string Email { get; set; }
        public string EmailPasswordHash { get; set; }
        public int EmailServerId { get; set; }
        public int EmailType { get; set; }
        public bool TestingSuccessed { get; set; }

        public bool PasswordChanged { get; set; }
    }
}
