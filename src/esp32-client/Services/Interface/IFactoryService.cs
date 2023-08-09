using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IFactoryService
    {
        Task<Factory?> GetById(int id);

        Task<List<Factory>> GetAll();

        Task<List<FactoryResponseModel>> GetAllResponse();

        Task<Factory> Create(int factoryNo, string factoryName);

        Task Delete(int id);

        Task UpdateName(int id, string name);
    }
}