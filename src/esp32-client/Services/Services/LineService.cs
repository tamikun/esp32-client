using System.Linq.Expressions;
using AutoMapper;
using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
using LinqToDB;

namespace esp32_client.Services;

/// <summary>
/// 
/// </summary>
public partial class LineService : ILineService
{

    private readonly LinqToDb _linq2Db;
    private readonly IMachineService _machineService;
    private readonly IMapper _mapper;
    private readonly Settings _settings;

    public LineService(LinqToDb linq2Db, IMapper mapper, IMachineService machineService, Settings settings)
    {
        _linq2Db = linq2Db;
        _mapper = mapper;
        _machineService = machineService;
        _settings = settings;
    }

    public async Task<Line?> GetById(int id)
    {
        var line = await _linq2Db.Line.Where(s => s.Id == id).FirstOrDefaultAsync();
        return line;
    }

    public async Task<List<Line>> GetByFactoryId(int factoryId)
    {
        return await _linq2Db.Line.Where(s => s.FactoryId == factoryId).ToListAsync();
    }

    public async Task<List<LineResponseModel>> GetAllLineResponse(int factoryId)
    {
        var result = await (from factory in _linq2Db.Factory.Where(s => s.Id == factoryId)
                            from line in _linq2Db.Line.Where(s => s.FactoryId == factoryId)
                            join product1 in _linq2Db.Product on line.ProductId equals product1.Id into product2
                            from product in product2.DefaultIfEmpty()
                            select new LineResponseModel
                            {
                                FactoryId = factory.Id,
                                FactoryName = factory.FactoryName,
                                LineId = line.Id,
                                LineName = line.LineName,
                                LineNo = line.LineNo,
                                ProductId = line.ProductId,
                                ProductName = product.ProductName,
                                NumberOfStation = _linq2Db.Station.Where(s => s.LineId == line.Id).Count()
                            }).OrderBy(s => s.LineNo).ToListAsync();

        return result;
    }

    public async Task<Line?> GetByLineNo(string lineNo, int factoryId)
    {
        var line = await _linq2Db.Line.Where(s => s.LineNo == lineNo && s.FactoryId == factoryId).FirstOrDefaultAsync();
        return line;
    }

    public async Task<Line> Create(LineCreateModel model)
    {
        if (model.FactoryId == 0) throw new Exception("Invalid factory");

        var line = new Line();
        line.LineName = model.LineName;
        line.FactoryId = model.FactoryId;

        string formattedNumber = model.LineNo.ToString($"D{_settings.MinCharLineFormat}");

        line.LineNo = string.Format(_settings.LineFormat, formattedNumber);

        await _linq2Db.InsertAsync(line);

        // Get line id
        line = await GetByLineNo(line.LineNo, model.FactoryId) ?? new Line();

        // Create station
        await AddStation(line.Id, model.NumberOfStation);

        return line;
    }


    public async Task<List<GetInfoProductLineModel>> GetInfoProductLine(int factoryId)
    {
        var result = await (from line in _linq2Db.Line.Where(s => s.FactoryId == factoryId)
                            join product1 in _linq2Db.Product on line.ProductId equals product1.Id into product2
                            from product in product2.DefaultIfEmpty()
                            select new GetInfoProductLineModel
                            {
                                LineId = line.Id,
                                LineName = line.LineName,
                                LineNo = line.LineNo,
                                ProductId = product.Id,
                                ProductName = product.ProductName,
                            }).ToListAsync();
        return result;
    }

    public async Task<Dictionary<string, string>> AssignProductLine(AssignProductLineModel model)
    {
        var queryListUpdate = _linq2Db.Line.Where(s => s.FactoryId == model.FactoryId);

        foreach (var item in model.ListProductLine)
        {
            queryListUpdate = queryListUpdate.Where(s => !(s.Id == item.LineId && s.ProductId == item.ProductId));
        }

        var listUpdate = await queryListUpdate.ToListAsync();

        // Update product of Line
        foreach (var item in model.ListProductLine)
        {
            await queryListUpdate.Where(s => s.Id == item.LineId)
                       .Set(s => s.ProductId, item.ProductId)
                       .UpdateAsync();
        }

        // Update process of Station (Set ProcessId to 0)
        var listLineId = listUpdate.Select(s => s.Id);

        var listStationQuery = _linq2Db.Station.Where(s => listLineId.Contains(s.LineId));
        await listStationQuery.Set(s => s.ProcessId, 0).UpdateAsync();

        // Update Pattern for machine (Set LineId = 0, ProcessId to 0 => Delete pattern)
        var listMachineId =  _linq2Db.Machine.Where(s => listLineId.Contains(s.LineId)).Select(s => s.Id);

        var response = await _machineService.AssignPatternMachine(listMachineId);
        return response;
    }

    public async Task<Dictionary<string, string>> AssignStationProcess(AssignStationProcessModel model)
    {
        // Validation
        // Duplicate process in line
        if (model.ListStationProcess.Where(s => s.ProcessId != 0).GroupBy(s => s.ProcessId).Any(s => s.Count() > 1))
            throw new Exception("A process cannot be used for more than one station.");

        // Get list station change
        var queryListUpdate = _linq2Db.Station.Where(s => true);

        foreach (var item in model.ListStationProcess)
        {
            queryListUpdate = queryListUpdate.Where(s => !(s.Id == item.StationId && s.ProcessId == item.ProcessId));
        }

        var listStationUpdate = await queryListUpdate.ToListAsync();

        // Update station data
        foreach (var item in model.ListStationProcess)
        {
            await _linq2Db.Station.Where(s => s.Id == item.StationId)
                       .Set(s => s.ProcessId, item.ProcessId)
                       .UpdateAsync();
        }

        // Get list machine that is in StationId
        var listMachine = await _linq2Db.Machine.Where(s => listStationUpdate.Select(s => s.Id).Contains(s.StationId)).Select(s => s.Id).ToListAsync();

        // Update pattern for machine
        var response = await _machineService.AssignPatternMachine(listMachine);
        return response;
    }

    public async Task<List<GetStationAndProcessModel>> GetStationAndProcess(int lineId)
    {
        var result = await (from line in _linq2Db.Line.Where(s => s.Id == lineId)
                            join station1 in _linq2Db.Station on line.Id equals station1.LineId into station2
                            from station in station2.DefaultIfEmpty()
                            join product1 in _linq2Db.Product on line.ProductId equals product1.Id into product2
                            from product in product2.DefaultIfEmpty()
                            join process1 in _linq2Db.Process on station.ProcessId equals process1.Id into process2
                            from process in process2.DefaultIfEmpty()
                            select new GetStationAndProcessModel
                            {
                                LineId = line.Id,
                                LineName = line.LineName,
                                LineNo = line.LineNo,
                                ProductId = product.Id,
                                ProductName = product.ProductName,
                                ProductNo = product.ProductNo,
                                StationId = station.Id,
                                StationName = station.StationName,
                                StationNo = station.StationNo,
                                ProcessId = process.Id,
                                ProcessName = process.ProcessName,
                                ProcessNo = process.ProcessNo,
                            }).ToListAsync();
        return result;
    }

    public async Task<List<GetProcessAndMachineOfLineModel>> GetProcessAndMachineOfLine(int factoryId)
    {
        var result = await (from line in _linq2Db.Line.Where(s => s.FactoryId == factoryId)
                            join station1 in _linq2Db.Station on line.Id equals station1.LineId into station2
                            from station in station2.DefaultIfEmpty()
                            join product1 in _linq2Db.Product on line.ProductId equals product1.Id into product2
                            from product in product2.DefaultIfEmpty()
                            join process1 in _linq2Db.Process on station.ProcessId equals process1.Id into process2
                            from process in process2.DefaultIfEmpty()
                            from machine in _linq2Db.Machine.Where(s =>
                                s.FactoryId == factoryId
                                && s.LineId == line.Id
                                && s.StationId == station.Id
                            ).DefaultIfEmpty()
                            select new GetProcessAndMachineOfLineModel
                            {
                                LineId = line.Id,
                                LineName = string.IsNullOrEmpty(line.LineName) ? "LineName: {Empty}" : line.LineName,
                                LineNo = line.LineNo,
                                StationId = station.Id,
                                StationName = string.IsNullOrEmpty(station.StationName) ? "StationName: {Empty}" : station.StationName,
                                StationNo = string.IsNullOrEmpty(station.StationNo) ? "StationNo: {Empty}" : station.StationNo,
                                ProductName = string.IsNullOrEmpty(product.ProductName) ? "ProductName: {Empty}" : product.ProductName,
                                ProductNo = string.IsNullOrEmpty(product.ProductNo) ? "ProductNo: {Empty}" : product.ProductNo,
                                ProcessName = string.IsNullOrEmpty(process.ProcessName) ? "ProcessName: {Empty}" : process.ProcessName,
                                ProcessNo = string.IsNullOrEmpty(process.ProcessNo) ? "ProcessNo: {Empty}" : process.ProcessNo,
                                PatternNo = string.IsNullOrEmpty(process.PatternNo) ? "PatternNo: {Empty}" : process.PatternNo,
                                PatterDescription = process.Description,
                                MachineName = string.IsNullOrEmpty(machine.MachineName) ? "MachineName: {Empty}" : machine.MachineName,
                                MachineNo = string.IsNullOrEmpty(machine.MachineNo) ? "MachineNo: {Empty}" : machine.MachineNo,
                                MachineIp = machine.IpAddress,
                            }).OrderBy(s => s.LineId).ThenBy(s => s.StationId).ToListAsync();

        return result;
    }

    public async Task Delete(int lineId)
    {
        var line = await GetById(lineId);
        if (line is null) throw new Exception("Line is not found");

        // Delete line
        await _linq2Db.DeleteAsync(line);

        // Delete station
        await _linq2Db.Station.Where(s => s.LineId == line.Id).DeleteAsync();

        // Release machine
        await _linq2Db.Machine.Where(s => s.LineId == line.Id)
                                .Set(s => s.LineId, 0)
                                .Set(s => s.StationId, 0)
                                .UpdateAsync();
    }

    public async Task UpdateNameAndStationNo(LineUpdateModel model)
    {
        var line = await GetById(model.LineId);
        if (line is null) throw new Exception("Line is not found");

        var numberOfStation = await _linq2Db.Station.Where(s => s.LineId == line.Id).CountAsync();

        line.LineName = model.LineName;

        if (numberOfStation != model.NumberOfStation)
        {
            // Check line is in use
            if (line.ProductId != 0) throw new Exception("Cannot change number of station: Product exists");

            if (numberOfStation < model.NumberOfStation)
            {
                // Add more station
                await AddStation(line.Id, model.NumberOfStation - numberOfStation, numberOfStation + 1);
            }
            else
            {
                var stationIdsToDelete = await _linq2Db.Station.Where(s => s.LineId == line.Id)
                                                                .OrderBy(s => s.Id)
                                                                .Skip(model.NumberOfStation)
                                                                .Select(s => s.Id)
                                                                .ToListAsync();

                // Delete station
                await _linq2Db.Station.Where(s => stationIdsToDelete.Contains(s.Id))
                                        .DeleteAsync();
            }
        }
        await _linq2Db.Update(line);
    }

    public async Task AddStation(int lineId, int numberOfStation, int index = 1)
    {
        var listStation = new List<Station>();
        for (int i = index; i < numberOfStation + index; i++)
        {
            string stationFormat = i.ToString($"D{_settings.MinCharStationFormat}");

            listStation.Add(new Station
            {
                LineId = lineId,
                StationNo = string.Format(_settings.StationFormat, stationFormat),
            });
        }
        await _linq2Db.BulkInsert(listStation);
    }

}