// using esp32_client.Services;
// using LinqToDB;

// namespace esp32_client.Builder;

// public class ScheduledTaskService : BackgroundService
// {
//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         var linq2Db = EngineContext.Resolve<LinqToDb>();
//         var scheduledTaskService = EngineContext.Resolve<IScheduleTaskService>();

//         while (!stoppingToken.IsCancellationRequested)
//         {
//             var listScheduleTask = await linq2Db.ScheduleTask.Where(s => s.Enabled == true && (s.LastStartUtc == null || s.LastStartUtc.Value.AddSeconds(s.Seconds) <= DateTime.UtcNow)).ToListAsync();
//             var tasks = listScheduleTask.Select(async s =>
//             {
//                 s.LastStartUtc = DateTime.UtcNow;
//                 try
//                 {
//                     var invoke = (Task?)typeof(IScheduleTaskService)?.GetMethod(s.Method)?.Invoke(scheduledTaskService, new object[] { });
//                     if (invoke is not null)
//                         await invoke.ConfigureAwait(true);

//                     s.LastSuccessUtc = DateTime.UtcNow;
//                 }
//                 catch (Exception ex)
//                 {
//                     System.Console.WriteLine($"{s.Method} Invoke - {ex.Message}");
//                 }

//                 try
//                 {
//                     await linq2Db.Update(s);
//                 }
//                 catch (Exception ex)
//                 {
//                     System.Console.WriteLine($"{s.Method} Update - {ex.Message}");
//                 }

//             });

//             await Task.WhenAll(tasks);
//         }
//     }
// }



