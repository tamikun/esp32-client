using Microsoft.AspNetCore.Mvc;
using esp32_client.Services;
using esp32_client.Builder;
using esp32_client.Models;

namespace esp32_client.Controllers;
[CustomAuthenticationFilter]
public class SettingController : BaseController
{
    private readonly IFactoryService _departmentService;
    private readonly ILineService _lineService;
    private readonly IProductService _productService;

    public SettingController(LinqToDb linq2db, IFactoryService departmentService, ILineService lineService, IProductService productService)
    {
        _linq2db = linq2db;
        _departmentService = departmentService;
        _lineService = lineService;
        _productService = productService;
    }


    public async Task<ActionResult> Product(int factoryId = 0)
    {
        ViewBag.FactoryId = factoryId;
        await Task.CompletedTask;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct(ProductCreateModel model)
    {
        return await HandleActionAsync(async () =>
        {
            var productDetail = await _productService.Create(model);
        }, RedirectToAction("Product", new { factoryId = model.FactoryId }));
    }

}
