
namespace esp32_client.Models;

public class EspFileModel
{
    public string? FileName { get; set; }

    public string? FileType { get; set; }

    public long? FileSize { get; set; }
}

public class RequestFileModel
{
    public RequestFileModel()
    {

    }

    public IFormFile? File { get; set; }

    public string? FilePath { get; set; }

}
