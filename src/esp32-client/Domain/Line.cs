
using esp32_client.Builder;

namespace esp32_client.Domain
{
    public class Line : BaseEntity
    {
#nullable disable
        public int DepartmentId { get; set; }
        public string LineName { get; set; } = string.Empty;
        public int Order { get; set; }
        public int ProductId { get; set; }
    }
}