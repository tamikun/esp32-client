using System.Text.Json.Serialization;
using esp32_client.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace esp32_client.Controllers;

[ApiController]
// [Route("api/[controller]/[action]")]
public class OpenApiController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDataReportService _dataReportService;

    public OpenApiController(IHttpContextAccessor httpContextAccessor, IDataReportService dataReportService)
    {
        _httpContextAccessor = httpContextAccessor;
        _dataReportService = dataReportService;
    }

    public class SaveProductDataModel
    {
#nullable disable
        [JsonProperty("IPAddress")]
        [JsonPropertyName("IPAddress")]
        public string IPAddress { get; set; }
        [JsonProperty("ProductNumber")]
        [JsonPropertyName("ProductNumber")]
        public int ProductNumber { get; set; }
    }

    [HttpPost]
    [Route("data_prod")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SaveproductData([FromBody] SaveProductDataModel model)
    {
        System.Console.WriteLine("==== model: " + Newtonsoft.Json.JsonConvert.SerializeObject(model));
        await _dataReportService.Create(model.IPAddress, model.ProductNumber);
        return Ok(model);
    }

    [HttpGet]
    //[CustomAuthenticationFilter]
    [Route("api/OpenApi/GetProductNumber/{id}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProductNumber(int id)
    {
        // var result = await _dataReportService.GetLastDataByStationId(id);

        // Get value by cache instead

        var response = new
        {
            result = 0
        };
        await Task.CompletedTask;
        return Ok(response);
    }
}
