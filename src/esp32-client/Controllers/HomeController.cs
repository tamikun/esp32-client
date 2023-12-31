﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using esp32_client.Models;
using esp32_client.Models.Singleton;
using esp32_client.Services;
using esp32_client.Builder;

namespace esp32_client.Controllers;

public class HomeController : BaseController
{
    private readonly ILogger<HomeController> _logger;
    private readonly Settings _settings;

    public HomeController(LinqToDb linq2db, ILogger<HomeController> logger, Settings settings)
    {
        _linq2db = linq2db;
        _logger = logger;
        _settings = settings;
    }
    
    [Authentication]
    public async Task<IActionResult> Index()
    {
        await Task.CompletedTask;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
