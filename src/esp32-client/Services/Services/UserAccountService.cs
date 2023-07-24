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
public partial class UserAccountService : IUserAccountService
{

    private readonly LinqToDb _linq2Db;
    private readonly IMapper _mapper;

    public UserAccountService(LinqToDb linq2Db, IMapper mapper)
    {
        _linq2Db = linq2Db;
        _mapper = mapper;
    }

    public async Task<bool> IsValidUser(string loginName, string password)
    {

        var user = await _linq2Db.UserAccount.Where(s => s.LoginName == loginName).FirstOrDefaultAsync();
        if (user is null) return false;

        return await VerifyPassword(loginName, password, user.SalfKey);
    }

    public async Task<UserAccountCreateModel> Create(UserAccountCreateModel model)
    {
        var userAccount = _mapper.Map<UserAccount>(model);

        string salf = await CreateSalfKey();

        userAccount.SalfKey = salf;
        userAccount.Password = await HashPassword(userAccount.Password, salf);

        await _linq2Db.InsertAsync(userAccount);

        return model;
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

    private async Task<bool> VerifyPassword(string loginName, string password, string salt)
    {
        var user = await _linq2Db.UserAccount.Where(s => s.LoginName == loginName).FirstOrDefaultAsync();

        System.Console.WriteLine("==== user: " + Newtonsoft.Json.JsonConvert.SerializeObject(user));

        if (user is null) return false;

        var hash = await HashPassword(password, salt);

        System.Console.WriteLine("==== hash: " + Newtonsoft.Json.JsonConvert.SerializeObject(hash));

        return hash == user.Password;
    }

}