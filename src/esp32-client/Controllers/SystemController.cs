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
        System.Console.WriteLine("==== request: " + Newtonsoft.Json.JsonConvert.SerializeObject(request));

        var tasks = request.ListUploadFile.Select(async file =>
        {
            await _fileService.WriteFile(file, request.Folder);
        });

        await Task.WhenAll(tasks);

        return RedirectToAction("Index", new { folder = request.Folder });
    }

    [HttpPost]
    public IActionResult Delete(FileSystemRequestModel request)
    {
        System.Console.WriteLine("==== files: " + Newtonsoft.Json.JsonConvert.SerializeObject(request));

        return RedirectToAction("Index", new { folder = request.Folder });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
