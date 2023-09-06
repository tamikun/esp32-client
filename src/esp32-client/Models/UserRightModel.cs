namespace esp32_client.Models;
public class UserRightCreateModel
{

    public int RoleId { get; set; }
    public string ControllerName { get; set; }
    public string ActionName { get; set; }
}

public class UserRightUpdateModel
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public string ControllerName { get; set; }
    public string ActionName { get; set; }
}
