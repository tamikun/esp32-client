
using esp32_client.Builder;

namespace esp32_client.Domain
{
    public class Factory : BaseEntity
    {
        public string FactoryNo { get; set; }
        public string FactoryName { get; set; }
    }
}