using esp32_client.Builder;
using esp32_client.Domain;
using LinqToDB;

namespace esp32_client.Services;

public partial class ScheduleTaskService : IScheduleTaskService
{

    private readonly LinqToDb _linq2db;
    private readonly IMachineService _machineService;

    public ScheduleTaskService(LinqToDb linq2Db, IMachineService machineService)
    {
        _linq2db = linq2Db;
        _machineService = machineService;
    }

    public async Task SaveProductData()
    {
        var listDataReport = new List<DataReport>();
        var queryData = await (from machine in _linq2db.Entity<Machine>()
                               join station in _linq2db.Entity<Station>() on machine.StationId equals station.Id
                               select new { machine.IpAddress, station.Id }).ToListAsync();

        var tasks = queryData.Select(async s =>
        {
            var getData = await _machineService.GetProductNumberMachine(s.IpAddress);

            if (getData.Success)
                listDataReport.Add(new DataReport { StationId = s.Id, ProductNumber = getData.Data, DateTimeUtc = DateTime.UtcNow });

        });

        await Task.WhenAll(tasks);

        if (listDataReport.Count > 0)
            await _linq2db.BulkInsert(listDataReport);
    }

}