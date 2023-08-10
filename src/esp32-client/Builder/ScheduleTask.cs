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

// var scheduledTaskService = EngineContext.Resolve<IScheduleTaskService>();

// var rs = typeof(IScheduleTaskService)?.GetMethod(nameof(IScheduleTaskService.SaveProductData))
//                                     ?.Invoke(scheduledTaskService, new object[] { });

//             await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
//         }
//     }
// }



