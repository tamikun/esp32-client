
using esp32_client.Builder;

namespace esp32_client.Domain
{
    public class UserRight : BaseEntity
    {

        public int RoleId { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }
}