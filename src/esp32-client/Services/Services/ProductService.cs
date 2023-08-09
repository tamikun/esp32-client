using AutoMapper;
using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
using LinqToDB;

namespace esp32_client.Services;

/// <summary>
/// 
/// </summary>
public partial class ProductService : IProductService
{

    private readonly LinqToDb _linq2Db;
    private readonly IMapper _mapper;
    private readonly Settings _settings;

    public ProductService(LinqToDb linq2Db, IMapper mapper, Settings settings)
    {
        _linq2Db = linq2Db;
        _mapper = mapper;
        _settings = settings;
    }

    public async Task<Product?> GetById(int id)
    {
        var product = await _linq2Db.Product.Where(s => s.Id == id).FirstOrDefaultAsync();
        return product;
    }

    public async Task<Product?> GetByProductNo(string productNo, int factoryId)
    {
        var product = await _linq2Db.Product.Where(s => s.ProductNo == productNo && s.FactoryId ==factoryId).FirstOrDefaultAsync();
        return product;
    }

    public async Task<List<Product>> GetAll(int factoryId)
    {
        return await _linq2Db.Product.Where(s => s.FactoryId == factoryId).ToListAsync();
    }

    public async Task<ProductCreateModel> Create(ProductCreateModel model)
    {
        if (model.FactoryId == 0) throw new Exception("Invalid factory");

        var product = new Product();
        product.ProductName = model.ProductName;
        product.FactoryId = model.FactoryId;

        string formattedNumber = model.ProductNo.ToString($"D{_settings.MinCharProductFormat}");
        product.ProductNo = string.Format(_settings.ProductFormat, formattedNumber);

        await _linq2Db.InsertAsync(product);

        // Get product id
        product = await GetByProductNo(product.ProductNo, model.FactoryId) ?? new Product();

        // Create process
        var listProcess = new List<Process>();
        for (int i = 1; i < model.NumberOfProcess + 1; i++)
        {
            listProcess.Add(new Process
            {
                ProductId = product.Id,
                ProcessNo = $"{product.ProductNo}.{i}",
            });
        }
        await _linq2Db.BulkInsert(listProcess);

        return model;
    }

    public async Task UpdateNameAndProcess(ProductUpdateModel model)
    {
        var product = await GetById(model.ProductId);
        if (product is null) throw new Exception("Product is not found");

        product.ProductName = model.ProductName;

        var listProcess = await _linq2Db.Process.Where(s => s.ProductId == product.Id).OrderBy(s => s.Id).ToListAsync();

        if (model.NumberOfProcess != listProcess.Count)
        {
            if (await IsProductInUse(product.Id))
            {
                throw new Exception("Can not change process: Product is in use");
            }
            if (model.NumberOfProcess > listProcess.Count)
            {
                var listNewProcess = new List<Process>();
                for (int i = listProcess.Count + 1; i < model.NumberOfProcess - listProcess.Count + 1; i++)
                {
                    listProcess.Add(new Process
                    {
                        ProductId = product.Id,
                        ProcessNo = $"{product.ProductNo}.{i}",
                    });
                }
                await _linq2Db.BulkInsert(listProcess);
            }
            else
            {
                var listDeleteProcess = listProcess.TakeLast(listProcess.Count - model.NumberOfProcess);
                foreach (var item in listDeleteProcess)
                {
                    await _linq2Db.DeleteAsync(item);
                }
            }
        }
        await _linq2Db.Update(product);
    }

    public async Task<List<ProductResponseModel>> GetProductByFactoryId(int factoryId)
    {
        var data = await (from factory in _linq2Db.Factory.Where(s => s.Id == factoryId)
                          from product in _linq2Db.Product.Where(s => s.FactoryId == factoryId)
                          select new ProductResponseModel
                          {
                              FactoryId = factoryId,
                              FactoryName = factory.FactoryName,
                              ProductId = product.Id,
                              ProductName = product.ProductName,
                              ProductNo = product.ProductNo,
                              NumberOfProcess = _linq2Db.Process.Where(s => s.ProductId == product.Id).Count(),
                          }
                        ).OrderBy(s => s.ProductNo).ToListAsync();
        return data;
    }

    public async Task<bool> IsProductInUse(int productId)
    {
        return await _linq2Db.Line.AnyAsync(s => s.ProductId == productId);
    }

    public async Task Delete(int id)
    {
        var product = await GetById(id);

        if (product is null) throw new Exception("Product is not found");

        // Check produce is in use
        if (await IsProductInUse(id)) throw new Exception("Product is in use");

        await _linq2Db.DeleteAsync(product);
    }
}