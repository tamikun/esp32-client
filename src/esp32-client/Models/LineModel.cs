namespace esp32_client.Models;
public class LineCreateModel
{
#nullable disable
    public int FactoryId { get; set; }
    public string LineName { get; set; }
    public int LineNo { get; set; }
    public int NumberOfStation { get; set; }
}

public class LineUpdateModel
{
    public int Id { get; set; }
    public int DepartmentId { get; set; }
    public string LineName { get; set; }
    public int Order { get; set; }
    public int ProductId { get; set; }
}

public class LineResponseModel
{
    public int FactoryId { get; set; }
    public string FactoryName { get; set; }
    public int LineId { get; set; }
    public string LineName { get; set; }
    public string LineNo { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int NumberOfStation { get; set; }
}

public class GetProcessAndMachineOfLineModel
{
    public int LineId { get; set; }
    public int LineOrder { get; set; }
    public string LineName { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int ProcessId { get; set; }
    public int ProcessOrder { get; set; }
    public string ProcessName { get; set; }
    public int MachineId { get; set; }
    public string MachineName { get; set; }
    public string MachineIp { get; set; }
    public int PatternId { get; set; }
    public string PatternName { get; set; }
}

public class GetInfoProductLineModel
{
    public int LineId { get; set; }
    public string LineName { get; set; }
    public string LineNo { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
}

public class ProductLineModel
{
    public int LineId { get; set; }
    public int ProductId { get; set; }
}

public class AssignProductLineModel
{
    public List<ProductLineModel> ListProductLine { get; set; }
    public int FactoryId { get; set; }
}

public class StationProcessModel
{
    public int StationId { get; set; }
    public int ProcessId { get; set; }
}

public class AssignStationProcessModel
{
    public List<StationProcessModel> ListStationProcess { get; set; }
    public int FactoryId { get; set; }
    public int LineId { get; set; }
}

public class GetStationAndProcessModel
{
    public int LineId { get; set; }
    public string LineName { get; set; }
    public string LineNo { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductNo { get; set; }
    public int StationId { get; set; }
    public string StationName { get; set; }
    public string StationNo { get; set; }
    public int ProcessId { get; set; }
    public string ProcessName { get; set; }
    public string ProcessNo { get; set; }
}
