
using esp32_client.Builder;

namespace esp32_client.Domain
{
    public class Department : BaseEntity
    {
#nullable disable
        public string DepartmentName { get; set; } = string.Empty;
    }
}