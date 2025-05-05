using System;
using System.Collections.Generic;

namespace RodicovskaKontrola.API.Models;

public class ChildStatus
{
    public int Id { get; set; }
    public string ChildId { get; set; } = string.Empty;
    public int TimeRemainingInSeconds { get; set; }
    public string ActiveProcesses { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
