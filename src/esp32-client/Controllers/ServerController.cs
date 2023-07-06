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

    public IActionResult Detail(string ipAddress, string subDirectory)
    {

        var serverModelDetail = new ServerModelDetail()
        {
            IpAddress = ipAddress,
            SubDirectory = subDirectory,
        };
        System.Console.WriteLine("==== detail: " + Newtonsoft.Json.JsonConvert.SerializeObject(serverModelDetail));

        // Pass the selected item to the view
        return View(serverModelDetail);
    }

    [HttpPost]
    public async Task<IActionResult> Detail(ServerModelDetail requestModel)
    {
        System.Console.WriteLine("==== detail requestModel: " + Newtonsoft.Json.JsonConvert.SerializeObject(requestModel));

        if (requestModel?.RequestFileModel?.File is null)
            return RedirectToAction("Detail", new { ipAddress = requestModel?.IpAddress, subDirectory = requestModel?.SubDirectory });

        var listAlert = new List<AlertModel>();
        try
        {
            byte[] fileBytes;

            using (var ms = new MemoryStream())
            {
                requestModel.RequestFileModel.File.CopyTo(ms);
                fileBytes = ms.ToArray();
            }

            var result = await _clientService.PostAsyncFile(fileBytes, requestModel.RequestFileModel.FilePath, $"http://{requestModel.IpAddress}/");

            if (!result.IsSuccessStatusCode)
            {
                listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = "Error: " + result.StatusCode.ToString() });
            }
            else
            {
                listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = "Success" });
            }
        }
        catch
        {
            listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = "Time out" });
        }
        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);
        requestModel.RequestFileModel = null;

        return RedirectToAction("Detail", new { ipAddress = requestModel.IpAddress, subDirectory = requestModel.SubDirectory });
    }

    public async Task<IActionResult> Delete(string ipAddress, string subDirectory, string fileName)
    {

        var response = await _clientService.DeleteFile($"http://{ipAddress}/", subDirectory, fileName);

        List<AlertModel> alertModel = new List<AlertModel>();

        if (!response.IsSuccessStatusCode)
        {
            alertModel.Add(new AlertModel
            {
                AlertType = Alert.Danger,
                AlertMessage = response.StatusCode.ToString(),
            });

        }
        else
        {
            alertModel.Add(new AlertModel
            {
                AlertType = Alert.Success,
                AlertMessage = "Delete successful.",
            });

        }
        TempData["AlertMessage"] = JsonConvert.SerializeObject(alertModel);
        return RedirectToAction("Detail", new { ipAddress = ipAddress, subDirectory = subDirectory });
    }

    public async Task<IActionResult> Reload()
    {

        // await ListServer.GetInstance(_clientService).ReloadDynamicList();
        await ListServer.GetInstance(_clientService).ReloadStaticList();

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
