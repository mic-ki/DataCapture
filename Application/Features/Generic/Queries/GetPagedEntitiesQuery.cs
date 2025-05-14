using Application.Abstraction;
using Application.Abstraction.Mediator;
using Application.Exceptions;
using Application.Features.Generic;
using Application.Models;
using Application.Services;
using Application.Services.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Generic.Queries;

public abstract class GetPagedEntitiesQuery<TEntity, TDto> : IRequest<Result<PagedList<TDto>>>, ICachableQuery<Result<PagedList<TDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
    
    // Implementace ICachableQuery
    public virtual string CacheKey => $"GetPaged_{typeof(TEntity).Name}_{PageNumber}_{PageSize}_{SortBy}_{SortDescending}";
    public virtual IEnumerable<string>? Tags => new[] { typeof(TEntity).Name };
}

public class GetPagedEntitiesQueryHandler<TEntity, TDto>
    : IRequestHandler<GetPagedEntitiesQuery<TEntity, TDto>, Result<PagedList<TDto>>>
    where TEntity : class
    where TDto : class, new()
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper<TEntity, TDto> _mapper;

    public GetPagedEntitiesQueryHandler(IApplicationDbContext context, IMapper<TEntity, TDto> mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PagedList<TDto>>> Handle(
        GetPagedEntitiesQuery<TEntity, TDto> request, 
        CancellationToken cancellationToken)
    {
        var query = _context.Set<TEntity>().AsQueryable();
        
        // Aplikace filtrů - přetížit v konkrétních implementacích
        query = ApplyFilters(query, request);
        
        // Aplikace řazení
        query = ApplySorting(query, request);
        
        // Aplikace eager loadingu - přetížit v konkrétních implementacích
        query = ApplyIncludes(query, request);
        
        try
        {
            var pagedList = await PagedList<TDto>.CreateAsync(
                query,
                request.PageNumber,
                request.PageSize,
                entity => _mapper.MapDirect(entity),
                cancellationToken);
            
            return await Result<PagedList<TDto>>.OkAsync(pagedList);
        }
        catch (Exception ex)
        {
            return await Result<PagedList<TDto>>.ErrorAsync(ex.Message);
        }
    }
    
    protected virtual IQueryable<TEntity> ApplyFilters(
        IQueryable<TEntity> query, 
        GetPagedEntitiesQuery<TEntity, TDto> request)
    {
        // Základní implementace bez filtrů
        // Přetížit v konkrétních implementacích pro přidání filtrů
        return query;
    }
    
    protected virtual IQueryable<TEntity> ApplySorting(
        IQueryable<TEntity> query, 
        GetPagedEntitiesQuery<TEntity, TDto> request)
    {
        // Základní implementace řazení
        if (string.IsNullOrEmpty(request.SortBy))
            return query;
        
        // Dynamické řazení podle názvu vlastnosti
        // Toto je zjednodušená implementace, v reálném projektu by bylo potřeba
        // ošetřit více případů a možná použít Expression Trees pro typově bezpečné řazení
        var propertyInfo = typeof(TEntity).GetProperty(request.SortBy);
        if (propertyInfo == null)
            return query;
        
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TEntity), "x");
        var property = System.Linq.Expressions.Expression.Property(parameter, propertyInfo);
        var lambda = System.Linq.Expressions.Expression.Lambda(property, parameter);
        
        var methodName = request.SortDescending ? "OrderByDescending" : "OrderBy";
        var resultExp = System.Linq.Expressions.Expression.Call(
            typeof(Queryable),
            methodName,
            new[] { typeof(TEntity), propertyInfo.PropertyType },
            query.Expression,
            System.Linq.Expressions.Expression.Quote(lambda));
        
        return query.Provider.CreateQuery<TEntity>(resultExp);
    }
    
    protected virtual IQueryable<TEntity> ApplyIncludes(
        IQueryable<TEntity> query, 
        GetPagedEntitiesQuery<TEntity, TDto> request)
    {
        // Základní implementace bez eager loadingu
        // Přetížit v konkrétních implementacích pro přidání Include
        return query;
    }
}
