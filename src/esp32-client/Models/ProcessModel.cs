namespace esp32_client.Models;

public class ProcessCreateModel
{
#nullable disable
    public int ProductId { get; set; }
    public string ProcessNo { get; set; }
}

public class ProcessUpdateModel
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProcessName { get; set; }
    public int PatternId { get; set; }
    public int Order { get; set; }
}

public class ProcessAddRequestModel
{
    public ProcessAddRequestModel()
    {
        ListProcessCreate = new List<ProcessModel>();
    }

    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public List<ProcessModel> ListProcessCreate { get; set; }
}

public class ProcessModel
{
    public int Id { get; set; }
    public string ProcessName { get; set; }
    public int PatternId { get; set; }
    public int Order { get; set; }
}