using IncidentManagement.Application.Dtos.Responses;
using IncidentManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IncidentManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class IncidentRoleController(IncidentDbContext db) : ControllerBase
{
    [HttpGet("all")]
    public async Task<ActionResult<List<IncidentRoleResponse>>> GetAll() =>
        Ok(await db.Roles.AsNoTracking().Select(x =>
            new IncidentRoleResponse(x.RoleId, x.RoleName ?? string.Empty)).ToListAsync());
}
