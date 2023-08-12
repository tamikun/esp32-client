using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IStationService
    {
        Task<Station?> GetById(int id);

        Task<List<Station>> GetAll();

        Task<List<Station>> GetByLineId(int lineId);

        Task UpdateStationName(StationUpdateModel model);

        Task DeleteListStation(List<Station> listStation);
    }
}