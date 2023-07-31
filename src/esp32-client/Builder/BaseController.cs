using esp32_client.Builder;
using esp32_client.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace esp32_client.Controllers
{
    public class BaseController : Controller
    {
#nullable disable
        protected LinqToDb _linq2db;

        protected async Task<IActionResult> HandleActionAsync(Func<Task> action, IActionResult redirect)
        {
            await _linq2db.BeginTransactionAsync();

            var listAlert = new List<AlertModel>();
            try
            {
                await action.Invoke();
                listAlert.Add(new AlertModel { AlertType = Alert.Success, AlertMessage = "Action successful." });
                TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);

                _linq2db.CommitTransaction();
            }
            catch (Exception ex)
            {
                listAlert.Add(new AlertModel { AlertType = Alert.Danger, AlertMessage = ex.Message });
                TempData["AlertMessage"] = JsonConvert.SerializeObject(listAlert);

                await _linq2db.RollbackTransactionAsync();
            }
            return redirect;
        }
    }

}