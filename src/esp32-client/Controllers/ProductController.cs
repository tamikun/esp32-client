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
        var productDetail = await _productService.Create(model);

        return RedirectToAction("Index");
    }

}
