
namespace esp32_client.Models;

public class MultipleUploadFileModel
{
    public MultipleUploadFileModel()
    {
        ListSelectedDataFile = new List<SelectedDataFileModel>();
        ListSelectedServer = new List<SelectedServerModel>();
        ReplaceIfExist = false;
    }

    public List<SelectedDataFileModel> ListSelectedDataFile { get; set; }

    public List<SelectedServerModel> ListSelectedServer { get; set; }

    public bool ReplaceIfExist { get; set; }

}

