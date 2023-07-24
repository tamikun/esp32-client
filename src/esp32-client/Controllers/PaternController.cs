using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using esp32_client.Models;
using esp32_client.Services;
using Newtonsoft.Json;
using esp32_client.Builder;

namespace esp32_client.Controllers;

[CustomAuthenticationFilter]
public class PaternController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPaternService _paternService;

    public PaternController(IHttpContextAccessor httpContextAccessor, IPaternService paternService)
    {
        _httpContextAccessor = httpContextAccessor;
        _paternService = paternService;
    }

    public async Task<IActionResult> Index()
    {
        await Task.CompletedTask;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Add(PaternCreateModel model)
    {
        // Handle if there is no file name
        model.FileName = string.IsNullOrEmpty(model.FileName) ? model.File.FileName : model.FileName;

        var listAlert = new List<AlertModel>();
        try
        {
            await _paternService.Create(model);
            listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = $"Add patern successfully" });

        }
        catch (Exception ex)
        {
            listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"{ex.Message}" });
        }

        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);
        return RedirectToAction("Index");
    }

}
