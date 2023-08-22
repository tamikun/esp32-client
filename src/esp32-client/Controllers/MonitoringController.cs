using Microsoft.AspNetCore.Mvc;
using esp32_client.Services;
using esp32_client.Builder;
using LinqToDB;
using esp32_client.Models;

namespace esp32_client.Controllers;
[Authentication]
public class MonitoringController : BaseController
{
    private readonly ILineService _lineService;

    public MonitoringController(LinqToDb linq2db, ILineService lineService)
    {
        _linq2db = linq2db;
        _lineService = lineService;
    }


    public async Task<IActionResult> Index(int factoryId = 0, int pageIndex = 0, int pageSize = 5,
        bool iotMachine = false, bool normalMachine = false, bool emptyMachine = false, bool notEmptyMachine = false)
    {
        var filterModel = new MonitoringFilterModel();
        filterModel.IoTMachine = iotMachine;
        filterModel.NormalMachine = normalMachine;
        filterModel.EmptyMachine = emptyMachine;
        filterModel.EmptyMachine = emptyMachine;
        filterModel.NotEmptyMachine = notEmptyMachine;

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

    public async Task<IActionResult> Detail(int factoryId, int lineId)
    {
        var data = await _lineService.GetProcessAndMachineOfLine(factoryId, lineId);
        ViewBag.FactoryId = factoryId;
        ViewBag.LineId = lineId;
        return View(data);
    }

    public class MonitoringFilterModel
    {
        public bool IoTMachine { get; set; }
        public bool NormalMachine { get; set; }
        public bool EmptyMachine { get; set; }
        public bool NotEmptyMachine { get; set; }
    }

}
