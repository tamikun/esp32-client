using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
using esp32_client.Models.Singleton;
using esp32_client.Services;
using FluentMigrator.Runner;
using LinqToDB;

namespace test;

[TestFixture]
public class ServiceTest
{
    private Settings _setting;
    private ISettingService _settingService;
    private IMigrationRunner _runner;
    private IMachineService _machineService;
    private ILineService _lineService;
    private LinqToDb _linq2db;

    public ServiceTest()
    {
        _setting = BaseTest.GetService<Settings>();
        _settingService = BaseTest.GetService<ISettingService>();
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

        await _linq2db.Entity<Machine>().AsQueryable().DeleteQuery();
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

        await _linq2db.Entity<Factory>().AsQueryable().DeleteQuery();
    }

    [Test]
    [Order(3)]
    public async Task ShouldCreateLine()
    {
        await BaseTest.Truncate<Line>();
        await BaseTest.Truncate<Station>();

        var model = new LineCreateModel
        {
            FactoryId = 1,
            LineName = "line 1",
            LineNo = 1,
            NumberOfStation = 2,
        };

        var line = await _lineService.Create(model);

        Assert.That(line.Id, Is.EqualTo(1)); // Insert new Line success, return LineId = 1

        var stations = await _linq2db.Entity<Station>().Where(s => s.LineId == 1).ToListAsync(); // Get Station of LineId 1
        Assert.That(stations.Count, Is.EqualTo(2)); // Insert 2 Stations sucess

        await _linq2db.Entity<Line>().AsQueryable().DeleteQuery();
        await _linq2db.Entity<Station>().AsQueryable().DeleteQuery();
    }

    [Test]
    [Order(3)]
    public async Task ShouldGetSettings()
    {
        Assert.That(_setting.DeleteOnUploadingEmptyFile, Is.EqualTo(false));
        await Task.CompletedTask;
    }

    [Test]
    [Order(4)]
    public async Task ShouldGetSettingPagedList()
    {
        var result = await _linq2db.Entity<Setting>().ToPagedListModel(0, 5);
        Assert.That(result.Data.Count, Is.EqualTo(5));
    }

    [Test]
    [Order(5)]
    public async Task ShouldUpdateListSetting()
    {
        Assert.That(_setting.GetApiTimeOut, Is.EqualTo(1000));
        Assert.That(_setting.PostFileTimeOut, Is.EqualTo(1000));

        var listUpdate = new List<Setting>{
            new Setting{Id = 1, Name = "GetApiTimeOut", Value = "2000"},
            new Setting{Id = 2, Name = "PostFileTimeOut", Value = "3000"},
        };

        await _settingService.UpdateListSetting(listUpdate);
        Assert.That(_setting.GetApiTimeOut, Is.EqualTo(2000));
        Assert.That(_setting.PostFileTimeOut, Is.EqualTo(3000));
    }

    [Test]
    [Order(6)]
    public async Task ShouldSimpleSearchMonitoring()
    {
        // Prepare data
        await _linq2db.Insert(new Factory { FactoryNo = "Factory 1", FactoryName = "Factory 1" });

        await _lineService.Create(new LineCreateModel { FactoryId = 1, LineName = "line 1", LineNo = 1, NumberOfStation = 3, });

        await _lineService.Create(new LineCreateModel { FactoryId = 1, LineName = "line 2", LineNo = 1, NumberOfStation = 3, });

        var machine1 = await _machineService.Create(new MachineCreateModel { FactoryId = 1, IoTMachine = true, IpAddress = "1", MachineNo = 1, MachineName = "Machine1" });
        var machine2 = await _machineService.Create(new MachineCreateModel { FactoryId = 1, IoTMachine = true, IpAddress = "2", MachineNo = 2, MachineName = "Machine2" });
        var machine3 = await _machineService.Create(new MachineCreateModel { FactoryId = 1, IoTMachine = true, IpAddress = "3", MachineNo = 3, MachineName = "Machine3" });

        machine1.StationId = 1;
        machine1.LineId = 1;
        await _linq2db.Update(machine1);

        // Simple Search
        string searchText = "Machine1";

        int factoryId = 1;
        var listLineId = new List<int>() { };

        var lineQuery = _linq2db.Entity<Line>().Where(s => s.FactoryId == factoryId);
        if (listLineId.Count > 0) lineQuery = lineQuery.Where(s => listLineId.Contains(s.Id));

        var result = await (from line in lineQuery
                            join station in _linq2db.Entity<Station>() on line.Id equals station.LineId
                            join product1 in _linq2db.Entity<Product>() on line.ProductId equals product1.Id into product2
                            from product in product2.DefaultIfEmpty()
                            join process1 in _linq2db.Entity<Process>() on station.ProcessId equals process1.Id into process2
                            from process in process2.DefaultIfEmpty()
                            from machine in _linq2db.Entity<Machine>().Where(s =>
                                s.FactoryId == factoryId
                                && s.LineId == line.Id
                                && s.StationId == station.Id
                            ).DefaultIfEmpty()
                            where line.LineName.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                            || line.LineNo.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                            || product.ProductName.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                            || product.ProductNo.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                            || process.ProcessName.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                            || process.ProcessNo.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                            || machine.MachineName.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                            || station.StationNo.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                            select new GetProcessAndMachineOfLineModel
                            {
                                LineId = line.Id,
                                LineName = line.LineName,
                                LineNo = line.LineNo,
                                StationId = station.Id,
                                StationName = station.StationName,
                                StationNo = station.StationNo,
                                ProductName = product.ProductName,
                                ProductNo = product.ProductNo,
                                ProcessName = process.ProcessName,
                                ProcessNo = process.ProcessNo,
                                PatternNo = process.PatternNo,
                                PatterDescription = process.Description,
                                MachineId = machine.Id,
                                MachineName = machine.MachineName,
                                MachineNo = machine.MachineNo,
                                IoTMachine = machine.IoTMachine,
                            }).OrderBy(s => s.LineId).ThenBy(s => s.StationId).ToListAsync();

        var listLine = result.Select(s => s.LineId).Distinct();

        await _linq2db.Entity<Machine>().AsQueryable().DeleteQuery();
    }

    [Test]
    [Order(6)]
    public async Task ShouldAdvancedSearchMonitoring()
    {
        // Prepare data
        await _linq2db.Insert(new Factory { FactoryNo = "Factory 1.0", FactoryName = "Factory 1" });

        await _lineService.Create(new LineCreateModel { FactoryId = 1, LineName = "line 1", LineNo = 1, NumberOfStation = 3, });

        await _lineService.Create(new LineCreateModel { FactoryId = 1, LineName = "line 2", LineNo = 1, NumberOfStation = 3, });

        var machine1 = await _machineService.Create(new MachineCreateModel { FactoryId = 1, IoTMachine = true, IpAddress = "1", MachineNo = 1, MachineName = "Machine1" });
        var machine2 = await _machineService.Create(new MachineCreateModel { FactoryId = 1, IoTMachine = false, IpAddress = "2", MachineNo = 2, MachineName = "Machine2" });
        var machine3 = await _machineService.Create(new MachineCreateModel { FactoryId = 1, IoTMachine = true, IpAddress = "3", MachineNo = 3, MachineName = "Machine3" });

        machine1.StationId = 1;
        machine1.LineId = 1;
        await _linq2db.Update(machine1);

        // machine2.StationId = 4;
        // machine2.LineId = 2;
        // await _linq2db.Update(machine2);

        // Advanced Search
        bool iotMachine = false;
        bool hasProduct = false;
        bool hasMachine = false;

        int factoryId = 1;
        var listLineId = new List<int>() { 1, 2 };

        var data = await _lineService.GetProcessAndMachineOfLine(factoryId, listLineId, iotMachine, hasProduct, hasMachine);

        await _linq2db.Entity<Machine>().AsQueryable().DeleteQuery();
    }

}