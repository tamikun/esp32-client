
using esp32_client.Builder;

namespace esp32_client.Domain
{
    public class Machine : BaseEntity
    {
        public string MachineNo { get; set; }
        public string MachineName { get; set; }
        public string IpAddress { get; set; }
        public int FactoryId { get; set; }
        public int LineId { get; set; }
        public int StationId { get; set; }
        public string COPartNo { get; set; } = string.Empty;
        public bool CncMachine { get; set; }
        // public bool UpdateFirmwareSucess { get; set; } = true;
    }
}