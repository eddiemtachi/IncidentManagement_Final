using IncidentManagement.Application.Dtos.Responses;
using IncidentManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IncidentManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class IncidentPriorityController(IncidentDbContext db) : ControllerBase
{
    [HttpGet("all")]
    public async Task<ActionResult<List<IncidentPriorityResponse>>> GetAll() =>
        Ok(await db.Priorities.AsNoTracking().Select(x =>
            new IncidentPriorityResponse(x.PriorityId, x.PriorityName ?? string.Empty)).ToListAsync());
}
