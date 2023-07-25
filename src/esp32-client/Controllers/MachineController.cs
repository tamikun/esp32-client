using Microsoft.AspNetCore.Mvc;
using esp32_client.Services;

namespace esp32_client.Controllers;

[CustomAuthenticationFilter]
public class MachineController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IProductService _productService;

    public MachineController(IHttpContextAccessor httpContextAccessor, IProductService productService)
    {
        _httpContextAccessor = httpContextAccessor;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        await Task.CompletedTask;
        return View();
    }

}
