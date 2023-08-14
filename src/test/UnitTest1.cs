using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Services;
using FluentMigrator.Runner;
using LinqToDB;

namespace test;

[TestFixture]
public class Tests
{
#nullable disable
    private Settings _setting;
    private IMigrationRunner _runner;
    private LinqToDb _linq2Db;
    [SetUp]
    public void Setup()
    {
        _setting = BaseTest.GetService<Settings>();
        _runner = BaseTest.GetService<IMigrationRunner>();
        _linq2Db = BaseTest.GetService<LinqToDb>();

        var settings = new List<Setting>{
            new Setting{Id = 1, Name = "GetApiTimeOut", Value = "1000"},
            new Setting{Id = 2, Name = "PostFileTimeOut", Value = "1000"},
        };

        _linq2Db.BulkInsert(settings).Wait();

        var department = new List<Factory>{
            new Factory { FactoryName = "Juki", FactoryNo = "Factory 001"},
        };
        _linq2Db.BulkInsert(department).Wait();

        var userAccount = new List<UserAccount>{
            new UserAccount { LoginName = "admin", Password = "MM08+DTe5SgJ/abCwZW2oRlH+g8mO1XyQxStxGwTetI=", SalfKey = "YSguanX0gfFpM9t6Cn711Q==", UserName = "QuanTM" },
        };
        _linq2Db.BulkInsert(userAccount).Wait();

        var userRole = new List<UserRole>{
            new UserRole{RoleName = "Administrator"},
            new UserRole{RoleName = "Operator"},
            new UserRole{RoleName = "View"},
        };
        _linq2Db.BulkInsert(userRole).Wait();

        var roleOfUser = new List<RoleOfUser>{
            new RoleOfUser{UserId = 1, RoleId = 1},
            new RoleOfUser{UserId = 1, RoleId = 2},
            new RoleOfUser{UserId = 1, RoleId = 3},
        };
        _linq2Db.BulkInsert(roleOfUser).Wait();

        var userRight = new List<UserRight>{
            new UserRight{RoleId = 1, ControllerName = "*", ActionName = "*"},
            new UserRight{RoleId = 2, ControllerName = "*", ActionName = "Index"},
            new UserRight{RoleId = 3, ControllerName = "*", ActionName = "Index"},
        };
        _linq2Db.BulkInsert(userRight).Wait();
    }

    [Test]
    public async Task Test1()
    {
        System.Console.WriteLine("==== Setting: " + Newtonsoft.Json.JsonConvert.SerializeObject(await _linq2Db.Setting.ToListAsync()));
        System.Console.WriteLine("==== UserAccount: " + Newtonsoft.Json.JsonConvert.SerializeObject(await _linq2Db.UserAccount.ToListAsync()));
        System.Console.WriteLine("==== Factory: " + Newtonsoft.Json.JsonConvert.SerializeObject(await _linq2Db.Factory.ToListAsync()));
        Assert.Pass();
    }
}