
using esp32_client.Builder;

namespace esp32_client.Domain
{
    public class UserAccount : BaseEntity
    {
#nullable disable
        public string LoginName { get; set; }
        public string Password { get; set; }
        public string SalfKey { get; set; }
        public string UserName { get; set; }
        // public int ExpiredSessionInHour { get; set; } = 168; // Dafault: 7 days
    }
}