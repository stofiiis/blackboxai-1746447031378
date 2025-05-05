using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RodicovskaKontrola.API.Data;
using RodicovskaKontrola.API.Models;

namespace RodicovskaKontrola.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ControlController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ControlController> _logger;

    public ControlController(ApplicationDbContext context, ILogger<ControlController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("command")]
    public async Task<IActionResult> CreateCommand(ControlCommand command)
    {
        try
        {
            command.Timestamp = DateTime.UtcNow;
            command.Status = "Pending";
            _context.ControlCommands.Add(command);
            await _context.SaveChangesAsync();
            return Ok(command);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating control command");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("status/{childId}")]
    public async Task<IActionResult> GetChildStatus(string childId)
    {
        try
        {
            var status = await _context.ChildStatuses
                .Where(s => s.ChildId == childId)
                .OrderByDescending(s => s.Timestamp)
                .FirstOrDefaultAsync();

            if (status == null)
            {
                return NotFound($"No status found for child {childId}");
            }

            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving child status");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("processes/{childId}")]
    public async Task<IActionResult> GetRunningProcesses(string childId)
    {
        try
        {
            var status = await _context.ChildStatuses
                .Where(s => s.ChildId == childId)
                .OrderByDescending(s => s.Timestamp)
                .FirstOrDefaultAsync();

            if (status == null)
            {
                return NotFound($"No status found for child {childId}");
            }

            return Ok(status.ActiveProcesses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving running processes");
            return StatusCode(500, "Internal server error");
        }
    }
}
