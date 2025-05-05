using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RodicovskaKontrola.API.Data;
using RodicovskaKontrola.API.Models;

namespace RodicovskaKontrola.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChildController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ChildController> _logger;

    public ChildController(ApplicationDbContext context, ILogger<ChildController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateStatus(ChildStatus status)
    {
        try
        {
            status.Timestamp = DateTime.UtcNow;
            _context.ChildStatuses.Add(status);
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating child status");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("command")]
    public async Task<IActionResult> GetPendingCommands(string childId)
    {
        try
        {
            var commands = await _context.ControlCommands
                .Where(c => c.ChildId == childId && c.Status == "Pending")
                .ToListAsync();

            foreach (var command in commands)
            {
                command.Status = "Executed";
            }
            
            await _context.SaveChangesAsync();
            return Ok(commands);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending commands");
            return StatusCode(500, "Internal server error");
        }
    }
}
