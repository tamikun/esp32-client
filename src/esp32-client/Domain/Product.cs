
using esp32_client.Builder;

namespace esp32_client.Domain
{
    public class Product : BaseEntity
    {
#nullable disable
        public int FactoryId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductNo { get; set; } = string.Empty;
    }
}