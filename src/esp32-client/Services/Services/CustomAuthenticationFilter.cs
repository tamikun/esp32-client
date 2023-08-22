using esp32_client.Builder;
using esp32_client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace esp32_client.Services
{
    public class Authentication : ActionFilterAttribute
    {
        // public override void OnActionExecuting(ActionExecutingContext context)
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var token = context.HttpContext.Session.GetString("Token");
            var listAlert = new List<AlertModel>();
            if (token is null)
            {
                // Did not log in. Do not show alert
                context.Result = new RedirectResult("/User/Login");
            }
            else
            {
                var userSessionService = EngineContext.Resolve<IUserSessionService>();

                if (await userSessionService.CheckToken(token))
                {
                    try
                    {
                        var user = JwtToken.GetDataFromToken(token);

                        var userAccountService = EngineContext.Resolve<IUserAccountService>();

                        // Get the name of the controller
                        string? controllerName = context.RouteData.Values["controller"]?.ToString();

                        // Get the name of the action method
                        string? actionName = context.RouteData.Values["action"]?.ToString();

                        //Check ...
                        var checkUserRight = await userAccountService.CheckUserRight(user.LoginName, controllerName, actionName);

                        if (!checkUserRight)
                        {
                            listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"Unauthorized" });
                            context.Result = new RedirectResult($"/Home/Error");
                        }
                        else
                        {
                            await next();
                        }

                    }
                    catch (SecurityTokenExpiredException)
                    {
                        // Token has expired
                        listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"Session has expired" });
                        context.Result = new RedirectResult("/User/Logout");
                    }
                    catch (SecurityTokenException)
                    {
                        // Token validation failed for other reasons
                        listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"Invalid token" });
                        context.Result = new RedirectResult("/User/Logout");
                    }
                    catch (Exception ex)
                    {
                        listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"{ex.Message}" });
                        context.Result = new RedirectResult("/User/Logout");
                    }
                }
                else
                {
                    listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = $"Unauthorized" });

                    context.Result = new RedirectResult("/User/Logout");
                }
            }

            var controller = context.Controller as Microsoft.AspNetCore.Mvc.Controller;
            if (controller is null)
            {
                System.Console.WriteLine("Controller is null.......");
            }
            else
            {
                controller.TempData["AlertMessageAuthentication"] = JsonConvert.SerializeObject(listAlert);
            }

            // base.OnActionExecuting(context);
        }

    }
}