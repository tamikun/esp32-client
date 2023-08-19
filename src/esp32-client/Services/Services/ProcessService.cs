using AutoMapper;
using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
using esp32_client.Models.Singleton;
using LinqToDB;

namespace esp32_client.Services;

/// <summary>
/// 
/// </summary>
public partial class ProcessService : IProcessService
{

    private readonly LinqToDb _linq2Db;
    private readonly IProductService _productService;
    private readonly IMapper _mapper;
    private readonly Settings _settings;

    public ProcessService(LinqToDb linq2Db, IMapper mapper, IProductService productService, Settings settings)
    {
        _linq2Db = linq2Db;
        _mapper = mapper;
        _productService = productService;
        _settings = settings;
    }

    public async Task<Process?> GetById(int id)
    {
        var process = await _linq2Db.Process.Where(s => s.Id == id).FirstOrDefaultAsync();
        return process;
    }

    public async Task<List<Process>> GetByProductId(int id)
    {
        var process = await _linq2Db.Process.Where(s => s.ProductId == id).ToListAsync();
        return process;
    }

    public async Task<Process?> GetByProcessName(string name)
    {
        var process = await _linq2Db.Process.Where(s => s.ProcessName == name).FirstOrDefaultAsync();
        return process;
    }

    public async Task<List<Process>> GetAll()
    {
        return await _linq2Db.Process.ToListAsync();
    }

    public async Task<ProcessUpdateModel> UpdateProcessNamePatternNoById(ProcessUpdateModel model)
    {
        var process = await GetById(model.Id);

        if (process is null) throw new Exception("Process is not found");

        process.ProcessName = model.ProcessName;
        process.Description = model.Description;

        if (model.FileData is not null)
        {
            if (await _productService.IsProductInUse(process.ProductId))
                throw new Exception("Cannot change pattern: Product is in use!");

            process.PatternNo = model.FileData.FileName.ToUpper();

            await Utils.Utils.DeleteFile(process.PatternDirectory);

            var directory = $"{_settings.FileDataDirectory}{DateTime.UtcNow.ToString("yyyyMMddhhmmssfff")}-{process.PatternNo}";
            process.PatternDirectory = directory;

            await Utils.Utils.WriteFile(model.FileData, directory);
        }

        await _linq2Db.Update(process);

        return model;
    }

}