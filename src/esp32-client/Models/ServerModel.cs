using esp32_client.Services;

namespace esp32_client.Models;

public class ServerModel
{
    public string? IpAddress { get; set; }
    public string? ServerName { get; set; }
}

public class SelectedServerModel
{
    public string? IpAddress { get; set; }
    public string? Folder { get; set; }
    public bool IsSelected { get; set; } = false;
}

public class ServerModelDetail
{
    public string? IpAddress { get; set; }
    public string? SubDirectory { get; set; }
}
