// using Microsoft.Extensions.Hosting;
// namespace esp32_client.Builder;

// public class ScheduledTaskService : BackgroundService
// {
//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         while (!stoppingToken.IsCancellationRequested)
//         {
//             // Perform your specific task here
//             Console.WriteLine($"Task executed at: {DateTime.Now}");

//             var linq2Db = EngineContext.Resolve<LinqToDb>();
//             var user = linq2Db.UserAccount.FirstOrDefault(s => s.Id == 1);
//             System.Console.WriteLine($"Username {user?.UserName}");

//             await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
//         }
//     }
// }



