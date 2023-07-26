using AutoMapper;
using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
using LinqToDB;

namespace esp32_client.Services;

/// <summary>
/// 
/// </summary>
public partial class LineService : ILineService
{

    private readonly LinqToDb _linq2Db;
    private readonly IMapper _mapper;

    public LineService(LinqToDb linq2Db, IMapper mapper)
    {
        _linq2Db = linq2Db;
        _mapper = mapper;
    }

    public async Task<Line?> GetById(int id)
    {
        var line = await _linq2Db.Line.Where(s => s.Id == id).FirstOrDefaultAsync();
        return line;
    }

    public async Task<List<Line>> GetAll()
    {
        return await _linq2Db.Line.ToListAsync();
    }

    public async Task<List<LineResponseModel>> GetAllLineResponse(int departmentId)
    {
        var result = await (from line in _linq2Db.Line.Where(s => s.DepartmentId == departmentId)
                            join product1 in _linq2Db.Product on line.ProductId equals product1.Id into product2
                            from product in product2.DefaultIfEmpty()
                            select new LineResponseModel
                            {
                                Id = line.Id,
                                LineName = line.LineName,
                                ProductName = product.ProductName,
                            }).ToListAsync();

        return result;
    }

    public async Task<Line> Create(LineCreateModel model)
    {
        var line = new Line { DepartmentId = model.DepartmentId, LineName = model.LineName, Order = model.Order, ProductId = model.ProductId };

        await _linq2Db.InsertAsync(line);

        return line;
    }

    public async Task<Line> Update(LineUpdateModel model)
    {
        var line = await GetById(model.Id);
        if (line is null) throw new Exception("Line is not found");

        line.DepartmentId = model.DepartmentId;
        line.LineName = model.LineName;
        line.Order = model.Order;
        line.ProductId = model.ProductId;

        await _linq2Db.Update(line);

        return line;
    }

    public async Task Delete(int id)
    {
        var line = await GetById(id);
        if (line is not null)
            await _linq2Db.DeleteAsync(line);
    }

}