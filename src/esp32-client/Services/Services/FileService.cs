using esp32_client.Models;


namespace esp32_client.Services;

/// <summary>
/// 
/// </summary>
public partial class FileService : IFileService
{
    private readonly IConfiguration _configuration;
    public FileService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public virtual async Task<List<DataFileModel>> GetAll(string? directoryPath)
    {
        try
        {

            var response = new List<DataFileModel>();

            directoryPath = directoryPath ?? _configuration["Settings:FileDataDirectory"].ToString();

            var folders = Directory.GetDirectories(directoryPath).OrderBy(q => q).ToList();

            foreach (var item in folders)
            {
                response.Add(new DataFileModel()
                {
                    FilePath = item,
                    FileType = "directory"
                });
            }

            var files = Directory.GetFiles(directoryPath).OrderBy(q => q).ToList();

            foreach (var item in files)
            {
                response.Add(new DataFileModel()
                {
                    FilePath = item,
                    FileType = "file",
                    FileSize = new FileInfo(item).Length
                });
            }


            await Task.CompletedTask;
            return response;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("==== ex: " + Newtonsoft.Json.JsonConvert.SerializeObject(ex));
            throw;
        }
    }
}