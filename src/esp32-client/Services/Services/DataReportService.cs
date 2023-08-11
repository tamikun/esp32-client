using AutoMapper;
using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
using LinqToDB;

namespace esp32_client.Services;

/// <summary>
/// 
/// </summary>
public partial class DataReportService : IDataReportService
{

    private readonly LinqToDb _linq2Db;
    private readonly IMapper _mapper;

    public DataReportService(LinqToDb linq2Db, IMapper mapper)
    {
        _linq2Db = linq2Db;
        _mapper = mapper;
    }

    public async Task<List<DataReport>> GetAll()
    {
        return await _linq2Db.DataReport.ToListAsync();
    }

    public async Task<DataReport> GetLastDataByStationId(int stationId)
    {
        if (await _linq2Db.DataReport.Where(s => s.StationId == stationId).AnyAsync())
        {
            int maxId = await _linq2Db.DataReport.Where(s => s.StationId == stationId).MaxAsync(s => s.Id);
            var result = await _linq2Db.DataReport.Where(s => s.Id == maxId).FirstOrDefaultAsync() ?? new DataReport();
            return result;
        }
        return new DataReport();

    }

}