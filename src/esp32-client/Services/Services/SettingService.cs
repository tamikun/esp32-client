using AutoMapper;
using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models.Singleton;
using LinqToDB;

namespace esp32_client.Services;

/// <summary>
/// 
/// </summary>
public partial class SettingService : ISettingService
{

    private readonly LinqToDb _linq2Db;
    private readonly Settings _settings;

    public SettingService(LinqToDb linq2Db, Settings settings)
    {
        _linq2Db = linq2Db;
        _settings = settings;
    }

    public async Task<List<Setting>> GetAll()
    {
        return await _linq2Db.Setting.ToListAsync();
    }

    public async Task<PagedListModel<Setting>> GetAll(int pageIndex, int pageSize, bool? isEditable = null)
    {
        if (pageSize == 0) pageSize = int.MaxValue;
        var query = _linq2Db.Setting.AsQueryable();
        if (isEditable is not null) query = query.Where(s => s.EnableEditing == isEditable);
        var result = await query.ToPagedListModel(pageIndex, pageSize);
        return result;
    }

    public async Task UpdateListSetting(List<Setting> model)
    {
        if (model.Count == 0) return;

        foreach (var item in model)
        {
            await _linq2Db.Setting.Where(s => s.Id == item.Id).Set(s => s.Value, item.Value).UpdateQuery();
        }

        _settings.Reload();
    }

}