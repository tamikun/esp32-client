using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IProductService
    {
        Task<List<string>> GetAllProduct();
        Task<List<Product>> GetProductDetail(string productName);
        Task<ProductCreateModel> Create(ProductCreateModel model);
    }
}