using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IProcessService
    {
        Task<Process?> GetById(int id);

        Task<List<Process>> GetByProductId(int id);

        Task<Process?> GetByProcessName(string name);

        Task<List<Process>> GetAll();

        Task<ProcessUpdateModel> UpdateProcessNamePatternNoById(ProcessUpdateModel model);
        
    }
}