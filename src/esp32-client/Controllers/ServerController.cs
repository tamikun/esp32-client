using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using esp32_client.Models;
using esp32_client.Services;
using Newtonsoft.Json;

namespace esp32_client.Controllers;

public class ServerController : Controller
{
    private readonly ILogger<ServerController> _logger;
    private readonly IClientService _clientService;

    public ServerController(ILogger<ServerController> logger, IClientService clientService)
    {
        _logger = logger;
        _clientService = clientService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Index(RequestFileModel? fileModel)
    // public IActionResult Index(IFormFile? File)
    {
        System.Console.WriteLine("Call Server-Index-Post");
        System.Console.WriteLine("==== file: " + JsonConvert.SerializeObject(fileModel.File));
        System.Console.WriteLine("==== file: " + JsonConvert.SerializeObject(fileModel.FilePath));

        return View();
    }

    public IActionResult Detail(string ipAddress, string subDirectory)
    {

        var serverModelDetail = new ServerModelDetail()
        {
            IpAddress = ipAddress,
            SubDirectory = subDirectory,
            // AlertMessage = alertMessage
            AlertMessage = TempData["AlertMessage"]?.ToString()
        };
        System.Console.WriteLine("==== detail: " + Newtonsoft.Json.JsonConvert.SerializeObject(serverModelDetail));

        // Pass the selected item to the view
        return View(serverModelDetail);
    }

    public async Task<IActionResult> Delete(string ipAddress, string subDirectory, string fileName)
    {
        string url = "";

        if (string.IsNullOrEmpty(subDirectory))
        {
            url = $"http://{ipAddress}/delete/{fileName}";
        }
        else
        {
            url = $"http://{ipAddress}/delete/{subDirectory}/{fileName}";
        }

        System.Console.WriteLine("==== delete: " + url);

        var response = await _clientService.PostAsyncApi(requestBody: null, apiUrl: url);

        AlertModel alertModel = new AlertModel();

        if (!response.IsSuccessStatusCode)
        {
            alertModel.AlertType = Alert.Danger;
            alertModel.AlertMessage = response.StatusCode.ToString();
        }
        else
        {
            alertModel.AlertType = Alert.Success;
            alertModel.AlertMessage = "Delete successful.";
        }

        var alertString = JsonConvert.SerializeObject(alertModel);

        TempData["AlertMessage"] = alertString;

        // Pass the selected item to the view
        // return RedirectToAction("Detail", new { ipAddress = ipAddress, subDirectory = subDirectory, alertMessage = alertString });
        return RedirectToAction("Detail", new { ipAddress = ipAddress, subDirectory = subDirectory });
    }

    public async Task<IActionResult> Reload()
    {

        ListServer.GetInstance(_clientService).ReloadItemList();
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
