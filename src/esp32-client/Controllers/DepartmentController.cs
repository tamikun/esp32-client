using Microsoft.AspNetCore.Mvc;
using esp32_client.Services;

namespace esp32_client.Controllers;
[CustomAuthenticationFilter]
public class DepartmentController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDepartmentService _departmentService;
    private readonly ILineService _lineService;

    public DepartmentController(IHttpContextAccessor httpContextAccessor, IDepartmentService departmentService, ILineService lineService)
    {
        _httpContextAccessor = httpContextAccessor;
        _departmentService = departmentService;
        _lineService = lineService;
    }


    public async Task<ActionResult> Index()
    {
        var model = await _lineService.GetProcessAndMachineOfLines(1);
        return View(model);
    }

}
