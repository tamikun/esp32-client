using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IClientService
    {
        List<ServerModel> GetAvailableIpAddress();

        Task<HttpResponseMessage> PostAsyncApi(string? requestBody, string apiUrl);

        Task<HttpResponseMessage> PostAsyncFile(IFormFile newFile, string filePath, string ipAddress);
        Task<HttpResponseMessage> PostAsyncFile(byte[] byteContent, string filePath, string ipAddress);

        Task<string> GetAsyncApi(string apiUrl, bool throwException = true);

        Task<List<EspFileModel>> GetListEspFile(string apiUrl, string node = "//table[@class='fixed']/tbody/tr");

        Task<Dictionary<string, object>> GetDictionaryFileWithNode(string ipAddress = "http://192.168.101.84/", string node = "//table[@class='fixed']/tbody/tr");

        Task<Dictionary<string, object>> GetDictionaryFile(string ipAddress = "http://192.168.101.84/", string node = "//table[@class='fixed']/tbody/tr");

        Task<List<SelectedServerModel>> GetListSelectedServer(string ipAddress = "http://192.168.101.84/", string folder = "",string node = "//table[@class='fixed']/tbody/tr");
    }
}