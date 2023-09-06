using esp32_client.Builder;
using esp32_client.Models;
using esp32_client.Services;

namespace test;

[TestFixture]
public class AssignTest
{
    private IMachineService _machineService;
    private ILineService _lineService;
    private IProductService _productService;
    private LinqToDb _linq2db;

    public AssignTest()
    {
        _machineService = BaseTest.GetService<IMachineService>();
        _linq2db = BaseTest.GetService<LinqToDb>();
        _lineService = BaseTest.GetService<ILineService>();
        _productService = BaseTest.GetService<IProductService>();
    }

    [SetUp]
    public void Setup()
    { }

# nullable disable

    [Test]
    public async Task ShouldAssignMachineLine()
    {
        await _lineService.Create(new LineCreateModel
        {
            FactoryId = 1,
            LineName = "line 1",
            LineNo = 1,
            NumberOfStation = 3,
        });

        await _productService.Create(new ProductCreateModel
        {
            FactoryId = 1,
            ProductName = "product 1",
            ProductNo = 1,
            NumberOfProcess = 3,
        });

        var m1 = await _machineService.Create(new MachineCreateModel
        {
            MachineName = "M1",
            MachineNo = 1,
            IpAddress = "192.168.1.101",
            FactoryId = 1,
        });
        var m2 = await _machineService.Create(new MachineCreateModel
        {
            MachineName = "M2",
            MachineNo = 1,
            IpAddress = "192.168.1.102",
            FactoryId = 1,
        });
        var m3 = await _machineService.Create(new MachineCreateModel
        {
            MachineName = "M3",
            MachineNo = 1,
            IpAddress = "192.168.1.103",
            FactoryId = 1,
        });

        await _machineService.AssignMachineLine(new ListAssignMachineLineModel
        {
            FactoryId = 1,
            LineId = 1,
            ListAssignMachine = new List<AssignMachineLineModel>(){
                new AssignMachineLineModel{MachineId=m1.Id,StationId=1},
                new AssignMachineLineModel{MachineId=m2.Id,StationId=2},
                new AssignMachineLineModel{MachineId=m3.Id,StationId=3},
            }
        });

        m1 = await _machineService.GetById(m1.Id);
        m2 = await _machineService.GetById(m2.Id);
        m3 = await _machineService.GetById(m3.Id);

        Assert.That(m1.StationId, Is.EqualTo(1));
        Assert.That(m2.StationId, Is.EqualTo(2));
        Assert.That(m3.StationId, Is.EqualTo(3));

        await _machineService.AssignMachineLine(new ListAssignMachineLineModel
        {
            FactoryId = 1,
            LineId = 1,
            ListAssignMachine = new List<AssignMachineLineModel>(){
                new AssignMachineLineModel{MachineId=m3.Id,StationId=1},
                new AssignMachineLineModel{MachineId=m2.Id,StationId=2},
                new AssignMachineLineModel{MachineId=0,StationId=3},
            }
        });

        m1 = await _machineService.GetById(m1.Id);
        m2 = await _machineService.GetById(m2.Id);
        m3 = await _machineService.GetById(m3.Id);

        Assert.That(m3.StationId, Is.EqualTo(1));
        Assert.That(m2.StationId, Is.EqualTo(2));
        Assert.That(m1.StationId, Is.EqualTo(0));
    }

}