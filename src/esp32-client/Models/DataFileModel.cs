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

    public bool IsSelected { get; set; } = false;
}

public class FileSystemRequestModel
{
    public FileSystemRequestModel()
    {
        ListDeleteFile = new List<SelectedDataFileModel>();
    }

    public IFormFileCollection? ListUploadFile { get; set; }
    public string? Folder { get; set; }
    public List<SelectedDataFileModel> ListDeleteFile { get; set; }
}
