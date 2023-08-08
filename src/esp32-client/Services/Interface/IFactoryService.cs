using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IFactoryService
    {
        Task<Factory?> GetById(int id);

        Task<List<Factory>> GetAll();

        Task Delete(int id);
    }
}