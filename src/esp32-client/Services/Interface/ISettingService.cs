using esp32_client.Builder;
using esp32_client.Domain;

namespace esp32_client.Services
{
    public interface ISettingService
    {
        Task<List<Setting>> GetAll();

        Task<PagedListModel<Setting>> GetAll(int pageIndex, int pageSize, bool? isEditable = null);

        Task UpdateListSetting(List<Setting> model);

    }
}