using Microsoft.AspNetCore.Mvc;
using esp32_client.Services;
using esp32_client.Builder;
using esp32_client.Models;
using Newtonsoft.Json;

namespace esp32_client.Controllers;
[Authentication]
public class SettingController : BaseController
{
    private readonly IFactoryService _factoryService;
    private readonly ILineService _lineService;
    private readonly IProductService _productService;
    private readonly IProcessService _processService;
    private readonly IStationService _stationService;
    private readonly IMachineService _machineService;

    public SettingController(LinqToDb linq2db, IFactoryService factoryService, ILineService lineService,
                            IProductService productService, IProcessService processService, IStationService stationService,
                            IMachineService machineService)
    {
        _linq2db = linq2db;
        _factoryService = factoryService;
        _lineService = lineService;
        _productService = productService;
        _processService = processService;
        _stationService = stationService;
        _machineService = machineService;
    }

    public async Task<IActionResult> Index()
    {
        await Task.CompletedTask;
        return View();
    }

    public async Task<ActionResult> Factory()
    {
        await Task.CompletedTask;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddFactory(int factoryNo, string factoryName)
    {
        return await HandleActionAsync(async () =>
        {
            var factory = await _factoryService.Create(factoryNo, factoryName);
        }, RedirectToAction("Factory"));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateFactory(int factoryId, string factoryName)
    {
        return await HandleActionAsync(async () =>
        {
            await _factoryService.UpdateName(factoryId, factoryName);
        }, RedirectToAction("Factory"));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteFactory(int factoryId)
    {
        return await HandleActionAsync(async () =>
        {
            await _factoryService.Delete(factoryId);
        }, RedirectToAction("Factory"));
    }

    public async Task<ActionResult> Machine(int factoryId = 0)
    {
        ViewBag.FactoryId = factoryId;
        await Task.CompletedTask;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddMachine(MachineCreateModel model)
    {
        return await HandleActionAsync(async () =>
        {
            var machine = await _machineService.Create(model);
        }, RedirectToAction("Machine", new { factoryId = model.FactoryId }));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateMachine(MachineUpdateModel model)
    {
        return await HandleActionAsync(async () =>
        {
            var machine = await _machineService.Update(model);
        }, RedirectToAction("Machine", new { factoryId = model.FactoryId }));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteMachine(int machineId, int factoryId)
    {
        return await HandleActionAsync(async () =>
        {
            await _machineService.DeleteById(machineId);
        }, RedirectToAction("Machine", new { factoryId = factoryId }));
    }

    [HttpPost]
    public async Task<IActionResult> ResetMachine(int machineId, int factoryId)
    {
        var listAlert = new List<AlertModel>();
        try
        {
            var machine = await _machineService.GetById(machineId) ?? new Domain.Machine();
            var result = await _machineService.ResetMachine(machine.IpAddress);

            if (result.Success)
            {
                listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = $"{result.ResponseBody}" });
            }
            else
            {
                listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"{result.ResponseBody}" });
            }
        }
        catch (Exception ex)
        {
            listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"{ex.Message}" });
        }

        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);

        return RedirectToAction("Machine", new { factoryId = factoryId });
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

    [HttpPost]
    public async Task<IActionResult> UpdateLine(LineUpdateModel model)
    {
        return await HandleActionAsync(async () =>
        {
            await _lineService.UpdateNameAndStationNo(model);
        }, RedirectToAction("Line", new { factoryId = model.FactoryId }));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteLine(int factoryId, int lineId)
    {
        return await HandleActionAsync(async () =>
        {
            await _lineService.Delete(lineId);
        }, RedirectToAction("Line", new { factoryId = factoryId }));
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
    public async Task<IActionResult> UpdateProduct(ProductUpdateModel model)
    {
        return await HandleActionAsync(async () =>
        {
            await _productService.UpdateNameAndProcess(model);
        }, RedirectToAction("Product", new { factoryId = model.FactoryId }));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteProduct(int factoryId, int productId)
    {
        return await HandleActionAsync(async () =>
        {
            await _productService.Delete(productId);
        }, RedirectToAction("Product", new { factoryId = factoryId }));
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
