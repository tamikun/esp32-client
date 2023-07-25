using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
using LinqToDB;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

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
        var product = await GetByProductName(model.ProductName);

        if (product is null) return model;

        product.ProductName = model.NewProductName;

        await _linq2Db.UpdateAsync(product);

        return model;
    }

    public async Task Delete(int id)
    {
        var product = await GetById(id);

        if (product is null) return;
        await _linq2Db.DeleteAsync(product);
    }
}