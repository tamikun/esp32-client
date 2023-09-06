
using esp32_client.Builder;

namespace esp32_client.Domain
{
    public class ScheduleTask : BaseEntity
    {

        public string Name { get; set; }
        public int Seconds { get; set; }
        public string Method { get; set; }
        public bool Enabled { get; set; }
        public DateTime? LastStartUtc { get; set; }
        public DateTime? LastSuccessUtc { get; set; }
    }
}