using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IUserAccountService
    {
        Task<UserAccount?> GetByLoginName(string loginName);

        Task<bool> IsValidUser(string loginName, string password);

        Task<UserAccountCreateModel> Create(UserAccountCreateModel model);
    }
}