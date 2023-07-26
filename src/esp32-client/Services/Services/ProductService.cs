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

    public ProductService(LinqToDb linq2Db, IMapper mapper)
    {
        _linq2Db = linq2Db;
        _mapper = mapper;
    }

    public async Task<Product?> GetById(int id)
    {
        var product = await _linq2Db.Product.Where(s => s.Id == id).FirstOrDefaultAsync();
        return product;
    }

    public async Task<Product?> GetByProductName(string name)
    {
        var product = await _linq2Db.Product.Where(s => s.ProductName == name).FirstOrDefaultAsync();
        return product;
    }

    public async Task<List<Product>> GetAll()
    {
        return await _linq2Db.Product.ToListAsync();
    }

    public async Task<ProductCreateModel> Create(ProductCreateModel model)
    {
        var product = new Product() { ProductName = model.ProductName };

        await _linq2Db.InsertAsync(product);

        return model;
    }

    public async Task<ProductUpdateModel> Update(ProductUpdateModel model)
    {
        var product = await GetById(model.Id);

        if (product is null) throw new Exception("Product is not found");

        product.ProductName = model.ProductName;

        await _linq2Db.Update(product);

        return model;
    }

    public async Task Delete(int id)
    {
        var product = await GetById(id);

        // Check produce is in use

        if (product is null) return;
        await _linq2Db.DeleteAsync(product);
    }
}