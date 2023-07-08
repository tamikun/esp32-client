using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using esp32_client.Models;
using HtmlAgilityPack;
using System.Linq;

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
                                return;
                            }

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Success @{address}");
                            response.Add(new ServerModel() { IpAddress = address });

                            client.Close();
                        }
                        catch//(SocketException ex)
                        { }
                    }
                }).Wait();
            }
        });

        // Access the response list outside the parallel loop
        var resultList = response.OrderBy(s => int.Parse(s?.IpAddress?.Split('.').LastOrDefault() ?? Int32.MaxValue.ToString())).ToList();

        sw.Stop();
        System.Console.WriteLine("==== sw: " + Newtonsoft.Json.JsonConvert.SerializeObject(sw.ElapsedMilliseconds));

        return resultList;
    }

    public async Task<List<ServerModel>> GetStaticIpAddress()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        var listIp = _configuration["Settings:ListServer"].ToString().Split(';').ToList();
        // string nodeServerName = _configuration["Settings:NodeServerName"].ToString();
        // int timeout = int.Parse(_configuration["Settings:GetDataTimeOut"].ToString());

        var responseTasks = listIp
            .Where(ip => !string.IsNullOrEmpty(ip))
            .Select(async ip =>
            {
                var serverModel = new ServerModel()
                {
                    IpAddress = ip
                };

                // var getServerNameTask = GetServerName($"http://{ip}/", nodeServerName);
                serverModel.ServerName = await GetServerName($"http://{ip}/");
                serverModel.ServerState = await GetServerState($"http://{ip}/");

                return serverModel;
            });

        var response = await Task.WhenAll(responseTasks);

        sw.Stop();
        System.Console.WriteLine($"==== GetStaticIpAddress completed in {sw.ElapsedMilliseconds} ms");
        return response.ToList();
    }

    // public virtual async Task<string> GetServerName(string url, string node)
    // {
    //     try
    //     {
    //         var pageData = await GetAsyncApi(url, true);

    //         string html = pageData;

    //         HtmlDocument htmlDoc = new HtmlDocument();
    //         htmlDoc.LoadHtml(html);
    //         HtmlNodeCollection selectedNodes = htmlDoc.DocumentNode.SelectNodes(node);
    //         if (selectedNodes != null && selectedNodes.Count > 0)
    //         {
    //             return selectedNodes[0].InnerText;
    //         }
    //         return "";
    //     }
    //     catch
    //     {
    //         return "";
    //     }
    // }
    public virtual async Task<string> GetServerName(string ip)
    {
        try
        {
            var config = _configuration["Settings:ServerNamePath"].ToString();
            var newurl = $"{ip}{config}";
            var pageData = await GetAsyncApi(newurl, true);

            string nodeServerName = _configuration["Settings:NodeServerName"].ToString();

            return pageData;
        }
        catch
        {
            return "";
        }
    }

    public virtual async Task<ServerState> GetServerState(string ip)
    {
        try
        {
            var node = _configuration["Settings:NodeServerState"].ToString();
            var html = await GetAsyncApi(ip, true);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            HtmlNodeCollection selectedNodes = htmlDoc.DocumentNode.SelectNodes(node);
            if (selectedNodes != null && selectedNodes.Count > 0)
            {
                if (selectedNodes[0].InnerHtml == "Device is attached to: Server")
                    return ServerState.Server;

                return ServerState.Machine;
            }

            return ServerState.Unknown;
        }
        catch
        {
            return ServerState.Unknown;
        }
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

            return response;
        }
    }

    public virtual async Task<HttpResponseMessage> PostAsyncFile(byte[] byteContent, string filePath, string ipAddress)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        var url = $"{ipAddress}upload/{filePath}";

        System.Console.WriteLine("==== PostAsyncFile url: " + Newtonsoft.Json.JsonConvert.SerializeObject(url));

        using (var httpClient = new HttpClient())
        {
            httpClient.Timeout = TimeSpan.FromMilliseconds(long.Parse(_configuration["Settings:PostFileTimeOut"].ToString()));

            var fileContent = new ByteArrayContent(byteContent);
            var response = await httpClient.PostAsync(url, fileContent);
            // using (var content = new MultipartFormDataContent())
            // {

            //     // Add file content
            //     content.Add(fileContent);

            //     // Send the request

            //     sw.Stop();

            //     System.Console.WriteLine("==== PostAsyncFile ElapsedMilliseconds: " + Newtonsoft.Json.JsonConvert.SerializeObject(sw.ElapsedMilliseconds));

            // }
            return response;
        }
    }

    public virtual async Task<string> GetAsyncApi(string apiUrl, bool throwException = true)
    {
        try
        {
            System.Console.WriteLine("==== GetAsyncApi: " + apiUrl);
            long timeout = long.Parse(_configuration["Settings:GetDataTimeOut"].ToString());
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMilliseconds(timeout);

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

    public virtual async Task<List<EspFileModel>> GetListEspFile(string apiUrl)
    {
        string node = _configuration["Settings:NodeListEspFile"].ToString();
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

    public virtual async Task<Dictionary<string, object>> GetDictionaryFileWithNode(string ipAddress = "http://192.168.101.84/")
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add(ipAddress, await GetDictionaryFile(ipAddress));
        return dict;
    }

    public virtual async Task<Dictionary<string, object>> GetDictionaryFile(string ipAddress = "http://192.168.101.84/")
    {
        string node = _configuration["Settings:NodeListEspFile"].ToString();

        Dictionary<string, object> dict = new Dictionary<string, object>();

        try
        {
            var pageData = await GetAsyncApi(ipAddress, true);

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
                            dict.Add($"{ipAddress}{fileName}/", await GetDictionaryFile(ipAddress: newAddress));
                        }
                        else
                        {
                            dict.Add($"{ipAddress}{fileName}/", $"{ipAddress}{fileName}/");
                        }
                    }
                }
            }
        }
        catch
        {

        }
        return dict;
    }

    public virtual async Task<HttpResponseMessage> DeleteFile(string ipAddress, string subDirectory, string fileName)
    {
        string url = "";

        if (string.IsNullOrEmpty(subDirectory))
        {
            url = $"{ipAddress}delete/{fileName}";
        }
        else
        {
            url = $"{ipAddress}delete/{subDirectory}/{fileName}";
        }

        System.Console.WriteLine("==== delete: " + url);

        var response = await PostAsyncApi(requestBody: null, apiUrl: url);

        return response;
    }
}