using System.Reflection;
using esp32_client.Builder;
using esp32_client.Controllers;
using esp32_client.Domain;
using esp32_client.Models;
using esp32_client.Models.Singleton;
using esp32_client.Services;
using FluentMigrator.Runner;

namespace test;

[TestFixture]
public class SettingTest
{
    private Settings _setting;
    private IMachineService _machineService;
    private ILineService _lineService;
    private LinqToDb _linq2db;
    public SettingTest()
    {
        _setting = BaseTest.GetService<Settings>();
        _machineService = BaseTest.GetService<IMachineService>();
        _linq2db = BaseTest.GetService<LinqToDb>();
        _lineService = BaseTest.GetService<ILineService>();
    }

    [SetUp]
    public void Setup()
    { }

    [Test]
    public async Task ShouldAddNewMachine()
    {
        await BaseTest.Truncate<Machine>();

        var newMachine = await _machineService.Create(new MachineCreateModel
        {
            MachineName = "Default new machine",
            MachineNo = 1,
            FactoryId = 1,
            CncMachine = true,
        });

        newMachine = await _machineService.GetById(newMachine.Id);

        Assert.That(newMachine.IpAddress, Is.EqualTo(_setting.DefaultNewMachineIp));
    }
}