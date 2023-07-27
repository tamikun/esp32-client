using Microsoft.AspNetCore.Mvc;
using esp32_client.Services;
using esp32_client.Models;
using Newtonsoft.Json;
using AutoMapper;

namespace esp32_client.Controllers;

[CustomAuthenticationFilter]
public class MachineController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMachineService _machineService;
    private readonly IMapper _mapper;

    public MachineController(IHttpContextAccessor httpContextAccessor, IMachineService machineService, IMapper mapper)
    {
        _httpContextAccessor = httpContextAccessor;
        _machineService = machineService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        await Task.CompletedTask;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Add(MachineCreateModel model)
    {
        var listAlert = new List<AlertModel>();
        try
        {
            var machineDetail = await _machineService.Create(model);
            listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = $"Add machine" });
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
            await _machineService.Delete(id);
            listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = $"Delete machine" });
        }
        catch (Exception ex)
        {
            listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"{ex.Message}" });
        }
        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);

        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Update(int id)
    {
        var machine = await _machineService.GetById(id);
        var productUpdateModel = _mapper.Map<MachineUpdateModel>(machine);
        return View(productUpdateModel);
    }

    [HttpPost]
    public async Task<IActionResult> Update(MachineUpdateModel model)
    {
        var listAlert = new List<AlertModel>();
        try
        {
            var machine = await _machineService.Update(model);
            listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = $"Update product" });
        }
        catch (Exception ex)
        {
            listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"{ex.Message}" });
        }
        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);
        return RedirectToAction("Index");
    }
}
