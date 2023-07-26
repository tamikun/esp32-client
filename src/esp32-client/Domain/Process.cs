
using esp32_client.Builder;

namespace esp32_client.Domain
{
    public class Process : BaseEntity
    {
#nullable disable
        public int ProductId { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public int PatternId { get; set; }
        public int Order { get; set; }
    }
}