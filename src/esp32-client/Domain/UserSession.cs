
using esp32_client.Builder;

namespace esp32_client.Domain
{
    public class UserSession : BaseEntity
    {
#nullable disable
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiredTime { get; set; }
    }
}