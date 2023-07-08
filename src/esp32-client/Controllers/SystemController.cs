using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using esp32_client.Models;
using esp32_client.Services;
using Newtonsoft.Json;

namespace esp32_client.Controllers;

public class SystemController : Controller
{
    private readonly ILogger<SystemController> _logger;
    private readonly IFileService _fileService;
    private readonly IConfiguration _configuration;

    public SystemController(ILogger<SystemController> logger, IFileService fileService, IConfiguration configuration)
    {
        _logger = logger;
        _fileService = fileService;
        _configuration = configuration;
    }

    public IActionResult Index(string? folder = null)
    {
        var model = new FileSystemRequestModel();
        model.Folder = folder ?? _configuration["Settings:FileDataDirectory"].ToString();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Upload(FileSystemRequestModel request)
    {
        var listAlert = new List<AlertModel>();
        var tasks = request.ListUploadFile.Select(async file =>
        {
            await _fileService.WriteFile(file, $"{request.Folder}/");
            listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = $"Upload {file.FileName}" });
        });
        await Task.WhenAll(tasks);
        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);

        return RedirectToAction("Index", new { folder = request.Folder });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(FileSystemRequestModel request)
    {
        var listAlert = new List<AlertModel>();

        var tasks = request.ListDeleteFile.Where(s => s.IsSelected).Select(async file =>
        {
            await _fileService.DeleteFile(file.FilePath);

            var fileName = file.FilePath.Split('/').LastOrDefault();
            listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = $"Delete file {fileName}" });
        });

        await Task.WhenAll(tasks);
        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);
        return RedirectToAction("Index", new { folder = request.Folder });
    }

    public async Task<IActionResult> AddFolder(string directory, string folderName)
    {
        var listAlert = new List<AlertModel>();
        if (Directory.Exists(directory + folderName))
        {
            listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = "Folder has already exited!" });
        }
        else
        {
            Directory.CreateDirectory($"{directory}/{folderName}");
            listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = $"Add {folderName}" });
        }
        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);
        await Task.CompletedTask;
        return RedirectToAction("Index", new { folder = directory });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
