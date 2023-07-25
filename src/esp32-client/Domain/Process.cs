
namespace esp32_client.Domain
{
    public class Process
    {
#nullable disable
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public string PatternId { get; set; }
        public int Order { get; set; }
    }
}