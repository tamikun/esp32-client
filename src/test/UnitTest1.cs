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
    }

    [Test]
    public async Task Test1()
    {
        System.Console.WriteLine("==== Setting: " + Newtonsoft.Json.JsonConvert.SerializeObject(await _linq2Db.Setting.ToListAsync()));
        System.Console.WriteLine("==== UserAccount: " + Newtonsoft.Json.JsonConvert.SerializeObject(await _linq2Db.UserAccount.ToListAsync()));
        System.Console.WriteLine("==== Factory: " + Newtonsoft.Json.JsonConvert.SerializeObject(await _linq2Db.Factory.ToListAsync()));
        System.Console.WriteLine("==== userRole: " + Newtonsoft.Json.JsonConvert.SerializeObject(await _linq2Db.UserRole.ToListAsync()));
        Assert.Pass();
    }
}