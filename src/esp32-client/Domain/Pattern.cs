
namespace esp32_client.Domain
{
    public class Pattern
    {
#nullable disable
        public int Id { get; set; }
        public string PatternNumber { get; set; } = string.Empty;
        public string FileName { get; set; }
        public string FileData { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}