using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IFileService
    {
        Task<List<DataFileModel>> GetAll(string? directoryPath);

    }
}