using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using esp32_client.Models;
using esp32_client.Services;
using Newtonsoft.Json;

namespace esp32_client.Controllers;

public class UserController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserController(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // Login action
    [HttpGet]
    public ActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Login(string username, string password)
    {
        // Perform custom authentication logic here
        if (IsValidUser(username, password))
        {
            // Authentication successful
            // Store the username in session
            _httpContextAccessor.HttpContext.Session.SetString("Username", username);

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
        _httpContextAccessor.HttpContext.Session.Clear();

        return RedirectToAction("Login");
    }

    // Custom authentication logic
    private bool IsValidUser(string username, string password)
    {
        // Validate the username and password against your data source (e.g., database)
        // Return true if the user is valid, false otherwise
        // You can also perform additional checks, such as checking for password hash, etc.
        // For simplicity, this example assumes a hardcoded username and password
        return (username == "admin" && password == "password");
    }
}
