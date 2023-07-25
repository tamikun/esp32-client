using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IUserAccountService
    {
        Task<UserAccount?> GetById(int id);

        Task<UserAccount?> GetByLoginName(string loginName);

        Task<bool> IsValidUser(string loginName, string password);

        Task<UserAccountCreateModel> Create(UserAccountCreateModel model);

        Task<UserAccountUpdateModel> Update(UserAccountUpdateModel model);

        Task<UserAccountChangePasswordModel> ChangePassword(UserAccountChangePasswordModel model);

        Task Delete(int id);

        Task<bool> CheckUserRight(string? loginName, string? controllerName, string? actionName);

        Task<List<UserRight>> GetUserRight(string? loginName);
    }
}