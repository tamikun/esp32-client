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
    private readonly Settings _settings;
    private readonly LinqToDb _connection;
    private readonly IUserAccountService _userAccountService;
    private readonly IMachineService _machineService;

    public TestApiController(ILogger<HomeController> logger, Settings settings, LinqToDb connection, IUserAccountService userAccountService, IMachineService machineService)
    {
        _logger = logger;
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

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSetting()
    {
        await Task.CompletedTask;
        return Ok(_settings);
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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> FormatString()
    {
        await Task.CompletedTask;
        return Ok(string.Format(_settings.UploadFileFormat, "192.168.1.1", "VDATA.VDT"));
    }

}
