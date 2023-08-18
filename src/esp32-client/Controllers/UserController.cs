using Microsoft.AspNetCore.Mvc;
using esp32_client.Models;
using esp32_client.Services;
using Newtonsoft.Json;
using esp32_client.Builder;

namespace esp32_client.Controllers;

public class UserController : BaseController
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserAccountService _userAccountService;

    public UserController(LinqToDb linq2db, IHttpContextAccessor httpContextAccessor, IUserAccountService userAccountService)
    {
        _linq2db = linq2db;
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
        if (!String.IsNullOrEmpty(_httpContextAccessor?.HttpContext?.Session.GetString("LoginName")))
            return RedirectToAction("Index", "Home");
        return View();
    }



    [HttpPost]
    public async Task<IActionResult> Login(string loginName, string password)
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

                // Set user session for 30 minutes
                var expiredTime = DateTime.UtcNow.AddMinutes(30).ToString("o");
                _httpContextAccessor?.HttpContext?.Session.SetString("ExpiredTime", expiredTime);

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
    public async Task<IActionResult> Create(UserAccountCreateModel model)
    {
        return await HandleActionAsync(async () =>
        {
            await _userAccountService.Create(model);
        }, RedirectToAction("Index"));
    }

    [CustomAuthenticationFilter]
    public async Task<IActionResult> Update()
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
    public async Task<IActionResult> Update(UserAccountUpdateModel model)
    {
        return await HandleActionAsync(async () =>
        {
            await _userAccountService.Update(model);
        }, RedirectToAction("Index"));
    }

    [CustomAuthenticationFilter]
    public async Task<IActionResult> ChangePassword()
    {
        string loginName = _httpContextAccessor?.HttpContext?.Session.GetString("LoginName") ?? "";
        var userAccount = await _userAccountService.GetByLoginName(loginName) ?? new Domain.UserAccount();
        var model = new UserAccountChangePasswordModel();
        model.Id = userAccount.Id;
        return View(model);
    }

    [HttpPost]
    [CustomAuthenticationFilter]
    public async Task<IActionResult> ChangePassword(UserAccountChangePasswordModel model)
    {

        return await HandleActionAsync(async () =>
        {
            await _userAccountService.ChangePassword(model);
        }, RedirectToAction("Index"));
    }

    public ActionResult Logout()
    {
        // Clear the session or cookie used for authentication
        _httpContextAccessor?.HttpContext?.Session.Clear();

        return RedirectToAction("Login");
    }

}
