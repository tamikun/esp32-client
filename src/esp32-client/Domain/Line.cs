
using esp32_client.Builder;

namespace esp32_client.Domain
{
    public class Line : BaseEntity
    {
#nullable disable
        public int FactoryId { get; set; }
        public string LineNo { get; set; } = string.Empty;
        public string LineName { get; set; } = string.Empty;
        public int ProductId { get; set; }
    }
}