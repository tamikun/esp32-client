
namespace esp32_client.Domain
{
    public class Line
    {
#nullable disable
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string LineName { get; set; } = string.Empty;
        public int Order { get; set; }
        public int ProductId { get; set; }
    }
}