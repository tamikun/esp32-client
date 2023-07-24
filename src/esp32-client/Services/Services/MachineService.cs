using System.Security.Cryptography;
using System.Text;
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

    public MachineService(LinqToDb linq2Db, IMapper mapper)
    {
        _linq2Db = linq2Db;
        _mapper = mapper;
    }

    public async Task<List<Machine>> GetAll()
    {
        return await _linq2Db.Machine.ToListAsync();
    }

    public async Task<Machine> Create(Machine model)
    {

        await _linq2Db.InsertAsync(model);

        return model;
    }

    public async Task<Machine?> GetById(int id)
    {
        var machine = await _linq2Db.Machine.Where(s => s.Id == id).FirstOrDefaultAsync();
        return machine;
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