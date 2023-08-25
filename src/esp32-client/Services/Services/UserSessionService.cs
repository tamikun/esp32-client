using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
using esp32_client.Models.Singleton;
using LinqToDB;

namespace esp32_client.Services;

/// <summary>
/// 
/// </summary>
public partial class UserSessionService : IUserSessionService
{

    private readonly LinqToDb _linq2db;
    private readonly Settings _settings;

    public UserSessionService(LinqToDb linq2Db, Settings settings)
    {
        _linq2db = linq2Db;
        _settings = settings;
    }

    public async Task<UserSession?> GetById(int id)
    {
        var userSession = await _linq2db.Entity<UserSession>().Where(s => s.Id == id).FirstOrDefaultAsync();
        return userSession;
    }

    public async Task<bool> CheckToken(string token)
    {
        return await _linq2db.Entity<UserSession>().AnyAsync(s => s.Token == token);
    }

    public async Task<List<UserSession>> GetAll()
    {
        return await _linq2db.Entity<UserSession>().ToListAsync();
    }

    public async Task<List<UserSession>> GetByUserId(int userId)
    {
        return await _linq2db.Entity<UserSession>().Where(s => s.UserId == userId).ToListAsync();
    }

    public async Task<UserSession> Create(UserSessionCreateModel model)
    {
        var userSession = new UserSession
        {
            UserId = model.UserId,
            Token = model.Token,
            ExpiredTime = DateTime.Now.AddSeconds(_settings.SessionExpiredTimeInSecond)
        };

        userSession = await _linq2db.Insert(userSession);

        return userSession;
    }

    public async Task Delete(string token)
    {
        await _linq2db.Entity<UserSession>().Where(s => s.Token == token).DeleteQuery();
    }

    public async Task Delete(int id)
    {
        await _linq2db.Entity<UserSession>().Where(s => s.Id == id).DeleteQuery();
    }

    public async Task DeleteAll(int userId, string token)
    {
        await _linq2db.Entity<UserSession>().Where(s => s.UserId == userId && s.Token != token).DeleteQuery();
    }

}