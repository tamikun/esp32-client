using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IPaternService
    {
        Task<List<PaternResponseModel>> GetAll();

        Task<Patern> Create(PaternCreateModel model);

        Task<Patern?> GetById(int id);

        Task Delete(int id);

        Task Delete(List<int> listId);
    }
}