using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IDepartmentService
    {
        Task<Factory?> GetById(int id);

        Task<List<Factory>> GetAll();

        Task<Factory> Create(DepartmentCreateModel model);

        Task<Factory> Update(DepartmentUpdateModel model);

        Task Delete(int id);
    }
}