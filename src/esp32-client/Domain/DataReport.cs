
using esp32_client.Builder;

namespace esp32_client.Domain
{
    public class DataReport : BaseEntity
    {
        public int StationId { get; set; }
        public int ProductNumber { get; set; } 
        public DateTime DateTimeUtc { get; set; }
    }
}