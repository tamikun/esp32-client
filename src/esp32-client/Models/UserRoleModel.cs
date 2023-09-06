namespace esp32_client.Models;
public class UserRoleCreateModel
{

    public string RoleName { get; set; }
}

public class UserRoleUpdateModel
{
    public int Id { get; set; }
    public string RoleName { get; set; }
}
