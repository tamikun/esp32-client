using esp32_client.Domain;
using esp32_client.Models;

namespace esp32_client.Services
{
    public interface IPatternService
    {
        Task<List<PatternResponseModel>> GetAll();

        Task<Pattern> Create(PatternCreateModel model);

        Task<Pattern?> GetById(int id);

        Task Delete(int id);

        Task Delete(List<int> listId);
    }
}