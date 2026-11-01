
using System.Security.Claims;
using IncidentManagement.Application.Dtos.Requests;
using IncidentManagement.Application.Dtos.Responses;
using IncidentManagement.Domain.Entities;
using IncidentManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IncidentManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class IncidentController(IncidentDbContext db) : ControllerBase
{
    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("create")]
    public async Task<IActionResult> CreateIncident([FromBody] CreateIncidentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title)) return BadRequest(new { message = "Title is required." });

        var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == CurrentUserId && x.IsActive);

        if (user is null) return Unauthorized();

        var refsExist = await db.IncidentCategories.AnyAsync(x => x.CategoryId == request.CategoryId) &&
                        await db.IncidentSeverities.AnyAsync(x => x.SeverityId == request.SeverityId) &&
                        await db.IncidentStatuses.AnyAsync(x => x.StatusId == request.StatusId) &&
                        await db.Priorities.AnyAsync(x => x.PriorityId == request.PriorityId);

        if (!refsExist) return BadRequest(new { message = "One or more selected incident values are invalid." });

        var incident = new Incident(request.Title.Trim(), request.Description ?? string.Empty,
            request.CategoryId, request.SeverityId, request.StatusId, request.PriorityId,
            user.UserId, user.PropertyId);

        db.Incidents.Add(incident);
        await db.SaveChangesAsync();

        return Ok(await GetResponse(incident.IncidentId));
    }

    [HttpPut("status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateIncidentStatusRequest request)
    {
        if (!await IsActiveAdministrator()) return Unauthorized();

        var incident = await db.Incidents.FirstOrDefaultAsync(x => x.IncidentId == request.IncidentId);

        if (incident is null) return NotFound(new { message = "Incident not found." });

        if (!await db.IncidentStatuses.AnyAsync(x => x.StatusId == request.StatusId))
            return BadRequest(new { message = "Invalid incident status." });

        incident.UpdateStatus(request.StatusId);
        await db.SaveChangesAsync();

        return Ok(await GetResponse(incident.IncidentId));
    }

    [HttpGet("my")]
    public async Task<ActionResult<List<IncidentResponse>>> GetMyIncidents()
    {
        var userId = CurrentUserId;

        if (!await db.Users.AnyAsync(u => u.UserId == userId && u.IsActive))
            return Unauthorized();

        // Filter incidents before creating the response projection.
        return Ok(await QueryResponses(userId)
            .Select(x => new IncidentResponse(
                x.IncidentId,
                x.Title,
                x.Description,
                x.Username,
                x.CategoryName,
                x.SeverityName,
                x.StatusName,
                x.PriorityName,
                x.ReporterName,
                x.PropertyName,
                x.CreatedAt,
                x.UpdatedAt))
            .ToListAsync());
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<IncidentResponse>>> GetAllIncidents()
    {
        if (!await IsActiveAdministrator()) return Unauthorized();

        return Ok(await QueryResponses()
            .Select(x => new IncidentResponse(
                x.IncidentId,
                x.Title,
                x.Description,
                x.Username,
                x.CategoryName,
                x.SeverityName,
                x.StatusName,
                x.PriorityName,
                x.ReporterName,
                x.PropertyName,
                x.CreatedAt,
                x.UpdatedAt))
            .ToListAsync());
    }

    [HttpGet("property/{propertyId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<IncidentResponse>>> GetByProperty(int propertyId)
    {
        if (!await IsActiveAdministrator()) return Unauthorized();

        return Ok(await QueryResponses()
            .Where(x => x.PropertyId == propertyId)
            .Select(x => new IncidentResponse(
                x.IncidentId,
                x.Title,
                x.Description,
                x.Username,
                x.CategoryName,
                x.SeverityName,
                x.StatusName,
                x.PriorityName,
                x.ReporterName,
                x.PropertyName,
                x.CreatedAt,
                x.UpdatedAt))
            .ToListAsync());
    }

    private async Task<bool> IsActiveAdministrator()
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
            return false;

        return await db.Users.AnyAsync(u => u.UserId == id && u.IsActive &&
            u.Role != null && u.Role.RoleName == "Admin");
    }

    // Optional reporterId allows the current user's incidents to be filtered
    // before the response projection is created.
    private IQueryable<IncidentResponseRow> QueryResponses(int? reporterId = null)
    {
        var query = db.Incidents.AsNoTracking()
            .Where(x => reporterId == null || x.UserId == reporterId.Value)
            .Select(x => new IncidentResponseRow(
                x.IncidentId,
                x.Title,
                x.Description,
                x.User.Username ?? string.Empty,
                x.Category.CategoryName ?? string.Empty,
                x.Severity.SeverityName ?? string.Empty,
                x.Status.StatusName ?? string.Empty,
                x.Priority.PriorityName ?? string.Empty,
                x.User.Username ?? string.Empty,
                x.Property.PropertyName ?? string.Empty,
                x.CreatedAt,
                x.UpdatedAt,
                x.PropertyId,
                x.UserId
            ));

        return query;
    }

    private async Task<IncidentResponse> GetResponse(int incidentId)
    {
        var x = await db.Incidents
            .AsNoTracking()
            .Where(i => i.IncidentId == incidentId)
            .Select(i => new IncidentResponse(
                i.IncidentId,
                i.Title,
                i.Description,
                i.User.Username ?? string.Empty,
                i.Category.CategoryName ?? string.Empty,
                i.Severity.SeverityName ?? string.Empty,
                i.Status.StatusName ?? string.Empty,
                i.Priority.PriorityName ?? string.Empty,
                i.User.Username ?? string.Empty,
                i.Property.PropertyName ?? string.Empty,
                i.CreatedAt,
                i.UpdatedAt
            ))
            .FirstAsync();

        return x;
    }

    private sealed record IncidentResponseRow(
        int IncidentId,
        string Title,
        string Description,
        string Username,
        string CategoryName,
        string SeverityName,
        string StatusName,
        string PriorityName,
        string ReporterName,
        string PropertyName,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        int PropertyId,
        int ReporterId);
}

