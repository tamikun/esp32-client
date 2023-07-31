
using esp32_client.Builder;

namespace esp32_client.Domain
{
    public class Factory : BaseEntity
    {
#nullable disable
        public string FactoryNo { get; set; }
        public string FactoryName { get; set; }
    }
}