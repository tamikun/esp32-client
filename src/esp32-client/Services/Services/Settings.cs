using System.Reflection;

namespace esp32_client.Services
{
    public class Settings
    {
        private readonly IConfiguration _configuration;

#nullable enable
        public Settings(IConfiguration configuration)
        {
            _configuration = configuration;

            var settingType = this.GetType();

            var propertyInfos = settingType.GetProperties();

            foreach (var property in propertyInfos)
            {
                PropertyInfo? pinfo = settingType.GetProperty(property.Name);
                if (pinfo is not null)
                    pinfo.SetValue(this, Utils.Utils.ChangeType(_configuration[$"Settings:{property.Name}"], pinfo.PropertyType), null);
            }
            GUID = Guid.NewGuid().ToString();
        }
#nullable disable

        public string Subnet { get; set; }
        public string Port { get; set; }
        public long ConnectionTimeOut { get; set; }
        public long GetDataTimeOut { get; set; }
        public long PostFileTimeOut { get; set; }
        public string FileDataDirectory { get; set; }
        public string ListServer { get; set; }
        public string NodeServerName { get; set; }
        public string ServerNamePath { get; set; }
        public string NodeListEspFile { get; set; }
        public string NodeServerState { get; set; }
        public string GUID { get; set; }
    }
}