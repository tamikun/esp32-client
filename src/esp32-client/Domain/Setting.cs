
using esp32_client.Builder;

namespace esp32_client.Domain
{
    public class Setting : BaseEntity
    {
#nullable disable
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }
}