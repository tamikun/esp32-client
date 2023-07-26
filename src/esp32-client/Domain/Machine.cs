
using esp32_client.Builder;

namespace esp32_client.Domain
{
    public class Machine : BaseEntity
    {
#nullable disable
        public string MachineName { get; set; } = string.Empty;
        public string IpAddress { get; set; }
        public int DepartmentId { get; set; }
        public int LineId { get; set; }
        public int ProcessId { get; set; }
    }
}