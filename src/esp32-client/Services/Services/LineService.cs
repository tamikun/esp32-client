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

    public async Task<List<Line>> GetAll()
    {
        return await _linq2Db.Line.ToListAsync();
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

    public async Task<List<GetProcessAndMachineOfLineModel>> GetProcessAndMachineOfLine(int departmentId, int lineId)
    {
        var result = await (from line in _linq2Db.Line.Where(s => s.FactoryId == departmentId && s.Id == lineId)
                            join product1 in _linq2Db.Product on line.ProductId equals product1.Id into product2
                            from product in product2.DefaultIfEmpty()
                            join process1 in _linq2Db.Process on product.Id equals process1.ProductId into process2
                            from process in process2.DefaultIfEmpty()
                            from machine in _linq2Db.Machine.Where(s => s.FactoryId == departmentId &&
                                                                        s.FactoryId != 0 &&
                                                                        s.LineId == lineId &&
                                                                        s.LineId != 0 &&
                                                                        s.StationId == process.Id &&
                                                                        s.StationId != 0).DefaultIfEmpty()
                            // from pattern in _linq2Db.Pattern.Where(s => s.Id == process.PatternId).DefaultIfEmpty()
                            select new GetProcessAndMachineOfLineModel
                            {
                                LineId = line.Id,
                                // LineOrder = line.Order,
                                LineName = line.LineName,
                                ProductId = line.ProductId,
                                ProductName = product.ProductName,
                                ProcessId = process.Id,
                                // ProcessOrder = process.Order,
                                ProcessName = process.ProcessName,
                                MachineId = machine.Id,
                                MachineName = machine.MachineName,
                                MachineIp = machine.IpAddress,
                                // PatternId = pattern.Id,
                                // PatternName = pattern.PatternNumber,
                            }).OrderBy(s => s.ProcessOrder).ToListAsync();

        return result;
    }

    public async Task<List<GetProcessAndMachineOfLineModel>> GetProcessAndMachineOfLines(int departmentId)
    {
        var result = await (from line in _linq2Db.Line.Where(s => s.FactoryId == departmentId)
                            join product1 in _linq2Db.Product on line.ProductId equals product1.Id into product2
                            from product in product2.DefaultIfEmpty()
                            join process1 in _linq2Db.Process on product.Id equals process1.ProductId into process2
                            from process in process2.DefaultIfEmpty()
                            from machine in _linq2Db.Machine.Where(s => s.FactoryId == departmentId &&
                                                                        s.FactoryId != 0 &&
                                                                        s.LineId == line.Id &&
                                                                        s.LineId != 0 &&
                                                                        s.StationId == process.Id &&
                                                                        s.StationId != 0).DefaultIfEmpty()
                            // from pattern in _linq2Db.Pattern.Where(s => s.Id == process.PatternId).DefaultIfEmpty()
                            select new GetProcessAndMachineOfLineModel
                            {
                                LineId = line.Id,
                                // LineOrder = line.Order,
                                LineName = line.LineName,
                                ProductId = line.ProductId,
                                ProductName = product.ProductName,
                                ProcessId = process.Id,
                                // ProcessOrder = process.Order,
                                ProcessName = process.ProcessName,
                                MachineId = machine.Id,
                                MachineName = machine.MachineName,
                                MachineIp = machine.IpAddress,
                                // PatternId = pattern.Id,
                                // PatternName = pattern.PatternNumber,
                            }).OrderBy(s => s.LineOrder).ThenBy(s => s.ProcessOrder).ToListAsync();

        return result;
    }

    public async Task<Line?> GetByLineNo(string lineNo)
    {
        var line = await _linq2Db.Line.Where(s => s.LineNo == lineNo).FirstOrDefaultAsync();
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
        line = await GetByLineNo(line.LineNo) ?? new Line();

        // Create station
        var listStation = new List<Station>();
        for (int i = 0; i < model.NumberOfStation; i++)
        {
            int index = i + 1;
            string stationFormat = index.ToString($"D{_settings.MinCharStationFormat}");

            listStation.Add(new Station
            {
                LineId = line.Id,
                StationNo = string.Format(_settings.StationFormat, stationFormat),
            });
        }
        await _linq2Db.BulkInsert(listStation);

        return line;
    }

    public async Task<Line> Update(LineUpdateModel model)
    {
        var line = await GetById(model.Id);
        if (line is null) throw new Exception("Line is not found");

        // Update line info
        line.FactoryId = model.DepartmentId;
        line.LineName = model.LineName;
        // line.Order = model.Order;
        line.ProductId = model.ProductId;

        await _linq2Db.Update(line);

        return line;
    }

    public async Task Delete(int id)
    {
        var line = await GetById(id);

        // Release machine attached to line
        await _machineService.UpdateMachineLineByProduct(id, 0);

        if (line is not null)
            await _linq2Db.DeleteAsync(line);
    }

}