namespace esp32_client.Models;
public class RoleOfUserCreateModel
{

    public int UserId { get; set; }
    public int RoleId { get; set; }
}

public class RoleOfUserUpdateModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RoleId { get; set; }
}
