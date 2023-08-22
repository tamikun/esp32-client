using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Services
{
    public interface ILineService
    {
        Task<Line?> GetById(int id);

        Task<PagedListModel<Line>> GetAll(int pageIndex, int pageSize);

        Task<List<Line>> GetByFactoryId(int factoryId);

        Task<List<LineResponseModel>> GetAllLineResponse(int factoryId);

        Task<Line> Create(LineCreateModel model);

        Task<List<GetInfoProductLineModel>> GetInfoProductLine(int factoryId);

        Task<Dictionary<string, string>> AssignProductLine(AssignProductLineModel model);

        Task<Dictionary<string, string>> AssignStationProcess(AssignStationProcessModel model);

        Task<List<GetStationAndProcessModel>> GetStationAndProcess(int lineId);

        Task<List<GetProcessAndMachineOfLineModel>> GetProcessAndMachineOfLine(int factoryId, List<int>? listLineId = null,
            bool iotMachine = false, bool hasProduct = false, bool hasMachine = true);

        Task Delete(int lineId);

        Task UpdateNameAndStationNo(LineUpdateModel model);
    }
}