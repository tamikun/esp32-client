
namespace esp32_client.Domain
{
    public class Product
    {
#nullable disable
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProcessName { get; set; }
        public int Order { get; set; }
        public string PatternNumber { get; set; } = string.Empty;
    }
}