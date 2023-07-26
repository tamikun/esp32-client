
using esp32_client.Builder;

namespace esp32_client.Domain
{
    public class Pattern : BaseEntity
    {
#nullable disable
        public string PatternNumber { get; set; } = string.Empty;
        public string FileName { get; set; }
        public string FileData { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}