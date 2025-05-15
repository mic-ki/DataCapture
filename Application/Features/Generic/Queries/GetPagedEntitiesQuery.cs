using Application.Abstraction;
using Application.Abstraction.Mediator;
using Application.Models;
using Application.Services.Mapper;
using System.Text;

namespace Application.Features.Generic.Queries;

/// <summary>
/// Základní abstraktní třída pro stránkovací dotazy entit
/// </summary>
/// <typeparam name="TEntity">Typ entity v databázi</typeparam>
/// <typeparam name="TDto">Typ DTO pro výstup</typeparam>
public abstract class GetPagedEntitiesQuery<TEntity, TDto> : IRequest<Result<PagedList<TDto>>>, ICachableQuery<Result<PagedList<TDto>>>
{
    private int _pageNumber = 1;
    private int _pageSize = 10;

    /// <summary>
    /// Číslo stránky (1 a více)
    /// </summary>
    public int PageNumber 
    { 
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }
    
    /// <summary>
    /// Velikost stránky (1 až 100)
    /// </summary>
    public int PageSize 
    {
        get => _pageSize;
        set => _pageSize = value switch
        {
            < 1 => 10,
            > 100 => 100,
            _ => value
        };
    }

    /// <summary>
    /// Název vlastnosti podle které se má řadit
    /// </summary>
    public string? SortBy { get; set; }
    
    /// <summary>
    /// Příznak pro sestupné řazení
    /// </summary>
    public bool SortDescending { get; set; } = false;
    
    /// <summary>
    /// Implementace ICachableQuery - klíč pro cache
    /// </summary>
    public virtual string CacheKey
    {
        get
        {
            var key = new StringBuilder($"GetPaged_{typeof(TEntity).Name}");
            key.Append($"_Page{PageNumber}");
            key.Append($"_Size{PageSize}");
            
            if (!string.IsNullOrEmpty(SortBy))
            {
                key.Append($"_Sort{SortBy}");
                if (SortDescending)
                    key.Append("Desc");
            }
            
            return key.ToString();
        }
    }

    /// <summary>
    /// Implementace ICachableQuery - tagy pro invalidaci cache
    /// </summary>
    public virtual IEnumerable<string>? Tags => new[] { typeof(TEntity).Name };
}

/// <summary>
/// Obecný handler pro zpracování stránkovacích dotazů
/// </summary>
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
        try
        {
            var query = _context.Set<TEntity>().AsQueryable();
            
            // Aplikace filtrů - přetížit v konkrétních implementacích
            query = ApplyFilters(query, request);
            
            // Aplikace řazení
            try
            {
                query = ApplySorting(query, request);
            }
            catch (Exception ex)
            {
                return await Result<PagedList<TDto>>.ErrorAsync($"Chyba při řazení: {ex.Message}");
            }
            
            // Aplikace eager loadingu - přetížit v konkrétních implementacích
            query = ApplyIncludes(query, request);
            
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
            return await Result<PagedList<TDto>>.ErrorAsync($"Chyba při získávání stránkovaných dat: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Aplikuje filtry na dotaz. Výchozí implementace neaplikuje žádné filtry.
    /// Přetižte tuto metodu v odvozené třídě pro implementaci specifických filtrů.
    /// </summary>
    protected virtual IQueryable<TEntity> ApplyFilters(
        IQueryable<TEntity> query, 
        GetPagedEntitiesQuery<TEntity, TDto> request)
    {
        return query;
    }
    
    /// <summary>
    /// Aplikuje řazení na dotaz podle zadaného názvu vlastnosti
    /// </summary>
    protected virtual IQueryable<TEntity> ApplySorting(
        IQueryable<TEntity> query, 
        GetPagedEntitiesQuery<TEntity, TDto> request)
    {
        // Pokud není zadán název vlastnosti pro řazení, vrátíme dotaz beze změny
        if (string.IsNullOrEmpty(request.SortBy))
            return query;
        
        // Zjistíme, zda entita obsahuje vlastnost s daným názvem
        var propertyInfo = typeof(TEntity).GetProperty(request.SortBy);
        if (propertyInfo == null)
            throw new InvalidOperationException($"Vlastnost '{request.SortBy}' nebyla nalezena na entitě {typeof(TEntity).Name}");
        
        // Vytvoříme lambda výraz pro řazení
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TEntity), "x");
        var property = System.Linq.Expressions.Expression.Property(parameter, propertyInfo);
        var lambda = System.Linq.Expressions.Expression.Lambda(property, parameter);
        
        // Aplikujeme řazení (vzestupné nebo sestupné)
        var methodName = request.SortDescending ? "OrderByDescending" : "OrderBy";
        var resultExp = System.Linq.Expressions.Expression.Call(
            typeof(Queryable),
            methodName,
            new[] { typeof(TEntity), propertyInfo.PropertyType },
            query.Expression,
            System.Linq.Expressions.Expression.Quote(lambda));
        
        return query.Provider.CreateQuery<TEntity>(resultExp);
    }
    
    /// <summary>
    /// Aplikuje eager loading na dotaz. Výchozí implementace nepřidává žádné include.
    /// Přetižte tuto metodu v odvozené třídě pro načtení souvisejících entit.
    /// </summary>
    protected virtual IQueryable<TEntity> ApplyIncludes(
        IQueryable<TEntity> query, 
        GetPagedEntitiesQuery<TEntity, TDto> request)
    {
        return query;
    }
}
