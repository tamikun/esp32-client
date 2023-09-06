namespace esp32_client.Models;



public class StationUpdateModel
{
    public int Id { get; set; }
    public string StationName { get; set; }

}

public class ListStationUpdateModel
{
    public List<StationUpdateModel> ListStationUpdate { get; set; }
    public int FactoryId { get; set; }
    public int LineId { get; set; }

}
