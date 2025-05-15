using Application.Features.Generic;
using Application.Features.Generic.Queries;
using Application.Features.Sample;
using Application.Models;
using Domain;

namespace Application.Features.Sample.Queries;

/// <summary>
/// Dotaz pro získání seznamu všech vzorových entit
/// </summary>
public class GetAllSamplesQuery : GetAllEntitiesQuery<SampleEntity, SampleDto>, ICachableQuery<Result<List<SampleDto>>>
{
    // Implementace ICachableQuery - přepíšeme výchozí implementaci
    public string CacheKey => $"GetAll_{typeof(SampleEntity).Name}";
    public IEnumerable<string>? Tags => new[] { typeof(SampleEntity).Name };
}

// Handler je již implementován genericky v GetAllEntitiesQueryHandler
