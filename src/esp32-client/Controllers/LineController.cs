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

    public LineController(IHttpContextAccessor httpContextAccessor, ILineService lineService)
    {
        _httpContextAccessor = httpContextAccessor;
        _lineService = lineService;
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
            listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = $"Add Line successfully" });
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
            listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = $"Delete Line successfully" });
        }
        catch (Exception ex)
        {
            listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"{ex.Message}" });
        }
        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);

        return RedirectToAction("Index");
    }

}
