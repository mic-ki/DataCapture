using Application.Features.Generic;
using Application.Features.Generic.Queries;
using Application.Features.Sample;
using Application.Models;
using Domain;

namespace Application.Features.Sample.Queries;

public class GetAllSamplesQuery : GetAllEntitiesQuery<SampleEntity, SampleDto>, ICachableQuery<Result<List<SampleDto>>>
{
    // Implementace ICachableQuery
    public string CacheKey => $"GetAll_{typeof(SampleEntity).Name}";
    public IEnumerable<string>? Tags => new[] { typeof(SampleEntity).Name };
}

// Handler je již implementován genericky v GetAllEntitiesQueryHandler
