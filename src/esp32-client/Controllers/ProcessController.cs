using Microsoft.AspNetCore.Mvc;
using esp32_client.Services;
using esp32_client.Models;
using Newtonsoft.Json;
using esp32_client.Builder;

namespace esp32_client.Controllers;
[CustomAuthenticationFilter]
public class ProcessController : BaseController
{
    private readonly IProcessService _processService;
    private readonly IProductService _productService;

    public ProcessController(LinqToDb linq2db, IProcessService processService, IProductService productService)
    {
        _linq2db = linq2db;
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
        return await HandleActionAsync(async () =>
        {
            await _processService.Update(model);
        }, RedirectToAction("Index", "Product"));
    }

}
