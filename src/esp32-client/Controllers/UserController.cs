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

    [CustomAuthenticationFilter]
    public ActionResult Index()
    {
        return View();
    }

    public ActionResult Login()
    {
        return View();
    }



    [HttpPost]
    public async Task<ActionResult> Login(string loginName, string password)
    {
        var userAccount = await _userAccountService.GetByLoginName(loginName);
        if (userAccount is not null)
        {
            // Perform custom authentication logic here
            if (await _userAccountService.IsValidUser(userAccount, password))
            {
                // Authentication successful
                // Store the username in session
                _httpContextAccessor?.HttpContext?.Session.SetString("LoginName", loginName);
                _httpContextAccessor?.HttpContext?.Session.SetString("UserName", userAccount.UserName);

                // Store the role in session
                var userRight = await _userAccountService.GetUserRight(loginName);
                if (userRight is not null)
                    _httpContextAccessor?.HttpContext?.Session.SetString("UserRight", JsonConvert.SerializeObject(userRight));

                return RedirectToAction("Index", "Home");
            }
        }


        // Authentication failed
        ModelState.AddModelError("", "Invalid username or password");
        return View();
    }

    [CustomAuthenticationFilter]
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [CustomAuthenticationFilter]
    public async Task<ActionResult> Create(UserAccountCreateModel model)
    {
        var listAlert = new List<AlertModel>();

        try
        {
            await _userAccountService.Create(model);
            listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = $"Create account" });
        }
        catch (Exception ex)
        {
            listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"{ex.Message}" });
        }

        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);

        return RedirectToAction("Index");
    }

    [CustomAuthenticationFilter]
    public async Task<ActionResult> Update()
    {
        string loginName = _httpContextAccessor?.HttpContext?.Session.GetString("LoginName") ?? "";
        var userAccount = await _userAccountService.GetByLoginName(loginName) ?? new Domain.UserAccount();
        var model = new UserAccountUpdateModel();
        model.Id = userAccount.Id;
        model.UserName = userAccount.UserName;
        model.LoginName = userAccount.LoginName;
        return View(model);
    }

    [HttpPost]
    [CustomAuthenticationFilter]
    public async Task<ActionResult> Update(UserAccountUpdateModel model)
    {
        var listAlert = new List<AlertModel>();

        try
        {
            await _userAccountService.Update(model);
            listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = $"Update account" });
        }
        catch (Exception ex)
        {
            listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"{ex.Message}" });
        }

        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);

        return RedirectToAction("Index");
    }

    [CustomAuthenticationFilter]
    public async Task<ActionResult> ChangePassword()
    {
        string loginName = _httpContextAccessor?.HttpContext?.Session.GetString("LoginName") ?? "";
        var userAccount = await _userAccountService.GetByLoginName(loginName) ?? new Domain.UserAccount();
        var model = new UserAccountChangePasswordModel();
        model.Id = userAccount.Id;
        return View(model);
    }

    [HttpPost]
    [CustomAuthenticationFilter]
    public async Task<ActionResult> ChangePassword(UserAccountChangePasswordModel model)
    {
        var listAlert = new List<AlertModel>();

        try
        {
            await _userAccountService.ChangePassword(model);
            listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = $"Change password" });
        }
        catch (Exception ex)
        {
            listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"{ex.Message}" });
        }

        TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);

        return RedirectToAction("Index");
    }

    public ActionResult Logout()
    {
        // Clear the session or cookie used for authentication
        _httpContextAccessor?.HttpContext?.Session.Clear();

        return RedirectToAction("Login");
    }

}
