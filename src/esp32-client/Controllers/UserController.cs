using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using esp32_client.Models;
using esp32_client.Services;
using Newtonsoft.Json;
using esp32_client.Builder;

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

    // Login action
    [HttpGet]
    public ActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Login(string username, string password)
    {
        // Perform custom authentication logic here
        if (await _userAccountService.IsValidUser(username, password))
        {
            // Authentication successful
            // Store the username in session
            _httpContextAccessor?.HttpContext?.Session.SetString("Username", username);

            return RedirectToAction("Index", "Home");
        }

        // Authentication failed
        ModelState.AddModelError("", "Invalid username or password");
        return View();
    }

    // Logout action
    public ActionResult Logout()
    {
        // Clear the session or cookie used for authentication
        _httpContextAccessor?.HttpContext?.Session.Clear();

        return RedirectToAction("Login");
    }

}
