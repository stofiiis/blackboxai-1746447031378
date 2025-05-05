using System.Diagnostics;
using System.Net.Http.Json;
using RodicovskaKontrola.Client.Models;

namespace RodicovskaKontrola.Client.Services;

public class MonitoringService
{
    private readonly HttpClient _httpClient;
    private readonly string _childId;
    private readonly Timer _statusUpdateTimer;
    private readonly Timer _commandCheckTimer;
    private bool _isRunning = true;

    public event EventHandler<int>? TimeUpdated;
    public event EventHandler? ShutdownRequested;

    private int _timeRemainingInSeconds;
    public int TimeRemainingInSeconds
    {
        get => _timeRemainingInSeconds;
        private set
        {
            _timeRemainingInSeconds = value;
            TimeUpdated?.Invoke(this, value);
        }
    }

    public MonitoringService(string apiUrl, string childId, int initialTimeLimit)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(apiUrl) };
        _childId = childId;
        TimeRemainingInSeconds = initialTimeLimit * 60; // Convert minutes to seconds

        // Timer for updating status every 30 seconds
        _statusUpdateTimer = new Timer(UpdateStatus, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));

        // Timer for checking commands every 5 seconds
        _commandCheckTimer = new Timer(CheckCommands, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
    }

    private async void UpdateStatus(object? state)
    {
        if (!_isRunning) return;

        try
        {
            var processes = GetRunningProcesses();
            var status = new ChildStatus
            {
                ChildId = _childId,
                TimeRemainingInSeconds = TimeRemainingInSeconds,
                ActiveProcesses = string.Join(",", processes),
                Timestamp = DateTime.UtcNow
            };

            var response = await _httpClient.PostAsJsonAsync("api/child/update", status);
            response.EnsureSuccessStatusCode();

            // Decrease remaining time
            if (TimeRemainingInSeconds > 0)
            {
                TimeRemainingInSeconds -= 30; // 30 seconds have passed
                if (TimeRemainingInSeconds <= 0)
                {
                    await ShutdownComputer();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating status: {ex.Message}");
        }
    }

    private async void CheckCommands(object? state)
    {
        if (!_isRunning) return;

        try
        {
            var commands = await _httpClient.GetFromJsonAsync<List<ControlCommand>>($"api/child/command?childId={_childId}");
            if (commands == null) return;

            foreach (var command in commands)
            {
                await ProcessCommand(command);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking commands: {ex.Message}");
        }
    }

    private async Task ProcessCommand(ControlCommand command)
    {
        switch (command.CommandType.ToLower())
        {
            case "shutdown":
                await ShutdownComputer();
                break;

            case "adjusttime":
                if (int.TryParse(command.Parameter, out int minutes))
                {
                    TimeRemainingInSeconds = minutes * 60;
                }
                break;

            case "cancellimit":
                TimeRemainingInSeconds = int.MaxValue;
                break;
        }
    }

    private List<string> GetRunningProcesses()
    {
        return Process.GetProcesses()
            .Where(p => !string.IsNullOrEmpty(p.MainWindowTitle)) // Only processes with UI
            .Select(p => p.ProcessName)
            .Distinct()
            .ToList();
    }

    private async Task ShutdownComputer()
    {
        _isRunning = false;
        ShutdownRequested?.Invoke(this, EventArgs.Empty);
        
        // In production, this would actually shut down the computer
        // Process.Start("shutdown", "/s /t 60");
    }

    public void Stop()
    {
        _isRunning = false;
        _statusUpdateTimer.Dispose();
        _commandCheckTimer.Dispose();
        _httpClient.Dispose();
    }
}
