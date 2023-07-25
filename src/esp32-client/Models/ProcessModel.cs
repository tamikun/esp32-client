namespace esp32_client.Models;

public class ProcessCreateModel
{
#nullable disable
    public int ProductId { get; set; }
    public string ProcessName { get; set; }
    public int PatternId { get; set; }
    public int Order { get; set; }
}

public class ProcessUpdateModel
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProcessName { get; set; }
    public int PatternId { get; set; }
    public int Order { get; set; }
}
