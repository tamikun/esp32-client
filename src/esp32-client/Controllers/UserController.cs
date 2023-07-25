using Microsoft.AspNetCore.Mvc;
using esp32_client.Models;
using esp32_client.Services;
using Newtonsoft.Json;

namespace esp32_client.Controllers;

public class UserController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserAccountService _userAccountService;

    public UserController(IHttpContextAccessor httpContextAccessor, IUserAccountService userAccountService)
    {
        _httpContextAccessor = httpContextAccessor;
        _userAccountService = userAccountService;
    }


    public ActionResult Login()
    {
        return View();
    }


    [CustomAuthenticationFilter]
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Login(string loginName, string password)
    {
        // Perform custom authentication logic here
        if (await _userAccountService.IsValidUser(loginName, password))
        {
            // Authentication successful
            // Store the username in session
            _httpContextAccessor?.HttpContext?.Session.SetString("LoginName", loginName);

            // Store the role in session
            var user = await _userAccountService.GetByLoginName(loginName);
            if (user is not null)
                _httpContextAccessor?.HttpContext?.Session.SetString("RoleId", "1");

            return RedirectToAction("Index", "Home");
        }

        // Authentication failed
        ModelState.AddModelError("", "Invalid username or password");
        return View();
    }

    [HttpPost]
    [CustomAuthenticationFilter]
    public async Task<ActionResult> Create(UserAccountCreateModel model)
    {
        int roleId = Int32.Parse(_httpContextAccessor?.HttpContext?.Session.GetString("RoleId") ?? "0");
        var listAlert = new List<AlertModel>();
        if (roleId == 1)
        {
            try
            {
                await _userAccountService.Create(model);
                listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = $"Create account successfully" });
            }
            catch (Exception ex)
            {
                listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"{ex.Message}" });
            }
        }
        else
        {
            listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"User has no right to create new account" });
        }

        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);

        return View();
    }

    [CustomAuthenticationFilter]
    public ActionResult Logout()
    {
        // Clear the session or cookie used for authentication
        _httpContextAccessor?.HttpContext?.Session.Clear();

        return RedirectToAction("Login");
    }

}
