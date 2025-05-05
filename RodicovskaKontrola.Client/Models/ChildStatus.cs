namespace RodicovskaKontrola.Client.Models;

public class ChildStatus
{
    public string ChildId { get; set; } = string.Empty;
    public int TimeRemainingInSeconds { get; set; }
    public string ActiveProcesses { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class ControlCommand
{
    public string ChildId { get; set; } = string.Empty;
    public string CommandType { get; set; } = string.Empty;
    public string? Parameter { get; set; }
    public DateTime Timestamp { get; set; }
    public string Status { get; set; } = "Pending";
}
