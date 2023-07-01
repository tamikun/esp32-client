using System.Diagnostics;
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

    public async Task<IActionResult> Index(string? directory = null)
    {
        var listDataFile = await _fileService.GetAll(directory);

        var model = new MultipleUploadFileModel()
        {
            DataFileDirectory = directory
        };

        foreach (var item in listDataFile)
        {
            model.ListSelectedDataFile.Add(new SelectedDataFileModel
            {
                FilePath = item.FilePath,
                FileType = item.FileType,
                FileSize = item.FileSize,
                IsSelected = false,
            });
        }

        foreach (var item in ListServer.GetInstance(_clientService).GetItemList())
        {
            model.ListSelectedServer.Add(new SelectedServerModel
            {
                IpAddress = item.IpAddress,
                IsSelected = false,
            });
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(MultipleUploadFileModel? fileModel)
    {
        System.Console.WriteLine("Call MultipleUploadFile-Index-Post");

        System.Console.WriteLine("==== fileModel: " + JsonConvert.SerializeObject(fileModel));

        var listDataFile = await _fileService.GetAll(null);

        var model = new MultipleUploadFileModel();

        foreach (var item in listDataFile)
        {
            model.ListSelectedDataFile.Add(new SelectedDataFileModel
            {
                FilePath = item.FilePath,
                FileType = item.FileType,
                FileSize = item.FileSize,
                IsSelected = false,
            });
        }

        foreach (var item in ListServer.GetInstance(_clientService).GetItemList())
        {
            model.ListSelectedServer.Add(new SelectedServerModel
            {
                IpAddress = item.IpAddress,
                IsSelected = false,
            });
        }

        return View(model);
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
