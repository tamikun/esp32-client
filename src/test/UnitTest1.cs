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

        // var addInitData = new AddInitData(_linq2Db);
        // _runner.Up(addInitData);

        // var settings = new List<Setting>{
        //     new Setting{Id = 1, Name = "GetApiTimeOut", Value = "1000"},
        //     new Setting{Id = 2, Name = "PostFileTimeOut", Value = "1000"},
        // };

        // _linq2Db.BulkInsert(settings).Wait();
    }

    [Test]
    public async Task Test1()
    {
        System.Console.WriteLine("==== _setting: " + Newtonsoft.Json.JsonConvert.SerializeObject(await _linq2Db.Setting.ToListAsync()));
        Assert.Pass();
    }
}