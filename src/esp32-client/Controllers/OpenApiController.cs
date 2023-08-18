using esp32_client.Services;
using Microsoft.AspNetCore.Mvc;

namespace esp32_client.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class OpenApiController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDataReportService _dataReportService;

    public OpenApiController(IHttpContextAccessor httpContextAccessor, IDataReportService dataReportService)
    {
        _httpContextAccessor = httpContextAccessor;
        _dataReportService = dataReportService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SaveproductData()
    {
        var ip = _httpContextAccessor?.HttpContext?.Connection.RemoteIpAddress;
        System.Console.WriteLine("==== ip: " + ip?.ToString());

        return Ok(await _dataReportService.RandomCreate());
    }
}
