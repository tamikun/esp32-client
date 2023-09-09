using System.Text.Json.Serialization;
using esp32_client.Builder;
using esp32_client.Models.Singleton;
using esp32_client.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace esp32_client.Controllers;

[ApiController]
// [Route("api/[controller]/[action]")]
public class OpenApiController : ControllerBase
{
    private readonly IDataReportService _dataReportService;
    private readonly IMachineService _machineService;
    private readonly LinqToDb _linq2db;
    private readonly Settings _settings;

    public OpenApiController(IDataReportService dataReportService, Settings settings, IMachineService machineService, LinqToDb linq2db)
    {
        _dataReportService = dataReportService;
        _settings = settings;
        _machineService = machineService;
        _linq2db = linq2db;
    }

    [HttpPost]
    [Route("data_prod")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SaveproductData([FromBody] SaveProductDataModel model)
    {
        await _dataReportService.Create(model.IPAddress, model.ProductNumber);
        return Ok(model);
    }

    [HttpGet]
    [Route("operate_app.bin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFirmwareMachine()
    {
        if (System.IO.File.Exists(_settings.MachineFirmwareFilePath))
        {
            var fileBytes = await System.IO.File.ReadAllBytesAsync(_settings.MachineFirmwareFilePath);
            return File(fileBytes, contentType: "application/octet-stream", fileDownloadName: "operate_app.bin");
        }
        await Task.CompletedTask;
        return NotFound();
    }

    [HttpPost]
    [Route("update_status")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusModel model)
    {
        System.Console.WriteLine($"==== model.UpdateFW: {model.UpdateFW}");

        if (model.UpdateFW.StartsWith("Finish!") && model.UpdateFW.EndsWith("successfully."))
        {
            var newMachine = await _machineService.GetByIpAddress(_settings.DefaultNewMachineIp);
            newMachine.UpdateFirmwareSucess = true;
            await _linq2db.Update(newMachine);
        }
        return Ok();
    }

    [HttpGet]
    [Route("api/OpenApi/GetProductNumber/{id}")]
    [Authentication]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProductNumber(int id)
    {
        var result = await _dataReportService.GetLastDataByStationId(id);

        var response = new
        {
            result = result.ProductNumber
        };

        return Ok(response);
    }

    public class SaveProductDataModel
    {
        [JsonProperty("IPAddress")]
        [JsonPropertyName("IPAddress")]
        public string IPAddress { get; set; }
        [JsonProperty("ProductNumber")]
        [JsonPropertyName("ProductNumber")]
        public int ProductNumber { get; set; }
    }

    public class UpdateStatusModel
    {
        [JsonProperty("UpdateFW")]
        [JsonPropertyName("UpdateFW")]
        public string UpdateFW { get; set; }
    }
}
