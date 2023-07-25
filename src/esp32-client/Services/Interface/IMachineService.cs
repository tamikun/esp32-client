using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IMachineService
    {
        Task<Machine?> GetById(int id);
        Task<List<Machine>> GetAll();
        Task<Machine> Create(MachineCreateModel model);
        Task<Machine> Update(MachineUpdateModel model);
        Task Delete(int id);
        Task Delete(List<int> listId);
    }
}