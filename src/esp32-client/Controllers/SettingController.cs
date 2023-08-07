﻿using Microsoft.AspNetCore.Mvc;
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
    private readonly IStationService _stationService;

    public SettingController(LinqToDb linq2db, IFactoryService departmentService, ILineService lineService, IProductService productService, IProcessService processService, IStationService stationService)
    {
        _linq2db = linq2db;
        _departmentService = departmentService;
        _lineService = lineService;
        _productService = productService;
        _processService = processService;
        _stationService = stationService;
    }


    public async Task<ActionResult> Line(int factoryId = 0)
    {
        ViewBag.FactoryId = factoryId;
        await Task.CompletedTask;
        return View();
    }

    public async Task<ActionResult> LineDetail(int factoryId = 0, int lineId = 0, bool edit = false)
    {
        ViewBag.FactoryId = factoryId;
        ViewBag.LineId = lineId;
        ViewBag.Edit = edit;
        await Task.CompletedTask;
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> UpdateStation(ListStationUpdateModel model)
    {
        return await HandleActionAsync(async () =>
        {
            foreach (var item in model.ListStationUpdate)
            {
                await _stationService.UpdateStationName(item);
            }
        }, RedirectToAction("LineDetail", new { factoryId = model.FactoryId, lineId = model.LineId }));
    }


    [HttpPost]
    public async Task<IActionResult> AddLine(LineCreateModel model)
    {
        return await HandleActionAsync(async () =>
        {
            var productDetail = await _lineService.Create(model);
        }, RedirectToAction("Line", new { factoryId = model.FactoryId }));
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

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DownloadFile(string path)
    {
        if (System.IO.File.Exists(path))
        {
            var fileName = path.Split('/').Where(s => !string.IsNullOrEmpty(s)).LastOrDefault();
            var fileBytes = await System.IO.File.ReadAllBytesAsync(path);

            string contentType = Utils.Utils.GetContentType(fileName?.Split('.')?.LastOrDefault()?.ToLower());

            return File(fileBytes, contentType, fileDownloadName: fileName);
        }
        return Ok();
    }

}
