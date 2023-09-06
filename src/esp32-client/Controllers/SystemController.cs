using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using esp32_client.Models;
using esp32_client.Models.Singleton;
using esp32_client.Services;
using esp32_client.Builder;
using esp32_client.Domain;

namespace esp32_client.Controllers;
[Authentication]
public class SystemController : BaseController
{
    private readonly ISettingService _settingService;
    private readonly Settings _settings;

    public SystemController(LinqToDb linq2db, ISettingService settingService, Settings settings)
    {
        _linq2db = linq2db;
        _settingService = settingService;
        _settings = settings;
    }

    public async Task<IActionResult> Index()
    {
        await Task.CompletedTask;
        return View();
    }

    public async Task<IActionResult> Setting(int pageIndex = 0, int pageSize = 5, bool edit = false)
    {
        ViewBag.PageIndex = pageIndex;
        ViewBag.PageSize = pageSize;
        ViewBag.Edit = edit;
        await Task.CompletedTask;
        return View(new List<Setting>());
    }

    [HttpPost]
    public async Task<IActionResult> Setting(List<Setting> model, int pageIndex = 0, int pageSize = 5)
    {
        await _settingService.UpdateListSetting(model);
        ViewBag.PageIndex = pageIndex;
        ViewBag.PageSize = pageSize;
        ViewBag.Edit = false;
        return View(new List<Setting>());
    }

    public async Task<IActionResult> MachineFirmware()
    {
        // Display filename, datetime, size at File IO
        // Change firmware
        string fileName = null;
        double? fileSizeKB = null;
        DateTime? creationDateTime = null;

        if (System.IO.File.Exists(_settings.MachineFirmwareFilePath))
        {
            fileName = _settings.MachineFirmwareFilePath.Split('/').Where(s => !string.IsNullOrEmpty(s)).LastOrDefault();
            var fileInfo = new FileInfo(_settings.MachineFirmwareFilePath);
            long fileSizeBytes = fileInfo.Length;
            fileSizeKB = fileSizeBytes / 1024.0; // Convert bytes to kilobytes
            fileName = fileInfo.Name;

            creationDateTime = System.IO.File.GetCreationTime(_settings.MachineFirmwareFilePath).AddHours(7);
        }

        ViewBag.FileName = fileName;
        ViewBag.FileSizeKB = fileSizeKB;
        ViewBag.CreationDateTime = creationDateTime;
        ViewBag.FilePath = _settings.MachineFirmwareFilePath;

        await Task.CompletedTask;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> MachineFirmware(IFormFile file)
    {
        await Utils.Utils.WriteFile(file, _settings.MachineFirmwareFilePath);

        return RedirectToAction("MachineFirmware");
    }
    
    [HttpPost]
    public async Task<IActionResult> DeleteMachineFirmware()
    {
        await Utils.Utils.DeleteFile(_settings.MachineFirmwareFilePath);

        return RedirectToAction("MachineFirmware");
    }

}
