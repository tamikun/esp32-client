using AutoMapper;
using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
using LinqToDB;

namespace esp32_client.Services;

/// <summary>
/// 
/// </summary>
public partial class RoleOfUserService : IRoleOfUserService
{

    private readonly LinqToDb _linq2Db;
    private readonly IMapper _mapper;

    public RoleOfUserService(LinqToDb linq2Db, IMapper mapper)
    {
        _linq2Db = linq2Db;
        _mapper = mapper;
    }

    public async Task<RoleOfUser?> GetById(int id)
    {
        var RoleOfUser = await _linq2Db.RoleOfUser.Where(s => s.Id == id).FirstOrDefaultAsync();
        return RoleOfUser;
    }

    public async Task<List<RoleOfUser>> GetAll()
    {
        return await _linq2Db.RoleOfUser.ToListAsync();
    }

    public async Task<RoleOfUser> Create(RoleOfUserCreateModel model)
    {
        var RoleOfUser = new RoleOfUser { UserId = model.UserId, RoleId = model.RoleId };

        await _linq2Db.InsertAsync(RoleOfUser);

        return RoleOfUser;
    }

    public async Task<RoleOfUser> Update(RoleOfUserUpdateModel model)
    {
        var RoleOfUser = await GetById(model.Id);
        if (RoleOfUser is null) throw new Exception("RoleOfUser is not found");
        
        RoleOfUser.RoleId = model.RoleId;
        RoleOfUser.UserId = model.UserId;
        
        await _linq2Db.UpdateAsync(RoleOfUser);

        return RoleOfUser;
    }

    public async Task Delete(int id)
    {
        var RoleOfUser = await GetById(id);
        if (RoleOfUser is not null)
            await _linq2Db.DeleteAsync(RoleOfUser);
    }

}