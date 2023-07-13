using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using esp32_client.Models;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;
using esp32_client.Services;

namespace esp32_client.Controllers;
[CustomAuthenticationFilter]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly Settings _settings;
    private readonly ListServer _listServer;

    public HomeController(ILogger<HomeController> logger, Settings settings, ListServer listServer)
    {
        _logger = logger;
        _settings = settings;
        _listServer = listServer;
    }

    public async Task<IActionResult> Index()
    {
        await Task.CompletedTask;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> LoadIndexData()
    {
        System.Console.WriteLine("Get Load INDEX DATA");
        await _listServer.GetStaticList();
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Index(MultipleUploadFileModel? fileModel)
    {
        System.Console.WriteLine("Call Home-Index-Post");

        System.Console.WriteLine("==== fileModel: " + JsonConvert.SerializeObject(fileModel));

        await Task.CompletedTask;

        return RedirectToAction("Index");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
