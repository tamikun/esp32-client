using AutoMapper;
using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
using LinqToDB;

namespace esp32_client.Services;

/// <summary>
/// 
/// </summary>
public partial class ProcessService : IProcessService
{

    private readonly LinqToDb _linq2Db;
    private readonly IMapper _mapper;

    public ProcessService(LinqToDb linq2Db, IMapper mapper)
    {
        _linq2Db = linq2Db;
        _mapper = mapper;
    }

    public async Task<Process?> GetById(int id)
    {
        var process = await _linq2Db.Process.Where(s => s.Id == id).FirstOrDefaultAsync();
        return process;
    }

    public async Task<Process?> GetByProcessName(string name)
    {
        var process = await _linq2Db.Process.Where(s => s.ProcessName == name).FirstOrDefaultAsync();
        return process;
    }

    public async Task<List<Process>> GetAll()
    {
        return await _linq2Db.Process.ToListAsync();
    }

    public async Task<ProcessCreateModel> Create(ProcessCreateModel model)
    {
        var process = new Process()
        {
            ProductId = model.ProductId,
            ProcessName = model.ProcessName,
            PatternId = model.PatternId,
            Order = model.Order,
        };

        await _linq2Db.InsertAsync(process);

        return model;
    }

    public async Task<ProcessUpdateModel> Update(ProcessUpdateModel model)
    {
        var process = await GetById(model.Id);

        if (process is null) throw new Exception("Process is not found");

        process.ProductId = model.ProductId;
        process.ProcessName = model.ProcessName;
        process.PatternId = model.PatternId;
        process.Order = model.Order;

        await _linq2Db.Update(process);

        return model;
    }

    public async Task Delete(int id)
    {
        var process = await GetById(id);

        if (process is null) return;
        await _linq2Db.DeleteAsync(process);
    }
}