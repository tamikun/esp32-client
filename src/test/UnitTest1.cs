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
#nullable disable
    private Settings _setting;
    private IMigrationRunner _runner;
    private IMachineService _machineService;
    [SetUp]
    public void Setup()
    {
        _setting = BaseTest.GetService<Settings>();
        _runner = BaseTest.GetService<IMigrationRunner>();
        _machineService = BaseTest.GetService<IMachineService>();
    }

    [Test]
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
}