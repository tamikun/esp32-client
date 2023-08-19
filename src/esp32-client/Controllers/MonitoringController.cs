using Microsoft.AspNetCore.Mvc;
using esp32_client.Services;
using esp32_client.Builder;
using LinqToDB;

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


    public async Task<IActionResult> Index(int factoryId = 0, int pageIndex = 0, int pageSize = 5, bool iotMachine = true, bool normalMachine = true, bool emptyMachine = true)
    {
        var filterModel = new MonitoringFilterModel();
        filterModel.IoTMachine = iotMachine;
        filterModel.NormalMachine = normalMachine;
        filterModel.EmptyMachine = emptyMachine;

        if (factoryId == 0)
        {
            var getFirst = await _linq2db.Factory.FirstOrDefaultAsync();
            if (getFirst is not null) factoryId = getFirst.Id;
        }

        ViewBag.FactoryId = factoryId;
        ViewBag.PageIndex = pageIndex;
        ViewBag.PageSize = pageSize;

        return View(filterModel);
    }

    public class MonitoringFilterModel
    {
        public bool IoTMachine { get; set; }
        public bool NormalMachine { get; set; }
        public bool EmptyMachine { get; set; }
    }

}
