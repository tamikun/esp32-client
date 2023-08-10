using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IMachineService
    {
        Task<Machine?> GetById(int id);
        Task<List<Machine>> GetByListId(IEnumerable<int> listId);
        Task<List<Machine>> GetAll();
        Task<List<MachineResponseModel>> GetByFactoryId(int factoryId);
        Task<List<Machine>> GetInUseMachineByLine(int lineId);
        Task<List<Machine>> GetAvalableMachine(int lineId);
        Task<List<Machine>> UpdateMachineLineByProduct(int lineId, int productId);
        Task<Machine> Create(MachineCreateModel model);
        Task<Machine> Update(MachineUpdateModel model);
        Task DeleteById(int id);
        Task<Dictionary<string, string>> AssignMachineLine(ListAssignMachineLineModel model);
        Task UpdateById(int id, int departmentId, int lineId, int processId);
        Task UpdateByListId(IEnumerable<int> listId, int departmentId, int lineId, int processId);
        Task<Dictionary<string, string>> AssignPatternMachine(IEnumerable<int> machineId);
        Task<List<EspFileModel>> GetDefaultListFile(string machineIp);
        Task<(bool Success, int Data)> GetProductNumberMachine(string ipAddress);
    }
}