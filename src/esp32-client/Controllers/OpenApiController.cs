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
        // Access the HttpContext to get the client's IP address
        var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;

        if (ipAddress != null)
        {
            // IPv4 is stored as an instance of IPAddress, so convert it to a string
            if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                System.Console.WriteLine($"ipAddress {ipAddress.ToString()}");
            }
        }


        return Ok(await _dataReportService.RandomCreate());
    }
}
