using esp32_client.Services;

namespace esp32_client.Models;

public class ServerModel
{
    public string? IpAddress { get; set; }
}

public class ServerModelDetail
{
    public string? IpAddress { get; set; }
    public string? SubDirectory { get; set; }
    public string? AlertMessage { get; set; }
}

public class ListServer
{
    private static ListServer? instance;
    private List<ServerModel> itemList;
    private readonly IClientService _clientService;

    // Private constructor to prevent direct instantiation
    private ListServer(IClientService clientService)
    {
        _clientService = clientService;

        // Initialize the list of items
        itemList = CreateItemList();
    }

    // Public method to get the instance of the singleton class
    public static ListServer GetInstance(IClientService clientService)
    {
        // Create a new instance if it doesn't exist
        if (instance == null)
        {
            instance = new ListServer(clientService);
        }

        return instance;
    }

    // Method to retrieve the list of items
    public List<ServerModel> GetItemList()
    {
        return itemList;
    }

    // Your method to create the list of items
    private List<ServerModel> CreateItemList()
    {
        // Implementation to create and return the list of items
        var response = _clientService.GetAvailableIpAddress();

        return response;
    }

    // Reload
    public void ReloadItemList()
    {
        // Implementation to create and return the list of items
        itemList = _clientService.GetAvailableIpAddress();
    }
}
