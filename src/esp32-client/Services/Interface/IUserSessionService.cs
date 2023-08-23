using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IUserSessionService
    {
        Task<UserSession?> GetById(int id);

        Task<List<UserSession>> GetByUserId(int userId);

        Task<bool> CheckToken(string token);

        Task<List<UserSession>> GetAll();

        Task<UserSession> Create(UserSessionCreateModel model);

        Task Delete(string token);

        Task Delete(int id);

        Task DeleteAll(int userId, string token);
    }
}