using Microsoft.AspNetCore.Mvc;
using esp32_client.Services;
using esp32_client.Models;
using Newtonsoft.Json;

namespace esp32_client.Controllers;
[CustomAuthenticationFilter]
public class LineController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILineService _lineService;
    private readonly IProductService _productService;
    private readonly IMachineService _machineService;

    public LineController(IHttpContextAccessor httpContextAccessor, ILineService lineService, IProductService productService, IMachineService machineService)
    {
        _httpContextAccessor = httpContextAccessor;
        _lineService = lineService;
        _productService = productService;
        _machineService = machineService;
    }


    public ActionResult Index()
    {
        var model = new LineCreateModel();
        model.DepartmentId = 1;
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Add(LineCreateModel model)
    {
        var listAlert = new List<AlertModel>();
        try
        {
            var LineDetail = await _lineService.Create(model);
            listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = $"Add line" });
        }
        catch (Exception ex)
        {
            listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"{ex.Message}" });
        }
        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int id)
    {
        var listAlert = new List<AlertModel>();
        try
        {
            await _lineService.Delete(id);
            listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = $"Delete line" });
        }
        catch (Exception ex)
        {
            listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"{ex.Message}" });
        }
        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> UpdateProductLine(int id)
    {
        var model = new LineUpdateModel();
        model.DepartmentId = 1;
        model.Id = id;

        await Task.CompletedTask;

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProductLine(LineUpdateModel model)
    {
        var listAlert = new List<AlertModel>();
        try
        {
            // Auto transfer machine
            var machines = await _machineService.UpdateMachineLineByProduct(model.Id, model.ProductId);

            await _lineService.Update(model);

            listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = $"Update product line" });
        }
        catch (Exception ex)
        {
            listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"{ex.Message}" });
        }
        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> UpdateMachineLine(int id)
    {
        var model = new UpdateMachineLineModel();
        model.DepartmentId = 1;

        model.ListProcessAndMachineOfLine = await _lineService.GetProcessAndMachineOfLine(model.DepartmentId, id);
        model.ListProcessAndMachineOfLine = model.ListProcessAndMachineOfLine.Where(s => s.ProcessId != 0).ToList();

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateMachineLine(UpdateMachineLineModel model)
    {
        System.Console.WriteLine("==== UpdateMachineLine: " + Newtonsoft.Json.JsonConvert.SerializeObject(model));
        var listAlert = new List<AlertModel>();
        try
        {
            // Validate data
            var duplicateMachineId = model.ListProcessAndMachineOfLine.Where(s => s.MachineId != 0)
                                                                    .GroupBy(s => s.MachineId)
                                                                    .Where(g => g.Count() > 1)
                                                                    .Select(s => s.Key);

            if (duplicateMachineId.Any())
                throw new Exception("Duplicate machine");

            int lineId = model.ListProcessAndMachineOfLine.Select(s => s.LineId).FirstOrDefault();

            // Update
            var oldList = await _lineService.GetProcessAndMachineOfLine(model.DepartmentId, lineId);

            var listNewId = model.ListProcessAndMachineOfLine.Select(s => s.MachineId);

            // Release machine
            var listReleaseMachineId = oldList.Where(s => !listNewId.Contains(s.MachineId) && s.MachineId != 0).Select(s => s.MachineId);
            await _machineService.UpdateByListId(listReleaseMachineId, model.DepartmentId, 0, 0);

            foreach (var item in model.ListProcessAndMachineOfLine)
            {
                // Update machine and process
                await _machineService.UpdateById(item.MachineId, model.DepartmentId, item.LineId, item.ProcessId);
            }

            listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = $"Update machine line" });
        }
        catch (Exception ex)
        {
            listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"{ex.Message}" });
        }
        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);

        return RedirectToAction("Index");
    }

}
