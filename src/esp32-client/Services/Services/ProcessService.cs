using AutoMapper;
using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
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

    public ProcessService(LinqToDb linq2Db, IMapper mapper, IProductService productService)
    {
        _linq2Db = linq2Db;
        _mapper = mapper;
        _productService = productService;
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

    public async Task<ProcessCreateModel> Create(ProcessCreateModel model)
    {
        var process = new Process()
        {
            ProductId = model.ProductId,
            ProcessNo = model.ProcessNo,
        };

        await _linq2Db.InsertAsync(process);

        return model;
    }

    public async Task<ProcessUpdateModel> Update(ProcessUpdateModel model)
    {
        var process = await GetById(model.Id);

        if (process is null) throw new Exception("Process is not found");

        process.ProductId = model.ProductId;
        process.ProcessName = model.ProcessName;
        // process.PatternId = model.PatternId;
        // process.Order = model.Order;

        await _linq2Db.Update(process);

        return model;
    }

    public async Task<ProcessAddRequestModel> Update(ProcessAddRequestModel model)
    {
        var product = await _productService.GetById(model.ProductId);

        if (product is null) throw new Exception("Product is not found");
        bool isProductInUse = await _productService.IsProductInUse(model.ProductId);

        if (product.ProductName != model.ProductName)
        {
            product.ProductName = model.ProductName;
            await _linq2Db.Update(product);
        }

        var processTable = _linq2Db.Process.Where(s => s.ProductId == model.ProductId);

        if (isProductInUse)
        {
            // Just update name, not delete insert or update Pattern
            var listProcessUpdate = await (from process in processTable
                                           join request in model.ListProcessCreate.Where(s => s.Id != 0) on process.Id equals request.Id
                                           where process.ProcessName != request.ProcessName
                                           // || process.PatternId != request.PatternId
                                           // || process.Order != request.Order
                                           select new Process
                                           {
                                               Id = process.Id,
                                               //    Order = request.Order,
                                               ProductId = process.ProductId,
                                               ProcessName = request.ProcessName,
                                               //    PatternId = process.PatternId,
                                           }).ToListAsync();

            foreach (var item in listProcessUpdate)
            {
                await _linq2Db.Update(item);
            }

        }
        else
        {
            var listProcessUpdate = await (from process in processTable
                                           join request in model.ListProcessCreate.Where(s => s.Id != 0) on process.Id equals request.Id
                                           where process.ProcessName != request.ProcessName
                                           // || process.PatternId != request.PatternId
                                           // || process.Order != request.Order
                                           select new Process
                                           {
                                               Id = process.Id,
                                               //    Order = request.Order,
                                               ProductId = process.ProductId,
                                               ProcessName = request.ProcessName,
                                               //    PatternId = request.PatternId,
                                           }).ToListAsync();

            foreach (var item in listProcessUpdate)
            {
                await _linq2Db.Update(item);
            }

            var listProcessDelete = await processTable.Where(s => !model.ListProcessCreate.Select(s => s.Id).Contains(s.Id)).ToListAsync();

            foreach (var item in listProcessDelete)
            {
                await Delete(item.Id);
            }

            var listProcessInsert = model.ListProcessCreate.Where(s => s.Id == 0).Select(s =>
            {
                return new Process
                {
                    ProductId = model.ProductId,
                    ProcessName = s.ProcessName,
                    // PatternId = s.PatternId,
                    // Order = s.Order,
                };
            });

            await _linq2Db.BulkInsert(listProcessInsert);
        }


        return model;
    }

    public async Task Delete(int id)
    {
        var process = await GetById(id);

        if (process is null) throw new Exception("Process is not found");

        if (await _productService.IsProductInUse(process.ProductId)) throw new Exception("Product is in use");

        await _linq2Db.DeleteAsync(process);
    }
}