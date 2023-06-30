using esp32_client.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace esp32_client.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class OpenApiController : ControllerBase
{
    private readonly ILogger<HomeController> _logger;
    private readonly IClientService _clientService;

    public OpenApiController(ILogger<HomeController> logger, IClientService clientService)
    {
        _logger = logger;
        _clientService = clientService;

    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ScanningIP(string subnet = "192.168.101.", int port = 80)
    {
        var result = _clientService.GetAvailableIpAddress();
        await Task.CompletedTask;
        var rs = Newtonsoft.Json.JsonConvert.SerializeObject(result);
        return Ok(rs);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAsyncApi(string apiUrl = "https://jsonplaceholder.typicode.com/todos")
    {
        var result = await _clientService.GetAsyncApi(apiUrl);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsyncFile()
    {
        System.Console.WriteLine("==== here PostAsyncFile");
        var newFile = Request.Form.Files.FirstOrDefault();
        var filePath = Request.Form["filePath"].FirstOrDefault();
        var ipAddress = Request.Form["ipAddress"].FirstOrDefault();

        var result = await _clientService.PostAsyncFile(newFile, filePath, ipAddress);

        return Ok(result);
    }

}
