﻿using System.Diagnostics;
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
    public async Task<IActionResult> Upload(ServerModelDetail requestModel)
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

            var result = await _clientService.PostAsyncFile(fileBytes, $"{requestModel.SubDirectory}/{requestModel.RequestFileModel.FilePath}", $"http://{requestModel.IpAddress}/");

            if (!result.IsSuccessStatusCode)
            {
                listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = result.StatusCode.ToString() });
            }
            else
            {
                listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = "" });
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

    [HttpPost]
    public async Task<IActionResult> Delete(ServerModelDetail requestModel)
    {
        System.Console.WriteLine("==== requestModel: " + Newtonsoft.Json.JsonConvert.SerializeObject(requestModel));

        List<AlertModel> alertModel = new List<AlertModel>();

        var deleteTasks = requestModel?.ListDeleteFile?.Where(s => s.IsSelected).Select(async item =>
        {
            var delete = _clientService.DeleteFile($"http://{requestModel.IpAddress}/", requestModel.SubDirectory, item.FileName);
            var deleteResult = await delete;

            if (!deleteResult.IsSuccessStatusCode)
            {
                alertModel.Add(new AlertModel
                {
                    AlertType = Alert.Danger,
                    AlertMessage = deleteResult.StatusCode.ToString(),
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
        });

        if (deleteTasks is not null)
            await Task.WhenAll(deleteTasks);

        // var response = await _clientService.DeleteFile($"http://{ipAddress}/", subDirectory, fileName);

        TempData["AlertMessage"] = JsonConvert.SerializeObject(alertModel);
        return RedirectToAction("Detail", new { ipAddress = requestModel?.IpAddress, subDirectory = requestModel?.SubDirectory });
    }

    public async Task<IActionResult> Reload()
    {

        // await ListServer.GetInstance(_clientService).ReloadDynamicList();
        var rs = await ListServer.GetInstance(_clientService).ReloadStaticList();

        return RedirectToAction("Index");
    }


    public async Task<IActionResult> ChangeState(string ipAddress)
    {
        try
        {
            var requestUrl = $"http://{ipAddress}/";
            System.Console.WriteLine("Call change state: " + requestUrl);

            var currentState = await _clientService.GetServerState(requestUrl);

            if (currentState == ServerState.Server)
            {
                await _clientService.GetAsyncApi($"{requestUrl}selectedMachine");
                await ListServer.GetInstance(_clientService).UpdateStaticItemState(ipAddress, ServerState.Machine);
            }
            else if (currentState == ServerState.Machine)
            {
                await _clientService.GetAsyncApi($"{requestUrl}selectedServer");
                await ListServer.GetInstance(_clientService).UpdateStaticItemState(ipAddress, ServerState.Server);
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("==== ChangeState ex: " + Newtonsoft.Json.JsonConvert.SerializeObject(ex));
        }

        return RedirectToAction("Detail", new { ipAddress = ipAddress, subDirectory = "" });
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
