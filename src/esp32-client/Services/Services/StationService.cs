using AutoMapper;
using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
using LinqToDB;

namespace esp32_client.Services;

/// <summary>
/// 
/// </summary>
public partial class StationService : IStationService
{

    private readonly LinqToDb _linq2Db;
    private readonly IMapper _mapper;

    public StationService(LinqToDb linq2Db, IMapper mapper)
    {
        _linq2Db = linq2Db;
        _mapper = mapper;
    }

    public async Task<Station?> GetById(int id)
    {
        var department = await _linq2Db.Station.Where(s => s.Id == id).FirstOrDefaultAsync();
        return department;
    }

    public async Task<List<Station>> GetAll()
    {
        return await _linq2Db.Station.ToListAsync();
    }

    public async Task<List<Station>> GetByLineId(int lineId)
    {
        return await _linq2Db.Station.Where(s => s.LineId == lineId).ToListAsync();
    }

    public async Task UpdateStationName(StationUpdateModel model)
    {
        var department = await _linq2Db.Station.Where(s => s.Id == model.Id).Set(s => s.StationName, model.StationName).UpdateAsync();
    }
    
}