using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IClientService
    {
        List<ServerModel> GetAvailableIpAddress();
        Task<List<ServerModel>> GetStaticIpAddress();

        Task<HttpResponseMessage> PostAsyncApi(string? requestBody, string apiUrl);

        Task<HttpResponseMessage> PostAsyncFile(byte[] byteContent, string filePath, string ipAddress);

        Task<string> GetAsyncApi(string apiUrl, bool throwException = true);

        Task<List<EspFileModel>> GetListEspFile(string apiUrl);

        Task<Dictionary<string, object>> GetDictionaryFileWithNode(string ipAddress = "http://192.168.101.84/");

        Task<Dictionary<string, object>> GetDictionaryFile(string ipAddress = "http://192.168.101.84/");

        Task<HttpResponseMessage> DeleteFile(string ipAddress, string subDirectory, string fileName);

        Task<string> GetServerName(string url);
    }
}