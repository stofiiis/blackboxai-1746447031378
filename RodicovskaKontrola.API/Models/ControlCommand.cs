using System;

namespace RodicovskaKontrola.API.Models;

public class ControlCommand
{
    public int Id { get; set; }
    public string ChildId { get; set; } = string.Empty;
    public string CommandType { get; set; } = string.Empty;
    public string? Parameter { get; set; }
    public DateTime Timestamp { get; set; }
    public string Status { get; set; } = "Pending";
}
