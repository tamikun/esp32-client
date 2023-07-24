using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using esp32_client.Models;
using esp32_client.Services;
using Newtonsoft.Json;
using esp32_client.Builder;

namespace esp32_client.Controllers;

[CustomAuthenticationFilter]
public class PatternController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPatternService _patternService;

    public PatternController(IHttpContextAccessor httpContextAccessor, IPatternService patternService)
    {
        _httpContextAccessor = httpContextAccessor;
        _patternService = patternService;
    }

    public async Task<IActionResult> Index()
    {
        await Task.CompletedTask;
        return View(new PatternIndexPageModel());
    }

    [HttpPost]
    public async Task<IActionResult> Add(PatternIndexPageModel model)
    {
        // Handle if there is no file name
        model.PatternCreateModel.FileName = string.IsNullOrEmpty(model.PatternCreateModel.FileName) ? model.PatternCreateModel.File.FileName : model.PatternCreateModel.FileName;

        var listAlert = new List<AlertModel>();
        try
        {
            await _patternService.Create(model.PatternCreateModel);
            listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = $"Add pattern successfully" });

        }
        catch (Exception ex)
        {
            listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"{ex.Message}" });
        }

        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);
        return RedirectToAction("Index");
    }


    [HttpPost]
    public async Task<IActionResult> Delete(PatternIndexPageModel model)
    {

        var listAlert = new List<AlertModel>();

        var listId = model.ListDeletePatternById.Where(s => s.IsSelected == true).Select(s => s.Id).ToList();

        try
        {
            await _patternService.Delete(listId);
            listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = $"Delete patterns successfully" });

        }
        catch (Exception ex)
        {
            listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"{ex.Message}" });
        }

        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);
        return RedirectToAction("Index");
    }


    [HttpGet]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Download(int id)
    {
        var pattern = await _patternService.GetById(id);
        if (pattern is null) return Ok();

        var format = pattern.FileName.Split('.').LastOrDefault() ?? "";

        return File(Convert.FromBase64String(pattern.FileData), Utils.Utils.GetContentType(format), fileDownloadName: pattern.FileName);
    }

}
