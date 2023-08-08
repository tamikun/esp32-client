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

    public async Task AssignProductLine(AssignProductLineModel model)
    {
        foreach (var item in model.ListProductLine)
        {
            await _linq2Db.Line.Where(s => s.Id == item.LineId)
                       .Set(s => s.ProductId, item.ProductId)
                       .UpdateAsync();
        }
    }
    
    public async Task AssignStationProcess(AssignStationProcessModel model)
    {
        foreach (var item in model.ListStationProcess)
        {
            await _linq2Db.Station.Where(s => s.Id == item.StationId)
                       .Set(s => s.ProcessId, item.ProcessId)
                       .UpdateAsync();
        }
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
                            // where process.ProductId == product.Id
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

}