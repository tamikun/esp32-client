namespace esp32_client.Models;
public class LineCreateModel
{
#nullable disable
    public int DepartmentId { get; set; }
    public string LineName { get; set; }
    public int Order { get; set; }
    public int ProductId { get; set; }
}

public class LineUpdateModel
{
    public int Id { get; set; }
    public int DepartmentId { get; set; }
    public string LineName { get; set; }
    public int Order { get; set; }
    public int ProductId { get; set; }
}

public class LineResponseModel
{
    public int Id { get; set; }
    public string LineName { get; set; }
    public string ProductName { get; set; }
}
