using Microsoft.AspNetCore.Mvc;
using esp32_client.Models;
using esp32_client.Services;
using Newtonsoft.Json;

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
        return View();
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

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var listAlert = new List<AlertModel>();
        try
        {
            await _productService.Delete(id);
            listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = $"Delete product successfully" });
        }
        catch (Exception ex)
        {
            listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"{ex.Message}" });
        }
        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);

        return RedirectToAction("Index");
    }

}
