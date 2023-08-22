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

    public SystemController(LinqToDb linq2db, ISettingService settingService)
    {
        _linq2db = linq2db;
        _settingService = settingService;
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


}
