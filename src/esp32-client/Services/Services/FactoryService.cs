using AutoMapper;
using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
using LinqToDB;

namespace esp32_client.Services;

/// <summary>
/// 
/// </summary>
public partial class FactoryService : IFactoryService
{

    private readonly LinqToDb _linq2Db;
    private readonly IMapper _mapper;

    public FactoryService(LinqToDb linq2Db, IMapper mapper)
    {
        _linq2Db = linq2Db;
        _mapper = mapper;
    }

    public async Task<Factory?> GetById(int id)
    {
        var department = await _linq2Db.Factory.Where(s => s.Id == id).FirstOrDefaultAsync();
        return department;
    }

    public async Task<List<Factory>> GetAll()
    {
        return await _linq2Db.Factory.ToListAsync();
    }

    public async Task Delete(int id)
    {
        var department = await GetById(id);
        if (department is not null)
            await _linq2Db.DeleteAsync(department);
    }

}