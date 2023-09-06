
using esp32_client.Builder;

namespace esp32_client.Domain
{
    public class RoleOfUser : BaseEntity
    {

        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}