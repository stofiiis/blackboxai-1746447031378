using RodicovskaKontrola.Client.Services;

// Configuration
const string ApiUrl = "https://localhost:7001"; // Update with your API URL
const string ChildId = "1"; // In production, this would be configured or generated
const int InitialTimeLimit = 120; // 2 hours in minutes

try
{
    Console.WriteLine("Spouštění monitorovací služby...");
    Console.WriteLine($"ID dítěte: {ChildId}");
    Console.WriteLine($"Počáteční časový limit: {InitialTimeLimit} minut");
    
    var monitoringService = new MonitoringService(ApiUrl, ChildId, InitialTimeLimit);

    Console.WriteLine("\nSlužba běží. Stiskněte libovolnou klávesu pro ukončení.");
    Console.WriteLine("Časový limit a běžící aplikace se aktualizují automaticky.");
    
    // Keep the application running until a key is pressed
    Console.ReadKey();
    
    monitoringService.Stop();
    Console.WriteLine("\nSlužba byla ukončena.");
}
catch (Exception ex)
{
    Console.WriteLine($"Došlo k chybě: {ex.Message}");
    Console.WriteLine("Stiskněte libovolnou klávesu pro ukončení.");
    Console.ReadKey();
}
