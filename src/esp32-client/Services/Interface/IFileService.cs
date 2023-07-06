using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IFileService
    {
        Task<List<DataFileModel>> GetAll(string? directoryPath);

        Task<List<DataFileModel>> GetFolders(string? directoryPath);

        Task<List<DataFileModel>> GetFiles(string? directoryPath);

        Task<Dictionary<string, object>> GetDictionaryFile(string? directoryPath);

    }
}