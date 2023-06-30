using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IClientService
    {
        List<ServerModel> GetAvailableIpAddress();

        Task<HttpResponseMessage> PostAsyncApi(string? requestBody, string apiUrl);

        Task<HttpResponseMessage> PostAsyncFile(IFormFile newFile, string filePath, string ipAddress);

        Task<string> GetAsyncApi(string apiUrl, bool throwException = true);

        Task<List<EspFileModel>> GetListEspFile(string apiUrl, string node = "//table[@class='fixed']/tbody/tr");
    }
}