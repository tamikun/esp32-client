using Microsoft.AspNetCore.Mvc;
using esp32_client.Services;
using esp32_client.Builder;
using esp32_client.Models;
using Newtonsoft.Json;

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

    public async Task<IActionResult> Index()
    {
        await Task.CompletedTask;
        return View();
    }

    public async Task<IActionResult> StationProcess(int factoryId = 0, int lineId = 0, bool edit = false)
    {
        ViewBag.FactoryId = factoryId;
        ViewBag.LineId = lineId;
        ViewBag.Edit = edit;
        await Task.CompletedTask;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AssignStationProcess(AssignStationProcessModel model)
    {
        var listAlert = new List<AlertModel>();
        try
        {
            var result = await _lineService.AssignStationProcess(model);
            foreach (var item in result)
            {
                listAlert.Add(new AlertModel { AlertType = item.Value == "Success" ? Alert.Success : Alert.Danger, AlertMessage = item.Value });
            }
        }
        catch (Exception ex)
        {
            listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = ex.Message });
        }

        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);

        return RedirectToAction("StationProcess", new { factoryId = model.FactoryId, lineId = model.LineId });
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
        var listAlert = new List<AlertModel>();
        try
        {
            var result = await _lineService.AssignProductLine(model);
            foreach (var item in result)
            {
                listAlert.Add(new AlertModel { AlertType = item.Value == "Success" ? Alert.Success : Alert.Danger, AlertMessage = item.Value });
            }
        }
        catch (Exception ex)
        {
            listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = ex.Message });
        }

        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);

        return RedirectToAction("ProductLine", new { factoryId = model.FactoryId });
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

        var listAlert = new List<AlertModel>();
        try
        {
            var result = await _machineService.AssignMachineLine(model);
            foreach (var item in result)
            {
                listAlert.Add(new AlertModel { AlertType = item.Value == "Success" ? Alert.Success : Alert.Danger, AlertMessage = item.Value });
            }
        }
        catch (Exception ex)
        {
            listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = ex.Message });
        }

        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);

        return RedirectToAction("MachineLine", new { factoryId = model.FactoryId, lineId = model.LineId });
    }

}
