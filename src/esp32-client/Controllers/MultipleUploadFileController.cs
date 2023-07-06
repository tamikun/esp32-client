﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using esp32_client.Models;
using esp32_client.Services;
using Newtonsoft.Json;

namespace esp32_client.Controllers;

public class MultipleUploadFileController : Controller
{
    private readonly ILogger<MultipleUploadFileController> _logger;
    private readonly IClientService _clientService;
    private readonly IFileService _fileService;

    public MultipleUploadFileController(ILogger<MultipleUploadFileController> logger, IFileService fileService, IClientService clientService)
    {
        _logger = logger;
        _fileService = fileService;
        _clientService = clientService;
    }

    public async Task<IActionResult> Index()
    {
        var model = new MultipleUploadFileModel();
        await Task.CompletedTask;
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(MultipleUploadFileModel? fileModel)
    {
        System.Console.WriteLine("==== fileModel: " + Newtonsoft.Json.JsonConvert.SerializeObject(fileModel));

        var selectedFile = fileModel.ListSelectedDataFile.Where(s => s.IsSelected);
        var selectedServer = fileModel.ListSelectedServer.Where(s => s.IsSelected);

        var listAlert = new List<AlertModel>();

        foreach (var file in selectedFile)
        {
            var fileName = file.FilePath.Split('/').LastOrDefault();
            byte[] fileBytes = { };

            if (System.IO.File.Exists(file.FilePath))
            {
                // Read file content as byte array
                fileBytes = System.IO.File.ReadAllBytes(file.FilePath);
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var tasks = selectedServer.Select(async server =>
             {
                 var filePath = server.Folder + "/" + fileName;
                 if (string.IsNullOrEmpty(server.Folder))
                 {
                     filePath = fileName;
                 }

                 string message = $"Upload file {file.FilePath} to {server.IpAddress}{filePath}: ";

                 if (fileModel.ReplaceIfExist)
                 {
                     try
                     {
                        await _clientService.DeleteFile(server.IpAddress, "VDATA", fileName);
                     }
                     catch
                     {
                         //Do nothing
                     }
                 }

                 try
                 {

                     var result = await _clientService.PostAsyncFile(fileBytes, filePath, server.IpAddress);

                     if (!result.IsSuccessStatusCode)
                     {
                         listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = message + result.StatusCode.ToString() });
                     }
                     else
                     {
                         listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = message + "Success" });
                     }
                 }
                 catch
                 {
                     listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = message + "Connection time out" });
                 }

             });

            await Task.WhenAll(tasks);
            sw.Stop();
            System.Console.WriteLine("==== Multi ElapsedMilliseconds: " + Newtonsoft.Json.JsonConvert.SerializeObject(sw.ElapsedMilliseconds));
        }

        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);

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
