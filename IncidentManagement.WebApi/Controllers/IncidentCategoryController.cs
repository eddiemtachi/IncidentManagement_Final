using IncidentManagement.Application.Dtos.Responses;
using IncidentManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IncidentManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class IncidentCategoryController(IncidentDbContext db) : ControllerBase
{
    [HttpGet("all")]
    public async Task<ActionResult<List<IncidentCategoryResponse>>> GetAll() =>
        Ok(await db.IncidentCategories.AsNoTracking().Select(x =>
            new IncidentCategoryResponse(x.CategoryId, x.CategoryName ?? string.Empty)).ToListAsync());
}
