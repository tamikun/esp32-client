using Microsoft.AspNetCore.Mvc;
using esp32_client.Models;
using esp32_client.Services;
using Newtonsoft.Json;
using esp32_client.Builder;
using esp32_client.Models.Singleton;

namespace esp32_client.Controllers;

public class UserController : BaseController
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserAccountService _userAccountService;
    private readonly IUserSessionService _userSessionService;
    private readonly Settings _settings;

    public UserController(LinqToDb linq2db, IHttpContextAccessor httpContextAccessor, IUserAccountService userAccountService,
                        Settings settings, IUserSessionService userSessionService)
    {
        _linq2db = linq2db;
        _httpContextAccessor = httpContextAccessor;
        _userAccountService = userAccountService;
        _settings = settings;
        _userSessionService = userSessionService;
    }

    [Authentication]
    public ActionResult Index()
    {
        return View();
    }

    public ActionResult Login()
    {
        if (!String.IsNullOrEmpty(_httpContextAccessor?.HttpContext?.Session.GetString("Token")))
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
                // // Authentication successful
                // // Store the username in session
                // _httpContextAccessor?.HttpContext?.Session.SetString("LoginName", loginName);
                // _httpContextAccessor?.HttpContext?.Session.SetString("UserName", userAccount.UserName);

                // // Set user session for _settings.MinutesPerSession minutes
                // var expiredTime = DateTime.UtcNow.AddMinutes(_settings.MinutesPerSession).ToString("o");
                // _httpContextAccessor?.HttpContext?.Session.SetString("ExpiredTime", expiredTime);

                // // Store the role in session
                // var userRight = await _userAccountService.GetUserRight(loginName);
                // if (userRight is not null)
                //     _httpContextAccessor?.HttpContext?.Session.SetString("UserRight", JsonConvert.SerializeObject(userRight));
                var token = JwtToken.GenerateJwtToken(userAccount.Id, userAccount.UserName, userAccount.LoginName);

                _httpContextAccessor?.HttpContext?.Session.SetString("Token", token);

                await _userSessionService.Create(new UserSessionCreateModel
                {
                    UserId = userAccount.Id,
                    Token = token
                });

                return RedirectToAction("Index", "Home");
            }
        }


        // Authentication failed
        ModelState.AddModelError("", "Invalid username or password");
        return View();
    }

    [Authentication]
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authentication]
    public async Task<IActionResult> Create(UserAccountCreateModel model)
    {
        return await HandleActionAsync(async () =>
        {
            await _userAccountService.Create(model);
        }, RedirectToAction("Index"));
    }

    [Authentication]
    public async Task<IActionResult> Update()
    {
        // string loginName = _httpContextAccessor?.HttpContext?.Session.GetString("LoginName") ?? "";
        var token = _httpContextAccessor?.HttpContext?.Session.GetString("Token");
        var user = JwtToken.GetDataFromToken(token);

        var userAccount = await _userAccountService.GetByLoginName(user.LoginName) ?? new Domain.UserAccount();
        var model = new UserAccountUpdateModel();
        model.Id = userAccount.Id;
        model.UserName = userAccount.UserName;
        model.LoginName = userAccount.LoginName;
        return View(model);
    }

    [HttpPost]
    [Authentication]
    public async Task<IActionResult> Update(UserAccountUpdateModel model)
    {
        return await HandleActionAsync(async () =>
        {
            await _userAccountService.Update(model);
        }, RedirectToAction("Index"));
    }

    [Authentication]
    public async Task<IActionResult> ChangePassword()
    {
        // string loginName = _httpContextAccessor?.HttpContext?.Session.GetString("LoginName") ?? "";

        var token = _httpContextAccessor?.HttpContext?.Session.GetString("Token");
        var user = JwtToken.GetDataFromToken(token);

        var userAccount = await _userAccountService.GetByLoginName(user.LoginName) ?? new Domain.UserAccount();
        var model = new UserAccountChangePasswordModel();
        model.Id = userAccount.Id;
        return View(model);
    }

    [HttpPost]
    [Authentication]
    public async Task<IActionResult> ChangePassword(UserAccountChangePasswordModel model)
    {

        return await HandleActionAsync(async () =>
        {
            await _userAccountService.ChangePassword(model);
        }, RedirectToAction("Index"));
    }

    [Authentication]
    public async Task<IActionResult> Session()
    {
        var token = _httpContextAccessor?.HttpContext?.Session.GetString("Token");
        var user = JwtToken.GetDataFromToken(token);

        ViewBag.UserId = user.UserId;
        ViewBag.Token = token;
        
        await Task.CompletedTask;

        return View();
    }

    [HttpPost]
    [Authentication]
    public async Task<IActionResult> DeleteSession(int sessionId)
    {

        return await HandleActionAsync(async () =>
        {
            await _userSessionService.Delete(sessionId);
        }, RedirectToAction("Session"));
    }
    public async Task<IActionResult> Logout()
    {
        var token = _httpContextAccessor?.HttpContext?.Session.GetString("Token");
        await _userSessionService.Delete(token ?? "");

        // Clear the session or cookie used for authentication
        _httpContextAccessor?.HttpContext?.Session.Clear();

        return RedirectToAction("Login");
    }

}
