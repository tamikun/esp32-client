namespace esp32_client.Models;
public class UserRoleCreateModel
{
#nullable disable
    public string RoleName { get; set; }
}

public class UserRoleUpdateModel
{
    public int Id { get; set; }
    public string RoleName { get; set; }
}
