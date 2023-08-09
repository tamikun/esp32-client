using System.Globalization;
using esp32_client.Domain;
using esp32_client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace esp32_client.Services
{

    public class CustomAuthenticationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            var loginName = filterContext.HttpContext.Session.GetString("LoginName");

            var expiredTimeString = filterContext.HttpContext.Session.GetString("ExpiredTime");
            DateTime expiredTime = DateTime.UtcNow.AddDays(-1);
            var parseDateTime = DateTime.TryParseExact(expiredTimeString, "o", null, DateTimeStyles.None, out expiredTime);

            // Check if the user is authenticated
            if (string.IsNullOrEmpty(loginName))
            {
                // User is not authenticated, redirect to login page
                filterContext.Result = new RedirectResult("/User/Login");
            }
            else if (expiredTime < DateTime.UtcNow)
            {
                filterContext.Result = new RedirectResult("/User/Logout");
            }
            else
            {
                // Get the name of the controller
                string? controllerName = filterContext.RouteData.Values["controller"]?.ToString();

                // Get the name of the action method
                string? actionName = filterContext.RouteData.Values["action"]?.ToString();

                var strUserRight = filterContext.HttpContext.Session.GetString("UserRight");
                var userRight = JsonConvert.DeserializeObject<List<UserRight>>(strUserRight ?? "");
                if (userRight is not null)
                {
                    var checkUserRight = userRight.Any(s =>
                                        (s.ControllerName == "*" && s.ActionName == "*")
                                        || (s.ControllerName == "*" && s.ActionName == actionName)
                                        || (s.ControllerName == controllerName && s.ActionName == "*")
                                        || (s.ControllerName == controllerName && s.ActionName == actionName)
                                        );

                    if (!checkUserRight)
                    {
                        var listAlert = new List<AlertModel>();
                        listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"Unauthorized" });

                        // Access TempData via the Controller property and store the data
                        var controller = filterContext.Controller as Microsoft.AspNetCore.Mvc.Controller;
                        if (controller is null)
                        {
                            System.Console.WriteLine("Controller is null.......");
                        }
                        else
                        {
                            controller.TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);
                        }

                        filterContext.Result = new RedirectResult($"/{controllerName}");
                    }
                }
            }

            base.OnActionExecuting(filterContext);
        }

    }


}