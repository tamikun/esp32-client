using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
using LinqToDB;

namespace esp32_client.Services;

/// <summary>
/// 
/// </summary>
public partial class UserSessionService : IUserSessionService
{

    private readonly LinqToDb _linq2Db;

    public UserSessionService(LinqToDb linq2Db)
    {
        _linq2Db = linq2Db;
    }

    public async Task<UserSession?> GetById(int id)
    {
        var userSession = await _linq2Db.UserSession.Where(s => s.Id == id).FirstOrDefaultAsync();
        return userSession;
    }

    public async Task<bool> CheckToken(string token)
    {
        return await _linq2Db.UserSession.AnyAsync(s => s.Token == token);
    }

    public async Task<List<UserSession>> GetAll()
    {
        return await _linq2Db.UserSession.ToListAsync();
    }

    public async Task<UserSession> Create(UserSessionCreateModel model)
    {
        var userSession = new UserSession
        {
            UserId = model.UserId,
            Token = model.Token,
        };

        userSession = await _linq2Db.Insert(userSession);

        return userSession;
    }

    public async Task Delete(string token)
    {
        await _linq2Db.UserSession.Where(s => s.Token == token).DeleteQuery();
    }

}