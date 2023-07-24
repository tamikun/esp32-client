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

    public async Task<List<string>> GetAllProduct()
    {
        return await _linq2Db.Product.Select(s => s.ProductName).Distinct().ToListAsync();
    }
    
    public async Task<List<Product>> GetProductDetail(string productName)
    {
        return await _linq2Db.Product.Where(s => s.ProductName == productName).OrderBy(s => s.Order).ToListAsync();
    }

    public async Task<ProductCreateModel> Create(ProductCreateModel model)
    {
        var listData = new List<Product>();
        for (int i = 0; i < model.ListProcessPattern.Count; i++)
        {
            listData.Add(new Product { ProductName = model.ProductName, ProcessName = model.ListProcessPattern[i].ProcessName, PatternNumber = model.ListProcessPattern[i].PatternNumber, Order = i });
        }

        await _linq2Db.BulkInsert(listData);

        return model;
    }


}