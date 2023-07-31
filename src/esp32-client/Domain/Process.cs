
using esp32_client.Builder;

namespace esp32_client.Domain
{
    public class Process : BaseEntity
    {
#nullable disable
        public int ProductId { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public string ProcessNo { get; set; } = string.Empty;
        public int PatternNumber { get; set; }
        public string PatternDirectory { get; set; }
        public string OperationData { get; set; }
        public string COAttachment { get; set; }
        public string Description { get; set; }

    }
}