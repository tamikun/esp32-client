using Microsoft.AspNetCore.Mvc;
using esp32_client.Services;

namespace esp32_client.Controllers;
[CustomAuthenticationFilter]
public class DepartmentController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDepartmentService _departmentService;

    public DepartmentController(IHttpContextAccessor httpContextAccessor, IDepartmentService departmentService)
    {
        _httpContextAccessor = httpContextAccessor;
        _departmentService = departmentService;
    }


    public async Task<ActionResult> Index()
    {
        var department = await _departmentService.GetById(1);
        return View(department);
    }

}
