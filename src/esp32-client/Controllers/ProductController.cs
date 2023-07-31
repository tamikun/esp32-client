using Microsoft.AspNetCore.Mvc;
using esp32_client.Models;
using esp32_client.Services;
using Newtonsoft.Json;
using AutoMapper;
using esp32_client.Builder;

namespace esp32_client.Controllers;

[CustomAuthenticationFilter]
public class ProductController : BaseController
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public ProductController(LinqToDb linq2db, IProductService productService, IMapper mapper)
    {
        _linq2db = linq2db;
        _productService = productService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        await Task.CompletedTask;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Add(ProductCreateModel model)
    {
        return await HandleActionAsync(async () =>
        {
            var productDetail = await _productService.Create(model);
        }, RedirectToAction("Index"));
    }

    public async Task<IActionResult> Delete(int id)
    {
        return await HandleActionAsync(async () =>
        {
            await _productService.Delete(id);
        }, RedirectToAction("Index"));
    }
}
