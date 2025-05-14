using Application.Abstraction.Mediator;
using Application.Features.Sample;
using Application.Features.Sample.Queries;
using Microsoft.AspNetCore.Mvc;

namespace DataCapture.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SampleController : ControllerBase
{
    private readonly IMediator _mediator;

    public SampleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        // Použití GetAllEntitiesQuery pro malé datasety
        var query = new GetAllSamplesQuery();
        var result = await _mediator.Send(query, cancellationToken);
        
        if (result.Succeeded)
            return Ok(result.Data);
        
        return BadRequest(result);
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] string? nameFilter = null,
        CancellationToken cancellationToken = default)
    {
        // Použití GetPagedEntitiesQuery pro větší datasety
        var query = new GetPagedSamplesQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDescending = sortDescending,
            NameFilter = nameFilter
        };
        
        var result = await _mediator.Send(query, cancellationToken);
        
        if (result.Succeeded)
            return Ok(result.Data);
        
        return BadRequest(result);
    }
}
