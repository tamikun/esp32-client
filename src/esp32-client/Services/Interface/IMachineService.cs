using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IMachineService
    {
        Task<List<Machine>> GetAll();
        Task<Machine> Create(Machine model);
        Task<Machine?> GetById(int id);
        Task Delete(int id);
        Task Delete(List<int> listId);
    }
}