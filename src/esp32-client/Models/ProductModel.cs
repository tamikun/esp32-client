namespace esp32_client.Models;

public class ProductCreateModel
{

    public int FactoryId { get; set; }
    public string ProductName { get; set; }
    public int ProductNo { get; set; }
    public int NumberOfProcess { get; set; }
}

public class ProductUpdateModel
{
    public int FactoryId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int NumberOfProcess { get; set; }
}

public class ProductResponseModel
{
    public int FactoryId { get; set; }
    public string FactoryName { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductNo { get; set; }
    public int NumberOfProcess { get; set; }
}
