
using esp32_client.Builder;

namespace esp32_client.Domain
{
    public class Log : BaseEntity
    {
#nullable disable
        public string Name { get; set; }
        public string Message { get; set; }
        public DateTime DateTimeUtc { get; set; }
    }
}