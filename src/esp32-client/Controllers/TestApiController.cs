using System.Diagnostics;
using System.Text;
using esp32_client.Models;
using esp32_client.Services;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace esp32_client.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TestApiController : ControllerBase
{
    private readonly ILogger<HomeController> _logger;
    private readonly IClientService _clientService;
    private readonly IFileService _fileService;
    private readonly Settings _settings;

    public TestApiController(ILogger<HomeController> logger, IClientService clientService, IFileService fileService, Settings settings)
    {
        _logger = logger;
        _clientService = clientService;
        _fileService = fileService;
        _settings = settings;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsyncFile()
    {
        var newFile = Request.Form.Files.FirstOrDefault();
        var filePath = Request.Form["filePath"].FirstOrDefault();
        var postUrl = Request.Form["postUrl"].FirstOrDefault();

        var rs = new
        {
            FileName = newFile?.FileName,
            FileSize = newFile?.Length,
            FileType = newFile?.ContentType,
            FilePath = filePath,
            PostUrl = postUrl
        };
        await Task.CompletedTask;
        return Ok(rs);
    }

    public class PostJsonModel
    {
        public string? Key1 { get; set; }
        public string? Key2 { get; set; }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostJson(string? requestBody, string apiUrl = "https://api.example.com/api/endpoint")
    {
        var rs = await _clientService.PostAsyncApi(requestBody, apiUrl);
        return Ok(rs);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get(int id)
    {
        await Task.CompletedTask;
        return Ok(id * 10);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetListEspFile(string apiUrl)
    {
        var result = await _clientService.GetListEspFile(apiUrl);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TestGetDataFromHtmlString(string apiUrl, string node = "//h3")
    {
        var pageData = await _clientService.GetAsyncApi(apiUrl, false);

        string html = pageData;

        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);
        HtmlNodeCollection selectedNodes = htmlDoc.DocumentNode.SelectNodes(node);
        if (selectedNodes != null && selectedNodes.Count > 0)
        {
            foreach (var select in selectedNodes)
            {
                System.Console.WriteLine("==== InnerHtml: " + Newtonsoft.Json.JsonConvert.SerializeObject(select.InnerHtml));
                System.Console.WriteLine("==== InnerText: " + Newtonsoft.Json.JsonConvert.SerializeObject(select.InnerText));
            }
        }
        return Ok(selectedNodes?.Count);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDictionaryFile(string? directoryPath = null)
    {
        var rs = await _fileService.GetDictionaryFile(directoryPath);

        return Ok(rs);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDictionaryFileWithNode(string ipAddress)
    {
        var rs = await _clientService.GetDictionaryFileWithNode(ipAddress);

        return Ok(rs);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFileContent(IFormFile file)
    {
        byte[] fileBytes;
        string fileContent;

        using (var ms = new MemoryStream())
        {
            file.CopyTo(ms);
            fileBytes = ms.ToArray();
        }

        fileContent = Encoding.UTF8.GetString(fileBytes, 0, fileBytes.Length);
        await Task.CompletedTask;
        return Ok(fileContent);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetServerName(string url)
    {
        return Ok(await _clientService.GetServerName(url));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSetting()
    {
        await Task.CompletedTask;
        return Ok(_settings);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TestTasks(int number)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        var listTask = new List<Task>();
        for (int i = 0; i < number; i++)
        {
            listTask.Add(DelayTask(i));
        }
        await Task.WhenAll(listTask);
        sw.Stop();
        return Ok(sw.ElapsedMilliseconds);
    }

    private async Task DelayTask(int i)
    {
        await Task.Delay(5000);
        System.Console.WriteLine($"Task {i} is completed");
    }

}
