using AutoMapper;
using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
using LinqToDB;

namespace esp32_client.Services;

/// <summary>
/// 
/// </summary>
public partial class UserRightService : IUserRightService
{

    private readonly LinqToDb _linq2Db;
    private readonly IMapper _mapper;

    public UserRightService(LinqToDb linq2Db, IMapper mapper)
    {
        _linq2Db = linq2Db;
        _mapper = mapper;
    }

    public async Task<UserRight?> GetById(int id)
    {
        var UserRight = await _linq2Db.UserRight.Where(s => s.Id == id).FirstOrDefaultAsync();
        return UserRight;
    }

    public async Task<List<UserRight>> GetAll()
    {
        return await _linq2Db.UserRight.ToListAsync();
    }

    public async Task<UserRight> Create(UserRightCreateModel model)
    {
        var UserRight = new UserRight
        {
            RoleId = model.RoleId,
            ControllerName = model.ControllerName,
            ActionName = model.ActionName,
        };

        await _linq2Db.InsertAsync(UserRight);

        return UserRight;
    }

    public async Task<UserRight> Update(UserRightUpdateModel model)
    {
        var UserRight = await GetById(model.Id);
        if (UserRight is null) throw new Exception("UserRight is not found");

        UserRight.RoleId = model.RoleId;
        UserRight.ControllerName = model.ControllerName;
        UserRight.ActionName = model.ActionName;

        await _linq2Db.Update(UserRight);

        return UserRight;
    }

    public async Task Delete(int id)
    {
        var UserRight = await GetById(id);
        if (UserRight is not null)
            await _linq2Db.DeleteAsync(UserRight);
    }

}