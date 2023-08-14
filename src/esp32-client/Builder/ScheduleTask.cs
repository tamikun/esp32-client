using esp32_client.Services;
using LinqToDB;

namespace esp32_client.Builder;

public class ScheduledTaskService : BackgroundService
{
    private const int BatchSize = 2; // Number of tasks to process in each batch

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var linq2Db = EngineContext.Resolve<LinqToDb>();
        var scheduledTaskService = EngineContext.Resolve<IScheduleTaskService>();
        var logService = EngineContext.Resolve<ILogService>();
        int minDelay = await linq2Db.ScheduleTask.MinAsync(s => s.Seconds);
        minDelay = minDelay < 60 ? 60 : minDelay;

        while (!stoppingToken.IsCancellationRequested)
        {
            var listScheduleTask = await linq2Db.ScheduleTask
                .Where(s => s.Enabled == true && (s.LastStartUtc == null || s.LastStartUtc.Value.AddSeconds(s.Seconds) <= DateTime.UtcNow))
                .Take(BatchSize) // Fetch tasks in batches
                .ToListAsync();

            var tasks = listScheduleTask.Select(async s =>
            {
                s.LastStartUtc = DateTime.UtcNow;
                try
                {
                    var invoke = (Task?)typeof(IScheduleTaskService)?.GetMethod(s.Method)?.Invoke(scheduledTaskService, new object[] { });
                    if (invoke is not null)
                        await invoke.ConfigureAwait(true);

                    s.LastSuccessUtc = DateTime.UtcNow;
                    await linq2Db.Update(s);
                }
                catch (Exception ex)
                {
                    await logService.AddLog(ex);
                }
            });

            await Task.WhenAll(tasks);
            await Task.Delay(TimeSpan.FromSeconds(minDelay), stoppingToken);
        }
    }
}

