using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
using esp32_client.Services;
using FluentMigrator.Runner;
using LinqToDB;

namespace test;

[TestFixture]
public class Tests
{
    private Settings _setting;
    private IMigrationRunner _runner;
    private IMachineService _machineService;
    private ILineService _lineService;
    private LinqToDb _linq2db;

    public Tests()
    {
        _setting = BaseTest.GetService<Settings>();
        _runner = BaseTest.GetService<IMigrationRunner>();
        _machineService = BaseTest.GetService<IMachineService>();
        _linq2db = BaseTest.GetService<LinqToDb>();
        _lineService = BaseTest.GetService<ILineService>();
    }

    [SetUp]
    public void Setup()
    { }

# nullable disable

    [Test]
    [Order(1)]
    public async Task ShouldUpdateMachine()
    {
        var machineAdd = new MachineCreateModel
        {
            MachineName = "Machine name 1",
            IpAddress = "0",
            FactoryId = 0,
            MachineNo = 1,
        };

        Exception ex = Assert.ThrowsAsync<Exception>(
           async () => await _machineService.Create(machineAdd));

        Assert.That(ex.Message, Is.EqualTo("Invalid factory"));

        machineAdd.FactoryId = 1;

        await _machineService.Create(machineAdd);

        var model = new MachineUpdateModel
        {
            MachineId = 1,
            MachineName = "Machine update",
            IpAddress = "1",
        };

        await _machineService.Update(model);

        var machineUpdate = await _machineService.GetById(1);

        Assert.That(machineUpdate.MachineName, Is.EqualTo(model.MachineName));
    }

    [Test]
    [Order(2)]
    public async Task ShouldInsertWithIdentity()
    {
        var fact = new Factory
        {
            FactoryNo = "Factory No.2",
            FactoryName = "Factory name",
        };
        var fact2 = await _linq2db.Insert(fact);
        Assert.That(fact2.Id, Is.EqualTo(2));

        fact.FactoryNo = "Factory No.3";

        var fact3 = await _linq2db.Insert(fact);
        Assert.That(fact3.Id, Is.EqualTo(3));

        await _linq2db.Delete(fact3);

        var fact4 = await _linq2db.Insert(fact);
        Assert.That(fact4.Id, Is.EqualTo(4));

    }

    [Test]
    [Order(3)]
    public async Task ShouldCreateLine()
    {
        var model = new LineCreateModel
        {
            FactoryId = 1,
            LineName = "line 1",
            LineNo = 1,
            NumberOfStation = 2,
        };

        var line = await _lineService.Create(model);

        Assert.That(line.Id, Is.EqualTo(1));

        var stations = await _linq2db.Station.Where(s => s.LineId == 1).ToListAsync();
        Assert.That(stations.Count, Is.EqualTo(2));
    }

    [Test]
    [Order(3)]
    public async Task ShouldGetSettings()
    {
        Assert.That(_setting.DeleteOnUploadingEmptyFile, Is.EqualTo(true));
        await Task.CompletedTask;
    }

}