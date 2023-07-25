using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IUserRightService
    {
        Task<UserRight?> GetById(int id);

        Task<List<UserRight>> GetAll();

        Task<UserRight> Create(UserRightCreateModel model);

        Task<UserRight> Update(UserRightUpdateModel model);

        Task Delete(int id);
    }
}