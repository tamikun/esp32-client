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
    private readonly IProductService _productService;

    public ProductController(IHttpContextAccessor httpContextAccessor, IProductService productService)
    {
        _httpContextAccessor = httpContextAccessor;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        await Task.CompletedTask;
        var model = new ProductCreateModel();
        model.ListProcessPattern.Add(new ProcessPattern { PatternNumber = "", ProcessName = "0" });
        return View(model);
    }

    public async Task<IActionResult> Detail(string productName)
    {
        var productDetail = await _productService.GetProductDetail(productName);

        return View(productDetail);
    }

    [HttpPost]
    public async Task<IActionResult> Add(ProductCreateModel model)
    {
        var listAlert = new List<AlertModel>();
        try
        {
            var productDetail = await _productService.Create(model);
            listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = $"Add product successfully" });
        }
        catch (Exception ex)
        {
            listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"{ex.Message}" });
        }
        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);

        return RedirectToAction("Index");
    }

}
