using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IProductService
    {
        Task<Product?> GetById(int id);

        Task<Product?> GetByProductName(string name);

        Task<List<Product>> GetAll();

        Task<ProductCreateModel> Create(ProductCreateModel model);

        Task<ProductUpdateModel> Update(ProductUpdateModel model);

        Task<bool> IsProductInUse(int productId);

        Task Delete(int id);
    }
}