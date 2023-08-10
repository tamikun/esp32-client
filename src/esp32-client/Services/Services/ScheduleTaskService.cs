using esp32_client.Builder;

namespace esp32_client.Services;

/// <summary>
/// 
/// </summary>
public partial class ScheduleTaskService : IScheduleTaskService
{

    private readonly LinqToDb _linq2Db;

    public ScheduleTaskService(LinqToDb linq2Db)
    {
        _linq2Db = linq2Db;
    }

    public async Task SaveProductData()
    {
        System.Console.WriteLine("========= Save product data ==========");
        await Task.CompletedTask;
    }

}