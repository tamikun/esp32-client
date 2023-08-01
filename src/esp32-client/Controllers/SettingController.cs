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
    private readonly IProcessService _processService;

    public SettingController(LinqToDb linq2db, IFactoryService departmentService, ILineService lineService, IProductService productService, IProcessService processService)
    {
        _linq2db = linq2db;
        _departmentService = departmentService;
        _lineService = lineService;
        _productService = productService;
        _processService = processService;
    }


    public async Task<ActionResult> Product(int factoryId = 0)
    {
        ViewBag.FactoryId = factoryId;
        await Task.CompletedTask;
        return View();
    }
    public async Task<ActionResult> ProductDetail(int factoryId = 0, int productId = 0, bool edit = false)
    {
        ViewBag.FactoryId = factoryId;
        ViewBag.ProductId = productId;
        ViewBag.Edit = edit;
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
    
    [HttpPost]
    public async Task<IActionResult> UpdateProcess(ListProcessUpdateModel model)
    {
        return await HandleActionAsync(async () =>
        {
            foreach (var item in model.ListProcessUpdate)
            {
                await _processService.UpdateProcessNamePatternNoById(item);
            }
        }, RedirectToAction("ProductDetail", new { factoryId = model.FactoryId, productId = model.ProductId }));
    }

}
