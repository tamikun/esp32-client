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

    public async Task<IActionResult> Index()
    {
        var model = new MultipleUploadFileModel();
        model.ListSelectedDataFile = await _fileService.GetAllFiles(null);

        foreach (var item in ListServer.GetInstance(_clientService).GetItemList())
        {
            model.ListSelectedServer.AddRange(await _clientService.GetListSelectedServer($"http://{item.IpAddress}/"));
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(MultipleUploadFileModel? fileModel)
    {
        System.Console.WriteLine("==== fileModel: " + Newtonsoft.Json.JsonConvert.SerializeObject(fileModel));

        var selectedFile = fileModel.ListSelectedDataFile.Where(s => s.IsSelected).ToList();
        var selectedServer = fileModel.ListSelectedServer.Where(s => s.IsSelected).ToList();

        foreach (var file in selectedFile)
        {
            var fileName = file.FilePath.Split('/').LastOrDefault();
            byte[] fileBytes = { };

            if (System.IO.File.Exists(file.FilePath))
            {
                // Read file content as byte array
                fileBytes = System.IO.File.ReadAllBytes(file.FilePath);
            }

            foreach (var server in selectedServer)
            {
                var filePath = server.Folder + "/" + fileName;
                if (string.IsNullOrEmpty(server.Folder))
                {
                    filePath = fileName;
                }
                var result = await _clientService.PostAsyncFile(fileBytes, filePath, server.IpAddress);
                System.Console.WriteLine("==== resutl: " + Newtonsoft.Json.JsonConvert.SerializeObject(result));
            }
        }

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
