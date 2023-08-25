using AutoMapper;
using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
using LinqToDB;

namespace esp32_client.Services;

public partial class UserRoleService : IUserRoleService
{
    private readonly LinqToDb _linq2db;
    private readonly IMapper _mapper;

    public UserRoleService(LinqToDb linq2Db, IMapper mapper)
    {
        _linq2db = linq2Db;
        _mapper = mapper;
    }

    public async Task<UserRole?> GetById(int id)
    {
        var userRole = await _linq2db.Entity<UserRole>().Where(s => s.Id == id).FirstOrDefaultAsync();
        return userRole;
    }

    public async Task<List<UserRole>> GetAll()
    {
        return await _linq2db.Entity<UserRole>().ToListAsync();
    }

    public async Task<UserRole> Create(UserRoleCreateModel model)
    {
        var userRole = new UserRole { RoleName = model.RoleName };

        userRole = await _linq2db.Insert(userRole);

        return userRole;
    }

    public async Task<UserRole> Update(UserRoleUpdateModel model)
    {
        var userRole = await GetById(model.Id);
        if (userRole is null) throw new Exception("UserRole is not found");

        userRole.RoleName = model.RoleName;
        await _linq2db.Update(userRole);

        return userRole;
    }

    public async Task Delete(int id)
    {
        var userRole = await GetById(id);
        if (userRole is not null)
            await _linq2db.Delete(userRole);
    }

}