namespace esp32_client.Models;

public class MachineCreateModel
{
#nullable disable
    public string MachineName { get; set; }
    public string IpAddress { get; set; }
    public int DepartmentId { get; set; }
    public int LineId { get; set; }
    public int ProcessId { get; set; }
}

public class MachineUpdateModel
{
    public int Id { get; set; }
    public string MachineName { get; set; }
    public string IpAddress { get; set; }
    // public int DepartmentId { get; set; }
    // public int LineId { get; set; }
    // public int ProcessId { get; set; }
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
