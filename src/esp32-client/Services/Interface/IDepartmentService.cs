using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IDepartmentService
    {
        Task<Department?> GetById(int id);

        Task<List<Department>> GetAll();

        Task<Department> Create(DepartmentCreateModel model);

        Task<Department> Update(DepartmentUpdateModel model);

        Task Delete(int id);
    }
}