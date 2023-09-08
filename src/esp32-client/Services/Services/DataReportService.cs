using AutoMapper;
using esp32_client.Builder;
using esp32_client.Domain;
using LinqToDB;

namespace esp32_client.Services;

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
        return await _linq2Db.Entity<DataReport>().ToListAsync();
    }

    public async Task Create(string IPAddress, int productNumber)
    {
        var machine = await _linq2Db.Entity<Machine>().Where(s => s.IpAddress == IPAddress).FirstOrDefaultAsync();
        if (machine is not null)
        {
            if (machine.StationId != 0)
            {
                var data = new DataReport
                {
                    StationId = machine.StationId,
                    ProductNumber = productNumber,
                    DateTimeUtc = DateTime.UtcNow,
                };
                await _linq2Db.Insert(data);
            }
        }
    }

    public async Task<DataReport> GetLastDataByStationId(int stationId)
    {
        if (stationId <= 0) return new DataReport();
        if (await _linq2Db.Entity<DataReport>().Where(s => s.StationId == stationId).AnyAsync())
        {
            int maxId = await _linq2Db.Entity<DataReport>().Where(s => s.StationId == stationId).MaxAsync(s => s.Id);
            var result = await _linq2Db.Entity<DataReport>().Where(s => s.Id == maxId).FirstOrDefaultAsync() ?? new DataReport();
            return result;
        }
        return new DataReport();
    }

}