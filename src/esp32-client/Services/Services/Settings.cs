using System.Reflection;
using esp32_client.Builder;
using LinqToDB;

namespace esp32_client.Services
{
    public class Settings
    {
        private readonly LinqToDb _linqToDb;

#nullable enable
        public Settings(LinqToDb linqToDb)
        {
            _linqToDb = linqToDb;
            var listSetting = _linqToDb.Setting.ToList();

            var settingType = this.GetType();

            var propertyInfos = settingType.GetProperties();

            foreach (var property in propertyInfos)
            {
                PropertyInfo? pinfo = settingType.GetProperty(property.Name);
                if (pinfo is not null)
                    // pinfo.SetValue(this, Utils.Utils.ChangeType(_linqToDb[$"Settings:{property.Name}"], pinfo.PropertyType), null);
                    pinfo.SetValue(this, Utils.Utils.ChangeType(listSetting.Where(s => s.Name == property.Name).Select(s => s.Value).FirstOrDefault(), pinfo.PropertyType), null);
            }
        }
#nullable disable

        public string Subnet { get; set; }
        public string Port { get; set; }
        public long ConnectionTimeOut { get; set; }
        public long GetApiTimeOut { get; set; }
        public long PostFileTimeOut { get; set; }
        public long PostApiTimeOut { get; set; }
        public string FileDataDirectory { get; set; }
        public string ListServer { get; set; }
        public string NodeServerName { get; set; }
        public string ServerNamePath { get; set; }
        public string NodeListEspFile { get; set; }
        public string NodeServerState { get; set; }
        public string UploadFileFormat { get; set; }
        public string ChangeMachineStateFormat { get; set; }
        public string ChangeServerStateFormat { get; set; }
        public int ChangeStateDelay { get; set; }
        public string DeleteFileFormat { get; set; }
        public string PostFileFormat { get; set; }
        public string GetListFileFormat { get; set; }
        public string FactoryFormat { get; set; }
        public int MinCharFactoryFormat { get; set; }
        public string LineFormat { get; set; }
        public int MinCharLineFormat { get; set; }
        public string StationFormat { get; set; }
        public int MinCharStationFormat { get; set; }
        public string ProductFormat { get; set; }
        public int MinCharProductFormat { get; set; }
        public string MachineFormat { get; set; }
        public int MinCharMachineFormat { get; set; }
    }
}