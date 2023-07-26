using System.Diagnostics;
using System.Text;
using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
using esp32_client.Services;
using HtmlAgilityPack;
using LinqToDB;
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
    private readonly LinqToDb _connection;
    private readonly IUserAccountService _userAccountService;
    private readonly IMachineService _machineService;

    public TestApiController(ILogger<HomeController> logger, IClientService clientService, IFileService fileService, Settings settings, LinqToDb connection, IUserAccountService userAccountService, IMachineService machineService)
    {
        _logger = logger;
        _clientService = clientService;
        _fileService = fileService;
        _settings = settings;
        _connection = connection;
        _userAccountService = userAccountService;
        _machineService = machineService;
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
    public async Task<IActionResult> GetSetting()
    {
        await Task.CompletedTask;
        return Ok(_settings);
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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkInsert([FromBody] List<UserAccount> data)
    {
        await _connection.BulkInsert(data);
        return Ok();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUserAccount([FromBody] UserAccountCreateModel model)
    {
        return Ok(await _userAccountService.Create(model));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> IsValidUser(string loginName, string password)
    {
        return Ok(await _userAccountService.IsValidUser(loginName, password));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckUserRight(string loginName, string controllerName, string actionName)
    {
        return Ok(await _userAccountService.CheckUserRight(loginName, controllerName, actionName));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMachine(MachineUpdateModel model)
    {
        return Ok(await _machineService.Update(model));
    }

}
