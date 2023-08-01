
using esp32_client.Builder;

namespace esp32_client.Domain
{
    public class Station : BaseEntity
    {
#nullable disable
        public int LineId { get; set; }
        public string StationNo { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
    }
}