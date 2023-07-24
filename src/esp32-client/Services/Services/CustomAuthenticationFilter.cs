using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace esp32_client.Services
{

    public class CustomAuthenticationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            // Check if the user is authenticated
            if (!filterContext.HttpContext.Session.TryGetValue("LoginName", out var _sessionValue) || _sessionValue == null)
            {
                // User is not authenticated, redirect to login page
                filterContext.Result = new RedirectResult("/User/Login");
            }

            base.OnActionExecuting(filterContext);
        }
    }

}