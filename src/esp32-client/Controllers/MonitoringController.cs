using Microsoft.AspNetCore.Mvc;
using esp32_client.Services;
using esp32_client.Builder;

namespace esp32_client.Controllers;
[CustomAuthenticationFilter]
public class MonitoringController : BaseController
{
    private readonly IFactoryService _departmentService;
    private readonly IScheduleTaskService _scheduleTaskService;

    public MonitoringController(LinqToDb linq2db, IFactoryService departmentService, IScheduleTaskService scheduleTaskService)
    {
        _linq2db = linq2db;
        _departmentService = departmentService;
        _scheduleTaskService = scheduleTaskService;
    }


    public async Task<IActionResult> Index(int factoryId = 0)
    {
        ViewBag.FactoryId = factoryId;
        try
        {
            await _scheduleTaskService.SaveProductData();
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("==== SaveProductData: " + Newtonsoft.Json.JsonConvert.SerializeObject(ex));
        }
        
        return View();
    }

}
