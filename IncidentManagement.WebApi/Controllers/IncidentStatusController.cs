using IncidentManagement.Application.Dtos.Responses;
using IncidentManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IncidentManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class IncidentStatusController(IncidentDbContext db) : ControllerBase
{
    [HttpGet("all")]
    public async Task<ActionResult<List<IncidentStatusResponse>>> GetAll() =>
        Ok(await db.IncidentStatuses.AsNoTracking().Select(x =>
            new IncidentStatusResponse(x.StatusId, x.StatusName ?? string.Empty)).ToListAsync());
}
