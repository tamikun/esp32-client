using Microsoft.AspNetCore.Mvc;
using esp32_client.Services;
using esp32_client.Models;
using Newtonsoft.Json;

namespace esp32_client.Controllers;
[CustomAuthenticationFilter]
public class ProcessController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IProcessService _processService;
    private readonly IProductService _productService;

    public ProcessController(IHttpContextAccessor httpContextAccessor, IProcessService processService, IProductService productService)
    {
        _httpContextAccessor = httpContextAccessor;
        _processService = processService;
        _productService = productService;
    }

    public async Task<IActionResult> Update(int id)
    {
        var product = await _productService.GetById(id);
        var model = new ProcessAddRequestModel();
        model.ProductId = id;
        model.ProductName = product?.ProductName;
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Update(ProcessAddRequestModel model)
    {
        System.Console.WriteLine("==== Update: " + Newtonsoft.Json.JsonConvert.SerializeObject(model));
        var listAlert = new List<AlertModel>();
        try
        {
            await _processService.Update(model);
            listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = $"Update product successfully" });
        }
        catch (Exception ex)
        {
            listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"{ex.Message}" });
        }
        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);

        return RedirectToAction("Index", "Product");
    }

}
