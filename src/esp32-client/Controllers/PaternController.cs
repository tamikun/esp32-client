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
        return View(new PaternIndexPageModel());
    }

    [HttpPost]
    public async Task<IActionResult> Add(PaternIndexPageModel model)
    {
        // Handle if there is no file name
        model.PaternCreateModel.FileName = string.IsNullOrEmpty(model.PaternCreateModel.FileName) ? model.PaternCreateModel.File.FileName : model.PaternCreateModel.FileName;

        var listAlert = new List<AlertModel>();
        try
        {
            await _paternService.Create(model.PaternCreateModel);
            listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = $"Add patern successfully" });

        }
        catch (Exception ex)
        {
            listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"{ex.Message}" });
        }

        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);
        return RedirectToAction("Index");
    }


    [HttpPost]
    public async Task<IActionResult> Delete(PaternIndexPageModel model)
    {

        var listAlert = new List<AlertModel>();

        var listId = model.ListDeletePaternById.Where(s => s.IsSelected == true).Select(s => s.Id).ToList();

        try
        {
            await _paternService.Delete(listId);
            listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = $"Delete paterns successfully" });

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
        var patern = await _paternService.GetById(id);
        if (patern is null) return Ok();

        var format = patern.FileName.Split('.').LastOrDefault() ?? "";

        return File(Convert.FromBase64String(patern.FileData), Utils.Utils.GetContentType(format), fileDownloadName: patern.FileName);
    }

}
