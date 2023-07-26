
using esp32_client.Builder;

namespace esp32_client.Domain
{
    public class Product : BaseEntity
    {
#nullable disable
        public string ProductName { get; set; } = string.Empty;
    }
}