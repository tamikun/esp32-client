using AutoMapper;
using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
using LinqToDB;

namespace esp32_client.Services;

/// <summary>
/// 
/// </summary>
public partial class MachineService : IMachineService
{

    private readonly LinqToDb _linq2Db;
    private readonly IMapper _mapper;
    private readonly IProcessService _processService;

    public MachineService(LinqToDb linq2Db, IMapper mapper, IProcessService processService)
    {
        _linq2Db = linq2Db;
        _mapper = mapper;
        _processService = processService;
    }

    public async Task<Machine?> GetById(int id)
    {
        var machine = await _linq2Db.Machine.Where(s => s.Id == id).FirstOrDefaultAsync();
        return machine;
    }

    public async Task<List<Machine>> GetByListId(IEnumerable<int> listId)
    {
        return await _linq2Db.Machine.Where(s => listId.Contains(s.Id)).ToListAsync();
    }

    public async Task<List<Machine>> GetAll()
    {
        return await _linq2Db.Machine.ToListAsync();
    }

    public async Task<List<Machine>> GetInUseMachineByLine(int lineId)
    {
        var result = await (from line in _linq2Db.Line.Where(s => s.Id == lineId)
                            join process in _linq2Db.Process on line.ProductId equals process.ProductId
                            from machine in _linq2Db.Machine.Where(s => s.LineId == lineId && s.ProcessId == process.Id)
                            select machine
                            ).ToListAsync();
        return result;
    }

    public async Task<List<Machine>> GetAvalableMachine(int lineId)
    {
        var result = await _linq2Db.Machine.Where(s => s.LineId == 0 || s.ProcessId == 0 || s.LineId == lineId).ToListAsync();
        return result;
    }

    public async Task<List<Machine>> UpdateMachineLineByProduct(int lineId, int productId)
    {
        var machines = await GetInUseMachineByLine(lineId);
        if (machines.Count == 0) return new List<Machine>();

        var processes = await _processService.GetByProductId(productId);

        int minIndex = Math.Min(machines.Count, processes.Count);

        for (int i = 0; i < minIndex; i++)
        {
            machines[i].ProcessId = processes[i].Id;
        }

        if (machines.Count > minIndex)
        {
            for (int i = minIndex; i < machines.Count - minIndex; i++)
            {
                machines[i].LineId = 0;
                machines[i].ProcessId = 0;
            }
        }

        foreach (var machine in machines)
        {
            await Update(machine);
        }

        return machines;
    }

    public async Task<Machine> Create(MachineCreateModel model)
    {
        var machine = _mapper.Map<Machine>(model);
        await _linq2Db.InsertAsync(machine);
        return machine;
    }

    public async Task<Machine> Update(MachineUpdateModel model)
    {
        var machine = await GetById(model.Id);
        if (machine is null) throw new Exception("Machine is not found");

        var machineUpdate = _mapper.Map<Machine>(model);
        machineUpdate.Id = machine.Id;

        await _linq2Db.Update(machineUpdate);
        return machineUpdate;
    }

    public async Task UpdateById(int id, int departmentId, int lineId, int processId)
    {

        await _linq2Db.Machine.Where(s => s.Id == id)
                    .Set(s => s.DepartmentId, departmentId)
                    .Set(s => s.LineId, lineId)
                    .Set(s => s.ProcessId, processId)
                    .UpdateAsync();
    }

    public async Task UpdateByListId(IEnumerable<int> listId, int departmentId, int lineId, int processId)
    {

        await _linq2Db.Machine.Where(s => listId.Contains(s.Id))
                    .Set(s => s.DepartmentId, departmentId)
                    .Set(s => s.LineId, lineId)
                    .Set(s => s.ProcessId, processId)
                    .UpdateAsync();
    }

    public async Task<Machine> Update(Machine model)
    {
        await _linq2Db.Update(model);
        return model;
    }

    public async Task Delete(int id)
    {
        var machine = await GetById(id);
        if (machine is not null)
            await _linq2Db.DeleteAsync(machine);
    }

    public async Task Delete(List<int> listId)
    {
        await _linq2Db.Machine.Where(s => listId.Contains(s.Id)).DeleteAsync();
    }

}