using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
using LinqToDB;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace esp32_client.Services;

/// <summary>
/// 
/// </summary>
public partial class PaternService : IPaternService
{

    private readonly LinqToDb _linq2Db;
    private readonly IMapper _mapper;

    public PaternService(LinqToDb linq2Db, IMapper mapper)
    {
        _linq2Db = linq2Db;
        _mapper = mapper;
    }

    public async Task<List<PaternResponseModel>> GetAll()
    {
        var result = await (from patern in _linq2Db.Patern
                            select new PaternResponseModel()
                            {
                                PaternNumber = patern.PaternNumber,
                                FileName = patern.FileName,
                                Description = patern.Description,
                            }).ToListAsync();
        return result;
    }

    public async Task<Patern> Create(PaternCreateModel model)
    {
        var patern = _mapper.Map<Patern>(model);
        patern.FileData = Convert.ToBase64String(Utils.Utils.GetBytesFromFile(model.File));

        await _linq2Db.InsertAsync(patern);

        return patern;
    }



}