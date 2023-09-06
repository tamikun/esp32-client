using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IProductService
    {
        Task<Product> GetById(int id);

        Task<Product> GetByProductNo(string productNo, int factoryId);

        Task<List<Product>> GetAll(int factoryId);

        Task<ProductCreateModel> Create(ProductCreateModel model);

        Task UpdateNameAndProcess(ProductUpdateModel model);

        Task<List<ProductResponseModel>> GetProductByFactoryId(int factoryId);

        Task<bool> IsProductInUse(int productId);

        Task Delete(int id);
    }
}