using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IUserRoleService
    {
        Task<UserRole> GetById(int id);

        Task<List<UserRole>> GetAll();

        Task<UserRole> Create(UserRoleCreateModel model);

        Task<UserRole> Update(UserRoleUpdateModel model);

        Task Delete(int id);
    }
}