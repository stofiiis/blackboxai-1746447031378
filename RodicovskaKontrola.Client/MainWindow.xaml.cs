using System;
using System.Windows;
using RodicovskaKontrola.Client.Services;

namespace RodicovskaKontrola.Client;

public partial class MainWindow : Window
{
    private readonly MonitoringService _monitoringService;
    private readonly System.Windows.Threading.DispatcherTimer _uiUpdateTimer;

    public MainWindow()
    {
        InitializeComponent();

        // Configuration
        const string ApiUrl = "https://localhost:7001";
        const string ChildId = "1";
        const int InitialTimeLimit = 120; // 2 hours in minutes

        try
        {
            _monitoringService = new MonitoringService(ApiUrl, ChildId, InitialTimeLimit);

            // Set up UI update timer
            _uiUpdateTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _uiUpdateTimer.Tick += UpdateUI;
            _uiUpdateTimer.Start();

            // Subscribe to monitoring service events
            _monitoringService.TimeUpdated += OnTimeUpdated;
            _monitoringService.ShutdownRequested += OnShutdownRequested;

            // Set initial progress bar maximum
            TimeProgressBar.Maximum = InitialTimeLimit * 60;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Chyba při spouštění aplikace: {ex.Message}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
        }
    }

    private void UpdateUI(object? sender, EventArgs e)
    {
        var timeRemaining = TimeSpan.FromSeconds(_monitoringService.TimeRemainingInSeconds);
        TimeRemainingText.Text = $"{timeRemaining.Hours:D2}:{timeRemaining.Minutes:D2}:{timeRemaining.Seconds:D2}";
        TimeProgressBar.Value = _monitoringService.TimeRemainingInSeconds;
    }

    private void OnTimeUpdated(object? sender, int remainingSeconds)
    {
        Dispatcher.Invoke(() =>
        {
            var timeSpan = TimeSpan.FromSeconds(remainingSeconds);
            TimeRemainingText.Text = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            TimeProgressBar.Value = remainingSeconds;
        });
    }

    private void OnShutdownRequested(object? sender, EventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            MessageBox.Show("Čas vypršel. Počítač bude vypnut.", "Upozornění", MessageBoxButton.OK, MessageBoxImage.Warning);
            Close();
        });
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        _monitoringService.Stop();
        _uiUpdateTimer.Stop();
    }
}
