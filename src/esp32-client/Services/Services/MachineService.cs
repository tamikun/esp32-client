using System.Text;
using AutoMapper;
using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
using esp32_client.Models.Singleton;
using HtmlAgilityPack;
using LinqToDB;
using Newtonsoft.Json;

namespace esp32_client.Services;

/// <summary>
/// 
/// </summary>
public partial class MachineService : IMachineService
{

    private readonly LinqToDb _linq2Db;
    private readonly IMapper _mapper;
    private readonly IProcessService _processService;
    private readonly ILogService _logService;
    private readonly Settings _settings;

    public MachineService(LinqToDb linq2Db, IMapper mapper, IProcessService processService, Settings settings, ILogService logService)
    {
        _linq2Db = linq2Db;
        _mapper = mapper;
        _processService = processService;
        _settings = settings;
        _logService = logService;
    }

    public async Task<Machine?> GetById(int id)
    {
        var machine = await _linq2Db.Machine.Where(s => s.Id == id).FirstOrDefaultAsync();
        return machine;
    }

    public async Task<List<Machine>> GetByListId(IEnumerable<int> listId)
    {
        return await _linq2Db.Machine.Where(s => listId.Contains(s.Id)).ToListAsync();
    }

    public async Task<List<Machine>> GetAll()
    {
        return await _linq2Db.Machine.ToListAsync();
    }

    public async Task<List<MachineResponseModel>> GetByFactoryId(int factoryId)
    {
        var response = await (from machine in _linq2Db.Machine.Where(s => s.FactoryId == factoryId)
                              join line1 in _linq2Db.Line.Where(s => s.FactoryId == factoryId) on machine.LineId equals line1.Id into line2
                              from line in line2.DefaultIfEmpty()
                              join process1 in _linq2Db.Process on machine.StationId equals process1.Id into process2
                              from process in process2.DefaultIfEmpty()
                              select new MachineResponseModel
                              {
                                  MachineId = machine.Id,
                                  LineId = machine.LineId,
                                  StationId = machine.StationId,
                                  MachineName = machine.MachineName,
                                  MachineNo = machine.MachineNo,
                                  IpAddress = machine.IpAddress,
                                  LineName = line.LineName,
                                  ProcessName = process.ProcessName,
                                  COPartNo = machine.COPartNo,
                              }
                            ).ToListAsync();
        return response;
    }

    public async Task<List<Machine>> GetInUseMachineByLine(int lineId)
    {
        var result = await (from line in _linq2Db.Line.Where(s => s.Id == lineId)
                            join station in _linq2Db.Station on line.Id equals station.LineId
                            from machine in _linq2Db.Machine.Where(s => s.LineId == lineId && s.StationId == station.Id)
                            select machine
                            ).ToListAsync();
        return result;
    }

    public async Task<List<Machine>> GetAvalableMachine(int lineId)
    {
        var result = await _linq2Db.Machine.Where(s => s.LineId == 0 || s.StationId == 0 || s.LineId == lineId).ToListAsync();
        return result;
    }

    public async Task<List<Machine>> UpdateMachineLineByProduct(int lineId, int productId)
    {
        var machines = await GetInUseMachineByLine(lineId);
        if (machines.Count == 0) return new List<Machine>();

        var processes = await _processService.GetByProductId(productId);

        int minIndex = Math.Min(machines.Count, processes.Count);

        for (int i = 0; i < minIndex; i++)
        {
            machines[i].StationId = processes[i].Id;
        }

        if (machines.Count > minIndex)
        {
            for (int i = minIndex; i < machines.Count; i++)
            {
                machines[i].LineId = 0;
                machines[i].StationId = 0;
            }
        }

        foreach (var machine in machines)
        {
            await Update(machine);
        }

        return machines;
    }

    public async Task<Machine> Create(MachineCreateModel model)
    {
        #region validation
        if (model.FactoryId == 0)
            throw new Exception("Invalid factory");
        if (model.MachineNo <= 0)
            throw new Exception("Machine No should be greater than 0");
        if (String.IsNullOrEmpty(model.IpAddress))
            throw new Exception("Ip Address is required");
        #endregion

        var machine = new Machine();
        machine.MachineName = model.MachineName;
        machine.IpAddress = model.IpAddress;
        machine.FactoryId = model.FactoryId;

        string formattedNumber = model.MachineNo.ToString($"D{_settings.MinCharMachineFormat}");
        machine.MachineNo = string.Format(_settings.MachineFormat, formattedNumber);

        machine = await _linq2Db.Insert(machine);
        return machine;
    }

    public async Task<Machine> Update(MachineUpdateModel model)
    {
        var machine = await GetById(model.MachineId);
        if (machine is null) throw new Exception("Machine is not found");

        machine.MachineName = model.MachineName;

        // Allow change ip when machine is not in use
        if (machine.LineId == 0 && machine.StationId == 0)
        {
            machine.IpAddress = model.IpAddress;
        }

        await _linq2Db.Update(machine);
        return machine;
    }

    public async Task DeleteById(int id)
    {
        var machine = await GetById(id);
        if (machine is null) throw new Exception("Machine is not found");
        if (machine.LineId != 0 || machine.StationId != 0) throw new Exception("Machine is in use");
        await _linq2Db.Delete(machine);
    }

    public async Task<Dictionary<string, string>> AssignMachineLine(ListAssignMachineLineModel model)
    {
        // Validation
        // Duplicate machine in line
        if (model.ListAssignMachine.Where(s => s.MachineId != 0).GroupBy(s => s.MachineId).Any(s => s.Count() > 1))
            throw new Exception("A machine cannot be used for more than one station.");

        var listUpdateMachineId = new List<int>();

        foreach (var item in model.ListAssignMachine)
        {
            if (item.MachineId == 0)
            {
                var machine = await _linq2Db.Machine
                            .Where(s => s.StationId == item.StationId)
                            .Where(s => s.LineId != 0 || s.StationId != 0)
                            .FirstOrDefaultAsync();
                if (machine is not null)
                {
                    await _linq2Db.Machine
                            .Where(s => s.StationId == item.StationId)
                            .Set(s => s.LineId, 0)
                            .Set(s => s.StationId, 0).UpdateQuery();

                    listUpdateMachineId.Add(machine.Id);
                }
            }
            else
            {
                var machine = await _linq2Db.Machine
                            .Where(s => s.Id == item.MachineId)
                            .Where(s => s.LineId != model.LineId || s.StationId != item.StationId)
                            .FirstOrDefaultAsync();

                if (machine is not null)
                {

                    await _linq2Db.Machine
                        .Where(s => s.Id == item.MachineId)
                        .Set(s => s.LineId, model.LineId)
                        .Set(s => s.StationId, item.StationId).UpdateQuery();

                    listUpdateMachineId.Add(machine.Id);
                }
            }
        }


        // Update file
        var assignPattern = await AssignPatternMachine(listUpdateMachineId);

        return assignPattern;
    }

    public async Task UpdateById(int id, int departmentId, int lineId, int processId)
    {
        await _linq2Db.Machine.Where(s => s.Id == id)
                .Set(s => s.FactoryId, departmentId)
                .Set(s => s.LineId, lineId)
                .Set(s => s.StationId, processId).UpdateQuery();
    }

    public async Task UpdateByListId(IEnumerable<int> listId, int departmentId, int lineId, int processId)
    {

        await _linq2Db.Machine.Where(s => listId.Contains(s.Id))
               .Set(s => s.FactoryId, departmentId)
               .Set(s => s.LineId, lineId)
               .Set(s => s.StationId, processId).UpdateQuery();
    }

    public async Task<Machine> Update(Machine model)
    {
        await _linq2Db.Update(model);
        return model;
    }

    public async Task<Dictionary<string, string>> AssignPatternMachine(IEnumerable<int> machineId)
    {
        var result = new Dictionary<string, string>();

        // Machine => Station => Process => Pattern
        var data = await (from machine in _linq2Db.Machine.Where(s => machineId.Contains(s.Id))
                          join station1 in _linq2Db.Station on machine.StationId equals station1.Id into station2
                          from station in station2.DefaultIfEmpty()
                          join process1 in _linq2Db.Process on station.ProcessId equals process1.Id into process2
                          from process in process2.DefaultIfEmpty()
                          select new { machine.IpAddress, process.PatternDirectory, process.PatternNo }
                        ).ToListAsync();

        var tasks = data.Select(async s =>
        {
            try
            {
                bool stepSuccess = true;

                // Change state to server
                var changeState = await ChangeState(s.IpAddress, ServerState.Server);
                stepSuccess &= changeState.Success;

                if (!stepSuccess)
                {
                    result.Add(s.IpAddress, "Cannot change to server state");
                    return;
                }

                // Delete if File exists
                var listCurrentFile = await GetDefaultListFile(s.IpAddress);
                if (String.IsNullOrEmpty(s.PatternDirectory))
                {
                    if (_settings.DeleteOnUploadingEmptyFile)
                    {
                        foreach (var file in listCurrentFile)
                        {
                            var deleteFile = await DeleteFile(s.IpAddress, file.FileName);
                            stepSuccess &= deleteFile.Success;
                        }
                    }
                }
                else
                {
                    foreach (var file in listCurrentFile)
                    {
                        var deleteFile = await DeleteFile(s.IpAddress, file.FileName);
                        stepSuccess &= deleteFile.Success;
                    }
                }

                if (!stepSuccess)
                {
                    result.Add(s.IpAddress, "Cannot delete old file");
                    return;
                }

                // Upload new file
                if (!String.IsNullOrEmpty(s.PatternDirectory))
                {
                    var file = File.ReadAllBytes(s.PatternDirectory);
                    var postFile = await Post(file, s.IpAddress, s.PatternNo);
                    stepSuccess &= postFile.Success;
                }

                if (!stepSuccess)
                {
                    result.Add(s.IpAddress, "Cannot upload new file");
                    return;
                }

                // Change state back to machine
                changeState = await ChangeState(s.IpAddress, ServerState.Machine);
                stepSuccess &= changeState.Success;

                if (!stepSuccess)
                {
                    result.Add(s.IpAddress, "Cannot change state to machine");
                    return;
                }

                result.Add(s.IpAddress, "Success");
            }
            catch (Exception ex)
            {
                result.Add(s.IpAddress, ex.Message);
            }
        });

        await Task.WhenAll(tasks);

        return result;
    }

    public async Task<(bool Success, int Data)> GetProductNumberMachine(string ipAddress)
    {
        try
        {
            var result = await Get(GetProductNumberUrl(ipAddress), _settings.GetApiProductNumberTimeOut);
            var dict = JsonConvert.DeserializeObject<Dictionary<string, int>>(result.ResponseBody);
            int number = 0;

            if (dict is not null)
                dict.TryGetValue("Data", out number);

            return (result.Success, number);
        }
        catch (Exception ex)
        {
            await _logService.AddLog(ex);
            return (false, 0);
        }
    }

    private string GetProductNumberUrl(string machineIp, bool isEndWithSlash = false)
    {
        string rs = string.Format(_settings.GetProductNumberFormat, machineIp);
        if (isEndWithSlash) rs = rs + '/';
        return rs;
    }
    private string GetChangeMachineStateUrl(string machineIp, bool isEndWithSlash = false)
    {
        string rs = string.Format(_settings.ChangeMachineStateFormat, machineIp);
        if (isEndWithSlash) rs = rs + '/';
        return rs;
    }
    private string GetChangeServerStateUrl(string machineIp, bool isEndWithSlash = false)
    {
        string rs = string.Format(_settings.ChangeServerStateFormat, machineIp);
        if (isEndWithSlash) rs = rs + '/';
        return rs;
    }

    private string GetDeleteFileUrl(string machineIp, string fileName, bool isEndWithSlash = false)
    {
        string rs = string.Format(_settings.DeleteFileFormat, machineIp, fileName);
        if (isEndWithSlash) rs = rs + '/';
        return rs;
    }

    private string GetPostFileUrl(string machineIp, string fileName, bool isEndWithSlash = false)
    {
        string rs = string.Format(_settings.PostFileFormat, machineIp, fileName);
        if (isEndWithSlash) rs = rs + '/';
        return rs;
    }

    private string GetListFileUrl(string machineIp, bool isEndWithSlash = false)
    {
        string rs = string.Format(_settings.GetListFileFormat, machineIp);
        if (isEndWithSlash) rs = rs + '/';
        return rs;
    }

    public async Task<(bool Success, string ResponseBody)> Get(string requestUri, long? timeOut = null)
    {
        using (HttpClient client = new HttpClient())
        {
            string responseBody;
            client.Timeout = TimeSpan.FromMilliseconds(timeOut ?? _settings.GetApiTimeOut);

            HttpResponseMessage response = await client.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
            {
                responseBody = response.StatusCode.ToString();
                return (false, responseBody);
            }

            responseBody = await response.Content.ReadAsStringAsync();
            return (true, responseBody);
        }
    }

    public async Task<List<EspFileModel>> GetListFile(string apiUrl)
    {
        List<EspFileModel> fileDataList = new List<EspFileModel>();

        string node = _settings.NodeListEspFile;
        var response = await Get(apiUrl);

        if (response.Success)
        {
            string html = response.ResponseBody;

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            HtmlNodeCollection tableRows = htmlDoc.DocumentNode.SelectNodes(node);
            if (tableRows != null && tableRows.Count > 0)
            {
                foreach (HtmlNode row in tableRows)
                {
                    HtmlNodeCollection tableCells = row.SelectNodes("td");
                    if (tableCells != null && tableCells.Count >= 4)
                    {
                        EspFileModel fileData = new EspFileModel();
                        fileData.FileName = tableCells[0].InnerText.Trim();
                        fileData.FileType = tableCells[1].InnerText.Trim();
                        fileData.FileSize = long.Parse(tableCells[2].InnerText.Trim());
                        fileDataList.Add(fileData);
                    }
                }
            }
        }

        return fileDataList;
    }

    public async Task<(bool Success, string ResponseBody)> Post(string? requestBody, string apiUrl)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            string responseBody;

            httpClient.Timeout = TimeSpan.FromMilliseconds(_settings.PostApiTimeOut);
            HttpResponseMessage response;

            if (requestBody is null)
            {
                response = await httpClient.PostAsync(apiUrl, null);
            }
            else
            {
                HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                response = await httpClient.PostAsync(apiUrl, content);
            }

            if (!response.IsSuccessStatusCode)
            {
                responseBody = response.StatusCode.ToString();
                return (false, responseBody);
            }

            responseBody = await response.Content.ReadAsStringAsync();
            return (true, responseBody);
        }
    }

    public async Task<(bool Success, string ResponseBody)> ChangeState(string machinIp, ServerState state)
    {
        var url = "";

        if (state == ServerState.Machine)
            url = GetChangeMachineStateUrl(machinIp);

        if (state == ServerState.Server)
            url = GetChangeServerStateUrl(machinIp);

        var result = await Get(url);
        await Task.Delay(_settings.ChangeStateDelay);
        return result;
    }

    public async Task<List<EspFileModel>> GetDefaultListFile(string machineIp)
    {
        return await GetListFile(GetListFileUrl(machineIp, isEndWithSlash: true));
    }

    public async Task<(bool Success, string ResponseBody)> DeleteFile(string ipAddress, string? fileName)
    {
        if (fileName is null) return (true, "");

        string url = GetDeleteFileUrl(ipAddress, fileName);

        var response = await Post(requestBody: null, apiUrl: url);

        return response;
    }

    public async Task<(bool Success, string ResponseBody)> Post(byte[] byteContent, string machineIp, string fileName)
    {
        var url = GetPostFileUrl(machineIp, fileName);

        using (var httpClient = new HttpClient())
        {
            string responseBody;

            httpClient.Timeout = TimeSpan.FromMilliseconds(_settings.PostFileTimeOut);

            var fileContent = new ByteArrayContent(byteContent);
            var response = await httpClient.PostAsync(url, fileContent);


            if (!response.IsSuccessStatusCode)
            {
                responseBody = response.StatusCode.ToString();
                return (false, responseBody);
            }

            responseBody = await response.Content.ReadAsStringAsync();
            return (true, responseBody);
        }
    }
}