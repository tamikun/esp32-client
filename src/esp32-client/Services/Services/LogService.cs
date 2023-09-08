using esp32_client.Builder;
using esp32_client.Domain;
using LinqToDB;

namespace esp32_client.Services;

public partial class LogService : ILogService
{

    private readonly LinqToDb _linq2Db;

    public LogService(LinqToDb linq2Db)
    {
        _linq2Db = linq2Db;
    }

    public async Task AddLog(Exception ex)
    {
        var log = new Log();

        log.Message = ex.Message;
        log.FullMessage = ex.ToString();
        log.DateTimeUtc = DateTime.UtcNow;

        await _linq2Db.Insert(log);
    }

}