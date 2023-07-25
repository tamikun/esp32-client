namespace esp32_client.Models;

public class ProductCreateModel
{
#nullable disable
    public string ProductName { get; set; }
}

public class ProductUpdateModel
{
    public int Id { get; set; }
    public string ProductName { get; set; }
}
