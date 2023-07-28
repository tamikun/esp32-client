using System.Text;
using AutoMapper;
using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
using HtmlAgilityPack;
using LinqToDB;

namespace esp32_client.Services;

/// <summary>
/// 
/// </summary>
public partial class MachineService : IMachineService
{

    private readonly LinqToDb _linq2Db;
    private readonly IMapper _mapper;
    private readonly IProcessService _processService;
    private readonly Settings _settings;

    public MachineService(LinqToDb linq2Db, IMapper mapper, IProcessService processService, Settings settings)
    {
        _linq2Db = linq2Db;
        _mapper = mapper;
        _processService = processService;
        _settings = settings;
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

    public async Task<List<Machine>> GetInUseMachineByLine(int lineId)
    {
        var result = await (from line in _linq2Db.Line.Where(s => s.Id == lineId)
                            join process in _linq2Db.Process on line.ProductId equals process.ProductId
                            from machine in _linq2Db.Machine.Where(s => s.LineId == lineId && s.ProcessId == process.Id)
                            select machine
                            ).ToListAsync();
        return result;
    }

    public async Task<List<Machine>> GetAvalableMachine(int lineId)
    {
        var result = await _linq2Db.Machine.Where(s => s.LineId == 0 || s.ProcessId == 0 || s.LineId == lineId).ToListAsync();
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
            machines[i].ProcessId = processes[i].Id;
        }

        if (machines.Count > minIndex)
        {
            for (int i = minIndex; i < machines.Count - minIndex; i++)
            {
                machines[i].LineId = 0;
                machines[i].ProcessId = 0;
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
        var machine = _mapper.Map<Machine>(model);
        await _linq2Db.InsertAsync(machine);
        return machine;
    }

    public async Task<Machine> Update(MachineUpdateModel model)
    {
        var machine = await GetById(model.Id);
        if (machine is null) throw new Exception("Machine is not found");

        var machineUpdate = _mapper.Map<Machine>(model);
        machineUpdate.Id = machine.Id;

        await _linq2Db.Update(machineUpdate);
        return machineUpdate;
    }

    public async Task UpdateById(int id, int departmentId, int lineId, int processId)
    {

        await _linq2Db.Machine.Where(s => s.Id == id)
                    .Set(s => s.DepartmentId, departmentId)
                    .Set(s => s.LineId, lineId)
                    .Set(s => s.ProcessId, processId)
                    .UpdateAsync();
    }

    public async Task UpdateByListId(IEnumerable<int> listId, int departmentId, int lineId, int processId)
    {

        await _linq2Db.Machine.Where(s => listId.Contains(s.Id))
                    .Set(s => s.DepartmentId, departmentId)
                    .Set(s => s.LineId, lineId)
                    .Set(s => s.ProcessId, processId)
                    .UpdateAsync();
    }

    public async Task<Machine> Update(Machine model)
    {
        await _linq2Db.Update(model);
        return model;
    }

    public async Task Delete(int id)
    {
        var machine = await GetById(id);
        if (machine is null) throw new Exception("Machine is not found");
        if (machine.LineId != 0 || machine.ProcessId != 0) throw new Exception("Machine is in use");

        await _linq2Db.DeleteAsync(machine);
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

    public async Task<string> GetAsyncApi(string requestUri)
    {
        using (HttpClient client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromMilliseconds(_settings.GetApiTimeOut);

            HttpResponseMessage response = await client.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
                throw new Exception(response.StatusCode.ToString());

            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
    }

    public async Task<List<EspFileModel>> GetListFile(string apiUrl)
    {
        string node = _settings.NodeListEspFile;
        var pageData = await GetAsyncApi(apiUrl);

        string html = pageData;

        List<EspFileModel> fileDataList = new List<EspFileModel>();

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
        return fileDataList;
    }

    public async Task<List<EspFileModel>> GetDefaultListFile(string machineIp)
    {
        return await GetListFile(GetListFileUrl(machineIp));
    }

    public async Task<HttpResponseMessage> PostAsyncApi(string? requestBody, string apiUrl)
    {
        using (HttpClient httpClient = new HttpClient())
        {
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

            return response;
        }
    }

    public async Task<HttpResponseMessage> PostAsyncFile(byte[] byteContent, string machineIp, string fileName)
    {
        var url = GetPostFileUrl(machineIp, fileName);

        using (var httpClient = new HttpClient())
        {
            httpClient.Timeout = TimeSpan.FromMilliseconds(_settings.PostFileTimeOut);

            var fileContent = new ByteArrayContent(byteContent);
            var response = await httpClient.PostAsync(url, fileContent);
            return response;
        }
    }

    public async Task ChangeState(string machinIp, ServerState state)
    {
        var url = "";

        if (state == ServerState.Machine)
            url = GetChangeMachineStateUrl(machinIp);

        if (state == ServerState.Server)
            url = GetChangeServerStateUrl(machinIp);

        await GetAsyncApi(url);
        await Task.Delay(_settings.ChangeStateDelay);
    }

    public async Task<HttpResponseMessage> DeleteFile(string ipAddress, string fileName)
    {
        string url = GetDeleteFileUrl(ipAddress, fileName);

        var response = await PostAsyncApi(requestBody: null, apiUrl: url);

        return response;
    }

}