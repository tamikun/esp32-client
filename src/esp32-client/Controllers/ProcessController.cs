using Microsoft.AspNetCore.Mvc;
using esp32_client.Services;

namespace esp32_client.Controllers;
[CustomAuthenticationFilter]
public class ProcessController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IProcessService _processService;

    public ProcessController(IHttpContextAccessor httpContextAccessor, IProcessService processService)
    {
        _httpContextAccessor = httpContextAccessor;
        _processService = processService;
    }


    public ActionResult Index()
    {
 
        return View();
    }

}
