using Microsoft.AspNetCore.Mvc;
using esp32_client.Services;
using esp32_client.Builder;
using esp32_client.Models;

namespace esp32_client.Controllers;

[CustomAuthenticationFilter]
public class AssigningController : BaseController
{
    private readonly IFactoryService _departmentService;
    private readonly ILineService _lineService;
    private readonly IMachineService _machineService;

    public AssigningController(LinqToDb linq2db, IFactoryService departmentService, ILineService lineService, IMachineService machineService)
    {
        _linq2db = linq2db;
        _departmentService = departmentService;
        _lineService = lineService;
        _machineService = machineService;
    }

    public async Task<IActionResult> ProductLine(int factoryId = 0, bool edit = false)
    {
        ViewBag.FactoryId = factoryId;
        ViewBag.Edit = edit;
        await Task.CompletedTask;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AssignProductLine(AssignProductLineModel model)
    {
        return await HandleActionAsync(async () =>
        {
            await _lineService.AssignProductLine(model);
        }, RedirectToAction("ProductLine", new { factoryId = model.FactoryId }));
    }

    public async Task<IActionResult> MachineLine(int factoryId = 0, int lineId = 0, bool edit = false)
    {
        ViewBag.FactoryId = factoryId;
        ViewBag.LineId = lineId;
        ViewBag.Edit = edit;
        await Task.CompletedTask;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AssignMachineLine(ListAssignMachineLineModel model)
    {

        return await HandleActionAsync(async () =>
        {
            await _machineService.AssignMachineLine(model);
        }, RedirectToAction("MachineLine", new { factoryId = model.FactoryId, lineId = model.LineId }));
    }

}
