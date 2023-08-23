using AutoMapper;
using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
using esp32_client.Models.Singleton;
using LinqToDB;

namespace esp32_client.Services;

/// <summary>
/// 
/// </summary>
public partial class LineService : ILineService
{

    private readonly LinqToDb _linq2Db;
    private readonly IMachineService _machineService;
    private readonly IStationService _stationService;
    private readonly IMapper _mapper;
    private readonly Settings _settings;

    public LineService(LinqToDb linq2Db, IMapper mapper, IMachineService machineService, Settings settings, IStationService stationService)
    {
        _linq2Db = linq2Db;
        _mapper = mapper;
        _machineService = machineService;
        _settings = settings;
        _stationService = stationService;
    }

    public async Task<Line?> GetById(int id)
    {
        var line = await _linq2Db.Line.Where(s => s.Id == id).FirstOrDefaultAsync();
        return line;
    }

    public async Task<PagedListModel<Line>> GetAll(int pageIndex, int pageSize)
    {
        pageSize = pageSize == 0 ? int.MaxValue : pageSize;
        var lines = await _linq2Db.Line.ToListAsync();
        return lines.ToPagedListModel(pageIndex, pageSize);
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

        line = await _linq2Db.Insert(line);

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
                   .Set(s => s.ProductId, item.ProductId).UpdateQuery();
        }

        // Update process of Station (Set ProcessId to 0)
        var listLineId = listUpdate.Select(s => s.Id);

        var listStationQuery = _linq2Db.Station.Where(s => listLineId.Contains(s.LineId));

        await listStationQuery.Set(s => s.ProcessId, 0).UpdateQuery();

        // Update Pattern for machine (Set LineId = 0, ProcessId to 0 => Delete pattern)
        var listMachineId = _linq2Db.Machine.Where(s => listLineId.Contains(s.LineId)).Select(s => s.Id);

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
                    .Set(s => s.ProcessId, item.ProcessId).UpdateQuery();
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

    public async Task<List<GetProcessAndMachineOfLineModel>> GetProcessAndMachineOfLine(int factoryId, List<int>? listLineId = null,
                                                                bool iotMachine = false, bool hasProduct = false, bool hasMachine = false)
    {
        var lineQuery = _linq2Db.Line.Where(s => s.FactoryId == factoryId);

        if (listLineId?.Count > 0) lineQuery = lineQuery.Where(s => listLineId.Contains(s.Id));
        if (hasProduct) lineQuery = lineQuery.Where(s => s.ProductId != 0);

        var query = (from line in lineQuery
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
                         LineName = line.LineName,
                         LineNo = line.LineNo,
                         StationId = station.Id,
                         StationName = station.StationName,
                         StationNo = station.StationNo,
                         ProductName = product.ProductName,
                         ProductNo = product.ProductNo,
                         ProcessName = process.ProcessName,
                         ProcessNo = process.ProcessNo,
                         PatternNo = process.PatternNo,
                         PatterDescription = process.Description,
                         MachineId = machine.Id,
                         MachineIp = machine.IpAddress,
                         MachineName = machine.MachineName,
                         MachineNo = machine.MachineNo,
                         IoTMachine = machine.IoTMachine,
                     });

        if (hasMachine) query = query.Where(s => s.MachineId != 0);
        if (iotMachine) query = query.Where(s => s.IoTMachine == true);

        query = query.OrderBy(s => s.LineId).ThenBy(s => s.StationId);

        var result = await query.ToListAsync();

        return result;
    }

    public async Task Delete(int lineId)
    {
        var line = await GetById(lineId);
        if (line is null) throw new Exception("Line is not found");

        if (await IsLineInUse(line)) throw new Exception("Cannot delete line: Product exists");

        // Delete line
        await _linq2Db.Delete(line);

        // Delete station
        var stationQuery = await _linq2Db.Station.Where(s => s.LineId == line.Id).ToListAsync();

        await _stationService.DeleteListStation(stationQuery);
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
            if (await IsLineInUse(line)) throw new Exception("Cannot change number of station: Product exists");

            if (numberOfStation < model.NumberOfStation)
            {
                // Add more station
                await AddStation(line.Id, model.NumberOfStation - numberOfStation, numberOfStation + 1);
            }
            else
            {
                var stationDelete = await _linq2Db.Station.Where(s => s.LineId == line.Id)
                                                                .OrderBy(s => s.Id)
                                                                .Skip(model.NumberOfStation)
                                                                .ToListAsync();

                // Delete station
                await _stationService.DeleteListStation(stationDelete);
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

    public async Task<bool> IsLineInUse(Line line)
    {
        await Task.CompletedTask;
        return line.ProductId != 0;
    }

}