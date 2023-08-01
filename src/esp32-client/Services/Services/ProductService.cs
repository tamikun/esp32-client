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

    public async Task<Product?> GetByProductNo(string productNo)
    {
        var product = await _linq2Db.Product.Where(s => s.ProductNo == productNo).FirstOrDefaultAsync();
        return product;
    }

    public async Task<List<Product>> GetAll()
    {
        return await _linq2Db.Product.ToListAsync();
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
        product = await GetByProductNo(product.ProductNo) ?? new Product();

        // Create process
        var listProcess = new List<Process>();
        for (int i = 0; i < model.NumberOfProcess; i++)
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
                        ).ToListAsync();
        return data;
    }

    public async Task<ProductUpdateModel> Update(ProductUpdateModel model)
    {
        var product = await GetById(model.Id);

        if (product is null) throw new Exception("Product is not found");

        // Check produce is in use
        if (await IsProductInUse(model.Id)) throw new Exception("Product is in use");

        product.ProductName = model.ProductName;

        await _linq2Db.Update(product);

        return model;
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