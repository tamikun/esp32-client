namespace esp32_client.Services
{
    public interface ILogService
    {
        Task AddLog(Exception ex);
    }
}