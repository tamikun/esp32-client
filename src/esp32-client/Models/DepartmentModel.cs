namespace esp32_client.Models;
public class DepartmentCreateModel
{
#nullable disable
    public string DepartmentName { get; set; }
}

public class DepartmentUpdateModel
{
    public int Id { get; set; }
    public string DepartmentName { get; set; }
}
