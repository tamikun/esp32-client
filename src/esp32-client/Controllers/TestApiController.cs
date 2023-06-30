using esp32_client.Models;
using esp32_client.Services;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace esp32_client.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TestApiController : ControllerBase
{
    private readonly ILogger<HomeController> _logger;
    private readonly IClientService _clientService;

    public TestApiController(ILogger<HomeController> logger, IClientService clientService)
    {
        _logger = logger;
        _clientService = clientService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsyncFile()
    {
        var newFile = Request.Form.Files.FirstOrDefault();
        var filePath = Request.Form["filePath"].FirstOrDefault();
        var postUrl = Request.Form["postUrl"].FirstOrDefault();

        var rs = new
        {
            FileName = newFile?.FileName,
            FileSize = newFile?.Length,
            FileType = newFile?.ContentType,
            FilePath = filePath,
            PostUrl = postUrl
        };
        await Task.CompletedTask;
        return Ok(rs);
    }

    public class PostJsonModel
    {
        public string? Key1 { get; set; }
        public string? Key2 { get; set; }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostJson(string? requestBody, string apiUrl = "https://api.example.com/api/endpoint")
    {
        var rs = await _clientService.PostAsyncApi(requestBody, apiUrl);
        return Ok(rs);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get(int id)
    {
        await Task.CompletedTask;
        return Ok(id * 10);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetListEspFile(string html, string node = "//table[@class='fixed']/tbody/tr")
    {
        List<EspFileModel> fileDataList = new List<EspFileModel>();

        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        HtmlNodeCollection tableRows = htmlDoc.DocumentNode.SelectNodes(node);
        if (tableRows != null && tableRows.Count > 0)
        {
            foreach (HtmlNode row in tableRows)
            {
                HtmlNodeCollection tableCells = row.SelectNodes("td");
                if (tableCells != null && tableCells.Count >= 4)
                {
                    EspFileModel fileData = new EspFileModel();
                    fileData.FileName = tableCells[0].InnerText.Trim();
                    fileData.FileType = tableCells[1].InnerText.Trim();
                    fileData.FileSize = long.Parse(tableCells[2].InnerText.Trim());
                    // fileData.DeleteActionUrl = tableCells[3].SelectSingleNode("form/@action").GetAttributeValue("value", "");
                    fileDataList.Add(fileData);
                }
            }
        }
        await Task.CompletedTask;
        return Ok(fileDataList);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetListEspFile2(string apiUrl, string node = "//table[@class='fixed']/tbody/tr")
    {
        var result = await _clientService.GetListEspFile(apiUrl, node);
        return Ok(result);
    }

}
