namespace esp32_client.Models;

public class DataFileModel
{
    public string? FilePath { get; set; }

    public string? FileType { get; set; }

    public long? FileSize { get; set; }

}

public class SelectedDataFileModel
{
    public string? FilePath { get; set; }

    public string? FileType { get; set; }

    public long? FileSize { get; set; }

    public bool IsSelected { get; set; } = false;
}
