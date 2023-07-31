using Microsoft.AspNetCore.Mvc;
using esp32_client.Services;
using esp32_client.Models;
using Newtonsoft.Json;
using AutoMapper;
using esp32_client.Builder;

namespace esp32_client.Controllers;

[CustomAuthenticationFilter]
public class MachineController : BaseController
{
    private readonly IMachineService _machineService;
    private readonly IMapper _mapper;

    public MachineController(LinqToDb linq2db, IMachineService machineService, IMapper mapper)
    {
        _linq2db = linq2db;
        _machineService = machineService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        await Task.CompletedTask;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Add(MachineCreateModel model)
    {
        return await HandleActionAsync(async () =>
        {
            var machineDetail = await _machineService.Create(model);
        }, RedirectToAction("Index"));
    }

    public async Task<IActionResult> Delete(int id)
    {
        return await HandleActionAsync(async () =>
        {
            await _machineService.Delete(id);
        }, RedirectToAction("Index"));
    }

    public async Task<IActionResult> Update(int id)
    {
        var machine = await _machineService.GetById(id);
        var productUpdateModel = _mapper.Map<MachineUpdateModel>(machine);
        return View(productUpdateModel);
    }

    [HttpPost]
    public async Task<IActionResult> Update(MachineUpdateModel model)
    {
        return await HandleActionAsync(async () =>
        {
            var machine = await _machineService.Update(model);
        }, RedirectToAction("Index"));
    }
}
