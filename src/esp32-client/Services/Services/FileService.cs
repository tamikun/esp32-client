using esp32_client.Models;


namespace esp32_client.Services;

/// <summary>
/// 
/// </summary>
public partial class FileService : IFileService
{
    private readonly Settings _settings;
    public FileService(Settings setting)
    {
        _settings = setting;
    }

    public virtual async Task<List<DataFileModel>> GetAll(string? directoryPath)
    {
        try
        {
            var response = new List<DataFileModel>();

            directoryPath = directoryPath ?? _settings.FileDataDirectory;

            var folders = (await GetFolders(directoryPath)).ToList();
            response.AddRange(folders);


            var files = (await GetFiles(directoryPath)).ToList();
            response.AddRange(files);

            return response;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("==== ex: " + Newtonsoft.Json.JsonConvert.SerializeObject(ex));
            throw;
        }
    }

    public virtual async Task<List<DataFileModel>> GetFolders(string? directoryPath)
    {
        var response = new List<DataFileModel>();
        directoryPath = directoryPath ?? _settings.FileDataDirectory;
        var folders = Directory.GetDirectories(directoryPath).OrderBy(q => q).ToList();

        foreach (var item in folders)
        {
            response.Add(new DataFileModel()
            {
                FilePath = item,
                FileType = "directory",
            });
        }
        await Task.CompletedTask;

        return response;
    }

    public async Task<List<DataFileModel>> GetFiles(string? directoryPath)
    {
        var response = new List<DataFileModel>();

        directoryPath = directoryPath ?? _settings.FileDataDirectory;

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

    public async Task<Dictionary<string, object>> GetDictionaryFile(string? directoryPath)
    {
        var rs = await GetAll(directoryPath);

        Dictionary<string, object> dict = new Dictionary<string, object>();

        foreach (var item in rs.Where(s => s.FileType == "file"))
        {
            dict.TryAdd(item?.FilePath?.Split('/').LastOrDefault() ?? "", item?.FilePath ?? "");
        }

        foreach (var item in rs.Where(s => s.FileType == "directory"))
        {
            dict.TryAdd(item?.FilePath?.Split('/').LastOrDefault() ?? "", await GetDictionaryFile(item?.FilePath));
        }

        return dict;
    }

    public async Task WriteFile(IFormFile file, string directoryPath)
    {
        using (var ms = new MemoryStream())
        {
            file.CopyTo(ms);
            var fileBytes = ms.ToArray();
            await File.WriteAllBytesAsync(directoryPath + file.FileName, fileBytes);
        }
    }

    public async Task DeleteFile(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            var files = Directory.GetFileSystemEntries(directoryPath);
            if (files.Any())
            {
                foreach (var file in files)
                {
                    await DeleteFile(file);
                }
            }
            Directory.Delete(directoryPath);
        }
        else if (File.Exists(directoryPath))
        {
            File.Delete(directoryPath);
        }
        await Task.CompletedTask;
    }

}