using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Services
{
    public interface ILineService
    {
        Task<Line?> GetById(int id);

        Task<List<Line>> GetAll();

        Task<List<LineResponseModel>> GetAllLineResponse(int departmentId);

        Task<Line> Create(LineCreateModel model);

        Task<Line> Update(LineUpdateModel model);

        Task Delete(int id);
    }
}