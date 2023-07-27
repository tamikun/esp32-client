// using System.Text;
// using esp32_client.Services;
// using Microsoft.AspNetCore.Cors;
// using Microsoft.AspNetCore.Mvc;
// using Newtonsoft.Json;
// using Newtonsoft.Json.Linq;

// namespace esp32_client.Controllers;

// [ApiController]
// [Route("api/[controller]/[action]")]
// public class OpenApiController : ControllerBase
// {
//     private readonly ILogger<HomeController> _logger;
//     private readonly IClientService _clientService;
//     private readonly Settings _settings;

//     public OpenApiController(ILogger<HomeController> logger, IClientService clientService, Settings settings)
//     {
//         _logger = logger;
//         _clientService = clientService;
//         _settings = settings;
//     }

//     [HttpPost]
//     [ProducesResponseType(StatusCodes.Status201Created)]
//     [ProducesResponseType(StatusCodes.Status400BadRequest)]
//     public async Task<IActionResult> ScanningIP(string subnet = "192.168.101.", int port = 80)
//     {
//         var result = _clientService.GetAvailableIpAddress();
//         await Task.CompletedTask;
//         var rs = Newtonsoft.Json.JsonConvert.SerializeObject(result);
//         return Ok(rs);
//     }

//     [HttpPost]
//     [ProducesResponseType(StatusCodes.Status201Created)]
//     [ProducesResponseType(StatusCodes.Status400BadRequest)]
//     public async Task<IActionResult> GetAsyncApi(string apiUrl = "https://jsonplaceholder.typicode.com/todos")
//     {
//         var result = await _clientService.GetAsyncApi(apiUrl);
//         return Ok(result);
//     }

//     // [HttpPost]
//     // [ProducesResponseType(StatusCodes.Status201Created)]
//     // [ProducesResponseType(StatusCodes.Status400BadRequest)]
//     // public async Task<IActionResult> PostAsyncFile()
//     // {
//     //     System.Console.WriteLine("==== here PostAsyncFile");
//     //     var newFile = Request.Form.Files.FirstOrDefault();
//     //     var filePath = Request.Form["filePath"].FirstOrDefault();
//     //     var ipAddress = Request.Form["ipAddress"].FirstOrDefault();

//     //     var result = await _clientService.PostAsyncFile(newFile, filePath, $"http://{ipAddress}/");

//     //     return Ok(result);
//     // }

//     [HttpGet]
//     [ProducesResponseType(StatusCodes.Status201Created)]
//     [ProducesResponseType(StatusCodes.Status400BadRequest)]
//     public async Task<IActionResult> DownloadFile(string url)
//     {
//         long timeout = _settings.GetDataTimeOut;
//         using (HttpClient client = new HttpClient())
//         {
//             client.Timeout = TimeSpan.FromMilliseconds(timeout);

//             HttpResponseMessage response = await client.GetAsync(url);

//             if (response.IsSuccessStatusCode)
//             {
//                 var content = await response.Content.ReadAsByteArrayAsync();

//                 string? fileName = url.Split('/').Where(s => !string.IsNullOrEmpty(s)).LastOrDefault();
//                 string? fileType = fileName?.Split('.').LastOrDefault();
//                 string contentType = GetContentType(fileType?.ToLower());

//                 return File(content, contentType, fileDownloadName: fileName);
//             }
//         }
//         return Ok();
//     }

//     [HttpGet]
//     [ProducesResponseType(StatusCodes.Status201Created)]
//     [ProducesResponseType(StatusCodes.Status400BadRequest)]
//     public async Task<IActionResult> DownloadFileServer(string path)
//     {
//         if (System.IO.File.Exists(path))
//         {
//             var fileName = path.Split('/').Where(s => !string.IsNullOrEmpty(s)).LastOrDefault();
//             var fileBytes = await System.IO.File.ReadAllBytesAsync(path);

//             string contentType = GetContentType(fileName?.Split('.')?.LastOrDefault()?.ToLower());

//             return File(fileBytes, contentType, fileDownloadName: fileName);
//         }
//         return Ok();
//     }

//     private string GetContentType(string? fileType)
//     {
//         string contentType = "";
//         switch (fileType?.ToLower())
//         {
//             case "txt":
//                 contentType = "text/plain";
//                 break;
//             case "ico":
//                 contentType = "image/x-icon";
//                 break;
//             case "json":
//                 contentType = "application/json";
//                 break;
//             case "css":
//                 contentType = "text/css";
//                 break;
//             default:
//                 contentType = "application/octet-stream";
//                 break;
//         }
//         return contentType;
//     }

// }
