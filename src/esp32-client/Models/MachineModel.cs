namespace esp32_client.Models;

public class MachineCreateModel
{
#nullable disable
    public string MachineName { get; set; }
    public int MachineNo { get; set; }
    public string IpAddress { get; set; }
    public int FactoryId { get; set; }
}

public class MachineUpdateModel
{
    public int MachineId { get; set; }
    public string MachineName { get; set; }
    public string IpAddress { get; set; }
    public int FactoryId { get; set; }
}

public class UpdateMachineLineModel
{
    public UpdateMachineLineModel()
    {
        ListProcessAndMachineOfLine = new List<GetProcessAndMachineOfLineModel>();
    }
    public int DepartmentId { get; set; }
    public List<GetProcessAndMachineOfLineModel> ListProcessAndMachineOfLine { get; set; }
}

public class AssignMachineLineModel
{
    public int MachineId { get; set; }
    public int StationId { get; set; }
}

public class ListAssignMachineLineModel
{
    public List<AssignMachineLineModel> ListAssignMachine { get; set; }
    public int FactoryId { get; set; }
    public int LineId { get; set; }
}

public class MachineResponseModel
{
    public int MachineId { get; set; }
    public string MachineName { get; set; }
    public string MachineNo { get; set; }
    public string IpAddress { get; set; }
    public string LineName { get; set; }
    public string ProcessName { get; set; }
    public string COPartNo { get; set; }
}
