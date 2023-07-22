
namespace esp32_client.Domain
{
    public class UserAccount
    {
        public int Id { get; set; }
        public string? LoginName { get; set; }
        public string? Password { get; set; }
        public string? SalfKey { get; set; }
        public string? UserName { get; set; }
        public int RoleId { get; set; }
    }
}