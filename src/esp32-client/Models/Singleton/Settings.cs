using System.Reflection;
using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Utils;
using LinqToDB;

namespace esp32_client.Models.Singleton
{
    public class Settings
    {
        private readonly LinqToDb _linqToDb;

        public Settings(LinqToDb linqToDb)
        {
            _linqToDb = linqToDb;
            Reload().Wait();
        }
        public async Task Reload()
        {
            var listSetting = await _linqToDb.Entity<Setting>().ToListAsync();
            var settingType = this.GetType();
            var propertyInfos = settingType.GetProperties();
            foreach (var property in propertyInfos)
            {
                PropertyInfo pinfo = settingType.GetProperty(property.Name);
                if (pinfo is not null)
                    pinfo.SetValue(
                        this,
                        listSetting.Where(s => s.Name == property.Name).Select(s => s.Value).FirstOrDefault().ChangeType(pinfo.PropertyType),
                        null);
            }
        }

        public long GetApiTimeOut { get; set; }
        public long GetApiProductNumberTimeOut { get; set; }
        public long PostFileTimeOut { get; set; }
        public long PostApiTimeOut { get; set; }
        public string FileDataDirectory { get; set; }
        public string NodeListEspFile { get; set; }
        public string UploadFileFormat { get; set; }
        public string ChangeMachineStateFormat { get; set; }
        public string ChangeServerStateFormat { get; set; }
        public string ResetMachineFormat { get; set; }
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
        public string GetProductNumberFormat { get; set; }
        public string AcceptedFile { get; set; }
        public bool DeleteOnUploadingEmptyFile { get; set; }
        public int ReloadMonitoringMilliseconds { get; set; }
        public int ReloadMonitoringBatchSize { get; set; }
        public bool EnableLog { get; set; }
        public string JwtTokenSecret { get; set; }
        public int SessionExpiredTimeInSecond { get; set; }
        public string MachineFirmwareFilePath { get; set; }
        public string DefaultNewMachineIp { get; set; }
    }
}