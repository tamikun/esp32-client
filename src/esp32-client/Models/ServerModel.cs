using esp32_client.Services;

namespace esp32_client.Models;

public class ServerModel
{
    public string? IpAddress { get; set; }
    public string? ServerName { get; set; }
    public ServerState ServerState { get; set; } = ServerState.Unknown;
}

public enum ServerState
{
    Unknown = 0,
    Server = 1,
    Machine = 2
}

public class SelectedServerModel
{
    public string? IpAddress { get; set; }
    public string? Folder { get; set; }
    public bool IsSelected { get; set; } = false;
}

public class SelectedDeleteFileServerModel
{
    public string? IpAddress { get; set; }
    public string? Folder { get; set; }
    public string? FileName { get; set; }
    public bool IsSelected { get; set; } = false;
}

public class ServerModelDetail
{
    public string? IpAddress { get; set; }
    public string? SubDirectory { get; set; }
    public RequestFileModel? RequestFileModel { get; set; }
    public List<SelectedDeleteFileServerModel>? ListDeleteFile { get; set; }
}
