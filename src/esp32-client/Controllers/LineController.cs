using Microsoft.AspNetCore.Mvc;
using esp32_client.Services;

namespace esp32_client.Controllers;
[CustomAuthenticationFilter]
public class LineController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILineService _lineService;

    public LineController(IHttpContextAccessor httpContextAccessor, ILineService lineService)
    {
        _httpContextAccessor = httpContextAccessor;
        _lineService = lineService;
    }


    public ActionResult Index()
    {
        return View();
    }

}
