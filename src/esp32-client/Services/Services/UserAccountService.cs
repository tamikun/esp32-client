using System.Security.Cryptography;
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
public partial class UserAccountService : IUserAccountService
{

    private readonly LinqToDb _linq2Db;
    private readonly IMapper _mapper;
    private readonly IRoleOfUserService _roleOfUser;

    public UserAccountService(LinqToDb linq2Db, IMapper mapper, IRoleOfUserService roleOfUser)
    {
        _linq2Db = linq2Db;
        _mapper = mapper;
        _roleOfUser = roleOfUser;
    }

    public async Task<UserAccount?> GetById(int id)
    {
        var user = await _linq2Db.UserAccount.Where(s => s.Id == id).FirstOrDefaultAsync();
        return user;
    }

    public async Task<UserAccount?> GetByLoginName(string loginName)
    {
        var user = await _linq2Db.UserAccount.Where(s => s.LoginName == loginName).FirstOrDefaultAsync();
        return user;
    }

    public async Task<bool> IsValidUser(UserAccount userAcount, string password)
    {

        return await VerifyPassword(userAcount, password);
    }

    public async Task<UserAccountCreateModel> Create(UserAccountCreateModel model)
    {
        if (await _linq2Db.UserAccount.AnyAsync(s => s.LoginName == model.LoginName))
            throw new Exception("Login name has aldready existed");

        var userAccount = _mapper.Map<UserAccount>(model);

        string salf = await CreateSalfKey();

        userAccount.SalfKey = salf;
        userAccount.Password = await HashPassword(userAccount.Password, salf);

        await _linq2Db.InsertAsync(userAccount);

        userAccount = await GetByLoginName(model.LoginName);

        if (userAccount is not null)
        {
            await _roleOfUser.Create(new RoleOfUserCreateModel
            {
                UserId = userAccount.Id,
                RoleId = model.RoleId,
            });
        }

        return model;
    }

    public async Task<UserAccountUpdateModel> Update(UserAccountUpdateModel model)
    {
        var user = await GetById(model.Id);

        if (user is null) throw new Exception("User is not found");

        user.LoginName = model.LoginName;
        user.UserName = model.UserName;

        await _linq2Db.Update(user);

        return model;
    }

    public async Task<UserAccountChangePasswordModel> ChangePassword(UserAccountChangePasswordModel model)
    {
        var user = await GetById(model.Id);

        if (user is null) throw new Exception("User is not found");

        var hash = await HashPassword(model.OldPassword, user.SalfKey);

        if (hash != user.Password) throw new Exception("Incorrect password");

        if (model.NewPassword != model.ConfirmedPassword) throw new Exception("Confirmed password does not match password");

        user.Password = await HashPassword(model.NewPassword, user.SalfKey);

        await _linq2Db.Update(user);

        return model;
    }

    public async Task Delete(int id)
    {
        var user = await GetById(id);

        if (user is null) return;
        await _linq2Db.DeleteAsync(user);
    }

    public async Task<bool> CheckUserRight(string? loginName, string? controllerName, string? actionName)
    {
        if (loginName is null) return false;
        if (controllerName is null) return true;
        if (actionName is null) return true;

        var userRights = await GetUserRight(loginName);

        if (userRights.Count == 0) return false;

        // Full rights
        if (userRights.Any(s => s.ControllerName == "*" && s.ActionName == "*")) return true;
        // Have a right
        if (userRights.Any(s => s.ControllerName == controllerName && s.ActionName == actionName)) return true;

        return false;
    }

    public async Task<List<UserRight>> GetUserRight(string? loginName)
    {
        if (loginName is null) return new List<UserRight>();

        var userRights = await (from user in _linq2Db.UserAccount.Where(s => s.LoginName == loginName)
                                join roleOfUser in _linq2Db.RoleOfUser on user.Id equals roleOfUser.UserId into roles
                                from role in roles
                                join userRight in _linq2Db.UserRight on role.RoleId equals userRight.RoleId
                                select userRight).ToListAsync();

        return userRights;
    }

    // Hash a password with a randomly generated salt
    private async Task<string> HashPassword(string password, string salt)
    {
        // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password!,
            salt: Convert.FromBase64String(salt),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

        await Task.CompletedTask;
        return hashed;
    }

    private async Task<string> CreateSalfKey()
    {
        byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
        await Task.CompletedTask;
        return Convert.ToBase64String(salt);
    }

    private async Task<bool> VerifyPassword(UserAccount userAcount, string password)
    {
        var hash = await HashPassword(password, userAcount.SalfKey);
        return hash == userAcount.Password;
    }

}