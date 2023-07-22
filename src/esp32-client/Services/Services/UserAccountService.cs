using esp32_client.Builder;
using esp32_client.Models;
using LinqToDB;

namespace esp32_client.Services;

/// <summary>
/// 
/// </summary>
public partial class UserAccountService : IUserAccountService
{

    private readonly LinqToDb _linq2Db;

    public UserAccountService(LinqToDb linq2Db)
    {
        _linq2Db = linq2Db;
    }

    public async Task<bool> IsValidUser(string loginName, string password)
    {

        var user = await _linq2Db.UserAccount.Where(s => s.LoginName == loginName).FirstOrDefaultAsync();
        if (user is null) return false;
        if (user.Password != password) return false;

        return true;
    }

}