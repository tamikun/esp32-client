using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using esp32_client.Models;
using esp32_client.Services;
using Newtonsoft.Json;
using esp32_client.Builder;

namespace esp32_client.Controllers;

[CustomAuthenticationFilter]
public class ProductController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPatternService _patternService;

    public ProductController(IHttpContextAccessor httpContextAccessor, IPatternService patternService)
    {
        _httpContextAccessor = httpContextAccessor;
        _patternService = patternService;
    }

    public async Task<IActionResult> Index()
    {
        await Task.CompletedTask;
        return View();
    }

}
