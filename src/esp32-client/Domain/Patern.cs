
namespace esp32_client.Domain
{
    public class Patern
    {
#nullable disable
        public int Id { get; set; }
        public string PaternNumber { get; set; } = string.Empty;
        public string FileName { get; set; }
        public string FileData { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}