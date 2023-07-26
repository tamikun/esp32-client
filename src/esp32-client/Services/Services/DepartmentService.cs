using AutoMapper;
using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
using LinqToDB;

namespace esp32_client.Services;

/// <summary>
/// 
/// </summary>
public partial class DepartmentService : IDepartmentService
{

    private readonly LinqToDb _linq2Db;
    private readonly IMapper _mapper;

    public DepartmentService(LinqToDb linq2Db, IMapper mapper)
    {
        _linq2Db = linq2Db;
        _mapper = mapper;
    }

    public async Task<Department?> GetById(int id)
    {
        var department = await _linq2Db.Department.Where(s => s.Id == id).FirstOrDefaultAsync();
        return department;
    }

    public async Task<List<Department>> GetAll()
    {
        return await _linq2Db.Department.ToListAsync();
    }

    public async Task<Department> Create(DepartmentCreateModel model)
    {
        var department = new Department { DepartmentName = model.DepartmentName };

        await _linq2Db.InsertAsync(department);

        return department;
    }

    public async Task<Department> Update(DepartmentUpdateModel model)
    {
        var department = await GetById(model.Id);
        if (department is null) throw new Exception("Department is not found");
        
        department.DepartmentName = model.DepartmentName;
        await _linq2Db.Update(department);

        return department;
    }

    public async Task Delete(int id)
    {
        var department = await GetById(id);
        if (department is not null)
            await _linq2Db.DeleteAsync(department);
    }

}