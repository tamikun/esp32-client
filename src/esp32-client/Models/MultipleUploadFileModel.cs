
namespace esp32_client.Models;

public class MultipleUploadFileModel
{
    public MultipleUploadFileModel()
    {
        ListSelectedDataFile = new List<SelectedDataFileModel>();
        ListSelectedServer = new List<SelectedServerModel>();
    }

    public List<SelectedDataFileModel> ListSelectedDataFile { get; set; }

    public List<SelectedServerModel> ListSelectedServer { get; set; }

    public string? DataFileDirectory { get; set; }
}

