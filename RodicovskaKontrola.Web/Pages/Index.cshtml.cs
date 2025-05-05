using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RodicovskaKontrola.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        // Page load logic can be added here if needed
    }
}
