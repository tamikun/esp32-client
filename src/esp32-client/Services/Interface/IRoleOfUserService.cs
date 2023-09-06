using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IRoleOfUserService
    {
        Task<RoleOfUser> GetById(int id);

        Task<List<RoleOfUser>> GetAll();

        Task<RoleOfUser> Create(RoleOfUserCreateModel model);

        Task<RoleOfUser> Update(RoleOfUserUpdateModel model);

        Task Delete(int id);
    }
}