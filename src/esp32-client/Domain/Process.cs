
using esp32_client.Builder;

namespace esp32_client.Domain
{
    public class Process : BaseEntity
    {
#nullable disable
        public int ProductId { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public string ProcessNo { get; set; } = string.Empty;
        public string PatternNo { get; set; } = string.Empty;
        public string PatternDirectory { get; set; } = string.Empty;
        public string OperationData { get; set; } = string.Empty;
        public string COAttachment { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

    }
}