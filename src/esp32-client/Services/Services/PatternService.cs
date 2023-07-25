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
public partial class PatternService : IPatternService
{

    private readonly LinqToDb _linq2Db;
    private readonly IMapper _mapper;

    public PatternService(LinqToDb linq2Db, IMapper mapper)
    {
        _linq2Db = linq2Db;
        _mapper = mapper;
    }

    public async Task<Pattern?> GetById(int id)
    {
        var pattern = await _linq2Db.Pattern.Where(s => s.Id == id).FirstOrDefaultAsync();
        return pattern;
    }

    public async Task<List<PatternResponseModel>> GetAll()
    {
        var result = await (from pattern in _linq2Db.Pattern
                            select new PatternResponseModel()
                            {
                                Id = pattern.Id,
                                PatternNumber = pattern.PatternNumber,
                                FileName = pattern.FileName,
                                Description = pattern.Description,
                            }).ToListAsync();
        return result;
    }

    public async Task<Pattern> Create(PatternCreateModel model)
    {
        var pattern = _mapper.Map<Pattern>(model);
        pattern.FileData = Convert.ToBase64String(Utils.Utils.GetBytesFromFile(model.File));

        await _linq2Db.InsertAsync(pattern);

        return pattern;
    }

    public async Task<Pattern> Update(PatternUpdateModel model)
    {
        var pattern = _mapper.Map<Pattern>(model);
        pattern.Id = model.Id;
        pattern.FileData = Convert.ToBase64String(Utils.Utils.GetBytesFromFile(model.File));

        await _linq2Db.UpdateAsync(pattern);

        return pattern;
    }

    public async Task Delete(int id)
    {
        var pattern = await GetById(id);
        if (pattern is not null)
            await _linq2Db.DeleteAsync(pattern);
    }

    public async Task Delete(List<int> listId)
    {
        await _linq2Db.Pattern.Where(s => listId.Contains(s.Id)).DeleteAsync();
    }



}