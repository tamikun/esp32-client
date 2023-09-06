using AutoMapper;
using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
using esp32_client.Models.Singleton;
using LinqToDB;

namespace esp32_client.Services;

public partial class FactoryService : IFactoryService
{

    private readonly LinqToDb _linq2db;
    private readonly IMapper _mapper;
    private readonly ILineService _lineService;
    private readonly Settings _settings;

    public FactoryService(LinqToDb linq2Db, IMapper mapper, ILineService lineService, Settings settings)
    {
        _linq2db = linq2Db;
        _mapper = mapper;
        _lineService = lineService;
        _settings = settings;
    }

    public async Task<Factory> GetById(int id)
    {
        var department = await _linq2db.Entity<Factory>().Where(s => s.Id == id).FirstOrDefaultAsync();
        return department;
    }

    public async Task<List<Factory>> GetAll()
    {
        return await _linq2db.Entity<Factory>().ToListAsync();
    }

    public async Task<List<FactoryResponseModel>> GetAllResponse()
    {
        var result = await (from factory in _linq2db.Entity<Factory>()
                            select new FactoryResponseModel
                            {
                                FactoryId = factory.Id,
                                FactoryName = factory.FactoryName,
                                FactoryNo = factory.FactoryNo,
                                NumberOfLine = _linq2db.Entity<Line>().Where(s => s.FactoryId == factory.Id).Count(),
                            }).ToListAsync();
        return result;
    }

    public async Task<Factory> Create(int factoryNo, string factoryName)
    {
        if (string.IsNullOrEmpty(factoryName)) throw new Exception("Invalid name");

        if (factoryNo == 0) throw new Exception("Invalid Factory No");

        var factory = new Factory();
        factory.FactoryName = factoryName;

        string formattedNumber = factoryNo.ToString($"D{_settings.MinCharFactoryFormat}");

        factory.FactoryNo = string.Format(_settings.FactoryFormat, formattedNumber);

        factory = await _linq2db.Insert(factory);
        return factory;
    }

    public async Task Delete(int id)
    {
        var factory = await GetById(id);
        if (factory is null) throw new Exception("Factory is not found");

        // Check any lines are in use
        if (await _linq2db.Entity<Line>().Where(s => s.FactoryId == id && s.ProductId != 0).AnyAsync())
            throw new Exception("Line is in use");

        var listDeleteLine = await _linq2db.Entity<Line>().Where(s => s.FactoryId == id).ToListAsync();

        foreach (var line in listDeleteLine)
        {
            await _lineService.Delete(line.Id);
        }

        await _linq2db.Delete(factory);
    }

    public async Task UpdateName(int id, string name)
    {
        if (string.IsNullOrEmpty(name)) throw new Exception("Invalid name");

        var factory = await GetById(id);
        if (factory is null) throw new Exception("Factory is not found");

        factory.FactoryName = name;
        await _linq2db.Update(factory);
    }

}