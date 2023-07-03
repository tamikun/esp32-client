using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using esp32_client.Models;
using HtmlAgilityPack;

namespace esp32_client.Services;

/// <summary>
/// 
/// </summary>
public partial class ClientService : IClientService
{
    private readonly IConfiguration _configuration;

    public ClientService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public virtual List<ServerModel> GetAvailableIpAddress()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        var configSubnet = _configuration["Settings:Subnet"];
        var configPort = int.Parse(_configuration["Settings:Port"]);
        var configConnectionTimeOut = int.Parse(_configuration["Settings:ConnectionTimeOut"]);

        System.Console.WriteLine("configSubnet " + configSubnet);
        System.Console.WriteLine("configPort " + configPort);
        System.Console.WriteLine("configConnectionTimeOut " + configConnectionTimeOut);

        string GetAddress(string subnet, int i)
        {
            return new StringBuilder(subnet).Append(i).ToString();
        }

        var response = new ConcurrentBag<ServerModel>();

        Parallel.ForEach(Partitioner.Create(0, 254), range =>
        {
            for (int i = range.Item1; i < range.Item2; i++)
            {
                string address = GetAddress(configSubnet, i);

                Task.Run(async () =>
                {
                    using (var client = new TcpClient())
                    {
                        try
                        {
                            var connectTask = client.ConnectAsync(IPAddress.Parse(address), configPort);

                            if (await Task.WhenAny(connectTask, Task.Delay(configConnectionTimeOut)) != connectTask || !client.Connected)
                            {
                                // Connection attempt timed out or failed
                                // Console.ForegroundColor = ConsoleColor.Red;
                                // Console.WriteLine($"Failed @{address}");
                                return;
                            }

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Success @{address}");
                            response.Add(new ServerModel() { IpAddress = address });

                            client.Close();
                        }
                        catch//(SocketException ex)
                        {
                            // Console.ForegroundColor = ConsoleColor.Red;
                            // Console.WriteLine($"Failed @{address} Error code: {ex.ErrorCode}");
                        }
                    }
                }).Wait();
            }
        });

        // Access the response list outside the parallel loop
        // var resultList = response.OrderBy(s => s.IpAddress).ToList();
        var resultList = response.OrderBy(s => int.Parse(s?.IpAddress?.Split('.').LastOrDefault() ?? Int32.MaxValue.ToString())).ToList();

        sw.Stop();
        System.Console.WriteLine("==== sw: " + Newtonsoft.Json.JsonConvert.SerializeObject(sw.ElapsedMilliseconds));

        return resultList;
    }

    public virtual async Task<HttpResponseMessage> PostAsyncApi(string? requestBody, string apiUrl)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response;
            if (requestBody is null)
            {
                response = await client.PostAsync(apiUrl, null);
            }
            else
            {
                HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                response = await client.PostAsync(apiUrl, content);
            }

            // if (response.IsSuccessStatusCode)
            // {
            //     string responseBody = await response.Content.ReadAsStringAsync();
            //     return responseBody;
            // }
            // else
            // {
            // }
            return response;
        }
    }

    public virtual async Task<HttpResponseMessage> PostAsyncFile(IFormFile newFile, string filePath, string ipAddress)
    {
        System.Console.WriteLine(newFile.FileName);
        System.Console.WriteLine(newFile.ContentType);
        System.Console.WriteLine(filePath);
        System.Console.WriteLine(ipAddress);

        // ipAddress += $"/{filePath}";

        var url = $"http://{ipAddress}/upload/{filePath}";

        using (var httpClient = new HttpClient())
        {
            using (var content = new MultipartFormDataContent())
            {
                // Add other form data keys
                // No need (filepath in request url)
                // content.Add(new StringContent(filePath), nameof(filePath));

                // Add file content
                var fileContent = new ByteArrayContent(await GetBytesFromFormFile(newFile));
                content.Add(fileContent, nameof(newFile), newFile.FileName);

                // Send the request
                var response = await httpClient.PostAsync(url, content);

                return response;
            }
        }
    }

    private async Task<byte[]> GetBytesFromFormFile(IFormFile file)
    {
        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }

    public virtual async Task<string> GetAsyncApi(string apiUrl, bool throwException = true)
    {
        try
        {
            System.Console.WriteLine("==== GetAsyncApi: " + apiUrl);
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                return response.StatusCode.ToString();
            }
        }
        catch (Exception ex)
        {
            if (throwException)
            {
                throw;
            }
            else
            {
                return ex.ToString();
            }
        }
    }

    public virtual async Task<List<EspFileModel>> GetListEspFile(string apiUrl, string node = "//table[@class='fixed']/tbody/tr")
    {
        var pageData = await GetAsyncApi(apiUrl, false);

        string html = pageData;

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
                    fileDataList.Add(fileData);
                }
            }
        }
        return fileDataList;
    }

    public virtual async Task<Dictionary<string, object>> GetDictionaryFileWithNode(string ipAddress = "http://192.168.101.84/", string node = "//table[@class='fixed']/tbody/tr")
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add(ipAddress, await GetDictionaryFile(ipAddress, node));
        return dict;
    }

    public virtual async Task<Dictionary<string, object>> GetDictionaryFile(string ipAddress = "http://192.168.101.84/", string node = "//table[@class='fixed']/tbody/tr")
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();

        var pageData = await GetAsyncApi(ipAddress, false);

        string html = pageData;

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
                    var fileName = tableCells[0].InnerText;
                    var fileType = tableCells[1].InnerText;
                    var fileSize = long.Parse(tableCells[2].InnerText.Trim());

                    if (fileType == "directory")
                    {
                        var newAddress = $"{ipAddress}{fileName}/";
                        dict.Add(fileName, await GetDictionaryFile(ipAddress: newAddress));
                    }
                    else
                    {
                        dict.Add(fileName, $"{ipAddress}{fileName}/");
                    }
                }
            }
        }
        return dict;
    }
}