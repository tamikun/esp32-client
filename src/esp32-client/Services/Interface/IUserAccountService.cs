using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IUserAccountService
    {
        Task<bool> IsValidUser(string loginName, string password);
    }
}