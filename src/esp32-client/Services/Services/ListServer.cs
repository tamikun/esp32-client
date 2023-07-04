using esp32_client.Models;

namespace esp32_client.Services
{
    public class ListServer
    {
        private static ListServer? instance;
        private List<ServerModel> dynamicList;
        private List<ServerModel> staticList;
        private readonly IClientService _clientService;

        // Private constructor to prevent direct instantiation
        private ListServer(IClientService clientService)
        {
            _clientService = clientService;

            dynamicList = new List<ServerModel>();
            staticList = new List<ServerModel>();
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
            if (dynamicList.Count == 0)
                CreateDynamicList();
            return dynamicList;
        }

        private void CreateDynamicList()
        {
            // Implementation to create and return the list of items
            dynamicList = _clientService.GetAvailableIpAddress();
        }

        // Reload
        public void ReloadDynamicList()
        {
            // Implementation to create and return the list of items
            dynamicList = _clientService.GetAvailableIpAddress();
        }

        public async Task<List<ServerModel>> GetStaticList()
        {
            if (staticList.Count == 0)
                staticList = await _clientService.GetStaticIpAddress();
            return staticList;
        }
    }
}