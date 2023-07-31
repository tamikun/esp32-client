﻿using Microsoft.AspNetCore.Mvc;
using esp32_client.Services;
using esp32_client.Builder;

namespace esp32_client.Controllers;
[CustomAuthenticationFilter]
public class SettingController : BaseController
{
    private readonly IDepartmentService _departmentService;
    private readonly ILineService _lineService;

    public SettingController(LinqToDb linq2db, IDepartmentService departmentService, ILineService lineService)
    {
        _linq2db = linq2db;
        _departmentService = departmentService;
        _lineService = lineService;
    }


    public async Task<ActionResult> Product()
    {
        await Task.CompletedTask;
        return View();
    }

}
