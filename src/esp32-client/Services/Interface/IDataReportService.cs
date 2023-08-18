using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IDataReportService
    {
        Task<List<DataReport>> GetAll();
        Task<DataReport> RandomCreate();
        Task<DataReport> GetLastDataByStationId(int stationId);
    }
}