namespace esp32_client.Models;

public class ProductCreateModel
{
#nullable disable
    public int FactoryId { get; set; }
    public string ProductName { get; set; }
    public int ProductNo { get; set; }
    public int NumberOfProcess { get; set; }
}

public class ProductUpdateModel
{
    public int Id { get; set; }
    public string ProductName { get; set; }
}

public class ProductResponseModel
{
    public int FactoryId { get; set; }
    public string FactoryName { get; set; }
    public string ProductName { get; set; }
    public string ProductNo { get; set; }
    public int NumberOfProcess { get; set; }
}
