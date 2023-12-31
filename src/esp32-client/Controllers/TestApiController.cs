﻿using esp32_client.Services;
using Microsoft.AspNetCore.Mvc;
using esp32_client.Models.Singleton;
using esp32_client.Builder;
using Microsoft.EntityFrameworkCore;
using esp32_client.Domain;

namespace esp32_client.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TestApiController : ControllerBase
{
    private readonly Settings _settings;
    private readonly Context _context;
    private readonly LinqToDb _linq2db;

    public TestApiController(Settings settings, Context context, LinqToDb linq2db)
    {
        _settings = settings;
        _context = context;
        _linq2db = linq2db;
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
    public async Task<IActionResult> FormatString()
    {
        await Task.CompletedTask;
        return Ok(string.Format(_settings.UploadFileFormat, "192.168.1.1", "VDATA.VDT"));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Invoke()
    {
        var scheduledTaskService = EngineContext.Resolve<IScheduleTaskService>();

        var rs = typeof(IScheduleTaskService)?.GetMethod(nameof(IScheduleTaskService.SaveProductData))
                                            ?.Invoke(scheduledTaskService, new object[] { });
        await Task.CompletedTask;
        return Ok();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetClientIp()
    {
        var dict = new Dictionary<string, string>();

        dict.Add("RemoteIpAddress", HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "");
        await Task.CompletedTask;

        return Ok(dict);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTokenHeader()
    {
        var auth = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        string token = null;
        if (!string.IsNullOrEmpty(auth) && auth.StartsWith("Bearer "))
        {
            token = auth.Substring("Bearer ".Length);
        }

        await Task.CompletedTask;

        return Ok(token);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EF()
    {
        var factories = await _context.Entity<Factory>().ToListAsync();

        return Ok(factories);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TruncateSessionTable()
    {
        await _linq2db.Truncate<UserSession>();
        return Ok();
    }
}
