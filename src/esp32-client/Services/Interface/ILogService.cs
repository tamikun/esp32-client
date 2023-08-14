using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Services
{
    public interface ILogService
    {
        Task AddLog(Exception ex);
    }
}