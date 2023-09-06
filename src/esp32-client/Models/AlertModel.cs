namespace esp32_client.Models;

public class AlertModel
{
    public Alert AlertType { get; set; }

    public string AlertMessage { get; set; }
}

public enum Alert
{
    None = 0,
    Primary = 1,
    Secondary = 2,
    Info = 3,
    Success = 4,
    Warning = 5,
    Danger = 6
}