using IncidentManagement.Application.Dtos.Responses;
using IncidentManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IncidentManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class IncidentPropertyController(IncidentDbContext db) : ControllerBase
{
    [HttpGet("all")]
    public async Task<ActionResult<List<IncidentPropertyResponse>>> GetAll() =>
        Ok(await db.Properties.AsNoTracking().Select(x =>
            new IncidentPropertyResponse(x.PropertyId, x.PropertyName ?? string.Empty)).ToListAsync());
}
