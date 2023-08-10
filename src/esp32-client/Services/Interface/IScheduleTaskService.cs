using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IScheduleTaskService
    {
        Task SaveProductData();
    }
}