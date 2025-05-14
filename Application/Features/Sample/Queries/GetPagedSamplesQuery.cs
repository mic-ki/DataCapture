using Application.Abstraction;
using Application.Features.Generic.Queries;
using Application.Features.Sample;
using Application.Services.Mapper;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Sample.Queries;

public class GetPagedSamplesQuery : GetPagedEntitiesQuery<SampleEntity, SampleDto>
{
    public string? NameFilter { get; set; }
    
    // Přepsání CacheKey pro zahrnutí filtru
    public override string CacheKey => $"{base.CacheKey}_{NameFilter}";
}
// TODO: Tohle vyžaduje větší úpravu, bacha na mapper a kontext.
public class GetPagedSamplesQueryHandler : GetPagedEntitiesQueryHandler<SampleEntity, SampleDto>
{
    public GetPagedSamplesQueryHandler(
        IApplicationDbContext context, 
        IMapper<SampleEntity, SampleDto> mapper) 
        : base(context, mapper)
    {
    }
    
    protected override IQueryable<SampleEntity> ApplyFilters(
        IQueryable<SampleEntity> query, 
        GetPagedEntitiesQuery<SampleEntity, SampleDto> request)
    {
        // Základní filtrování z base třídy
        query = base.ApplyFilters(query, request);
        
        // Specifické filtrování pro SampleEntity
        if (request is GetPagedSamplesQuery samplesQuery && !string.IsNullOrEmpty(samplesQuery.NameFilter))
        {
            query = query.Where(e => e.Name != null && e.Name.Contains(samplesQuery.NameFilter));
        }
        
        return query;
    }
    
    protected override IQueryable<SampleEntity> ApplyIncludes(
        IQueryable<SampleEntity> query, 
        GetPagedEntitiesQuery<SampleEntity, SampleDto> request)
    {
        // Základní eager loading z base třídy
        query = base.ApplyIncludes(query, request);
        
        // Specifický eager loading pro SampleEntity
        // Například:
        // query = query.Include(e => e.RelatedEntities);
        
        return query;
    }
}
