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

        // foreach (var item in ListServer.GetInstance(_clientService).GetDynamicList())
        var staticList = await ListServer.GetInstance(_clientService).GetStaticList();

        // Stopwatch sw = new Stopwatch();
        // sw.Start();
        // foreach (var item in staticList)
        // {
        //     var getListSelected = await _clientService.GetListSelectedServer($"http://{item.IpAddress}/");
        //     model.ListSelectedServer.AddRange(getListSelected);
        // }
        // sw.Stop();
        // System.Console.WriteLine("==== Test sw: " + Newtonsoft.Json.JsonConvert.SerializeObject(sw.ElapsedMilliseconds)); //2500

        // Stopwatch sw = new Stopwatch();
        // sw.Start();
        var tasks = staticList.Select(async item =>
        {
            var serverUrl = $"http://{item.IpAddress}/";
            var getListSelectedTask = _clientService.GetListSelectedServer(serverUrl);

            var getListSelected = await getListSelectedTask;
            model.ListSelectedServer.AddRange(getListSelected);

        });

        await Task.WhenAll(tasks);
        // sw.Stop();
        // System.Console.WriteLine("==== Test sw: " + Newtonsoft.Json.JsonConvert.SerializeObject(sw.ElapsedMilliseconds));  //1000

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(MultipleUploadFileModel? fileModel)
    {
        System.Console.WriteLine("==== fileModel: " + Newtonsoft.Json.JsonConvert.SerializeObject(fileModel));

        var selectedFile = fileModel.ListSelectedDataFile.Where(s => s.IsSelected).ToList();
        var selectedServer = fileModel.ListSelectedServer.Where(s => s.IsSelected).ToList();

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

            foreach (var server in selectedServer)
            {
                var filePath = server.Folder + "/" + fileName;
                if (string.IsNullOrEmpty(server.Folder))
                {
                    filePath = fileName;
                }

                string message = $"Upload file {file.FilePath} to {server.IpAddress}{filePath}: ";

                Stopwatch sw = new Stopwatch();
                sw.Start();
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
                sw.Stop();
                System.Console.WriteLine("==== Multi ElapsedMilliseconds: " + Newtonsoft.Json.JsonConvert.SerializeObject(sw.ElapsedMilliseconds));

            }
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
