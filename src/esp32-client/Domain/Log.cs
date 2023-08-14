
using esp32_client.Builder;

namespace esp32_client.Domain
{
    public class Log : BaseEntity
    {
#nullable disable
        public string Message { get; set; }
        public string FullMessage { get; set; }
        public DateTime DateTimeUtc { get; set; }
    }
}