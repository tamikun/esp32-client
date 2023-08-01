using Microsoft.AspNetCore.Mvc;
using esp32_client.Services;
using esp32_client.Builder;

namespace esp32_client.Controllers;
[CustomAuthenticationFilter]
public class AssigningController : BaseController
{
    private readonly IFactoryService _departmentService;
    private readonly ILineService _lineService;

    public AssigningController(LinqToDb linq2db, IFactoryService departmentService, ILineService lineService)
    {
        _linq2db = linq2db;
        _departmentService = departmentService;
        _lineService = lineService;
    }


    public async Task<ActionResult> Index()
    {
        await Task.CompletedTask;
        return View();
    }

}
