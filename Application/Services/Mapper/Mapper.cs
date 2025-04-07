using System.Reflection;

namespace Application.Services.Mapper;

using System.Linq.Expressions;

public interface IMapper<in TSource, out TTarget>
    where TSource : class
    where TTarget : class, new()
{
    TTarget Map(TSource source);
    TTarget MapDirect(TSource source);
    IEnumerable<TTarget> MapCollection(IEnumerable<TSource> sources);
    IEnumerable<TTarget> MapDirectCollection(IEnumerable<TSource> sources);
}

public class Mapper<TSource, TTarget> : IMapper<TSource, TTarget>
    where TSource : class
    where TTarget : class, new()
{
    private readonly Func<TSource, TTarget> _configuredMapper;
    private static readonly Lazy<Func<TSource, TTarget>> _directMapperLazy = new(
        BuildDirectMapper, 
        LazyThreadSafetyMode.ExecutionAndPublication
    );
    private static readonly Lazy<Func<IEnumerable<TSource>, List<TTarget>>> _directCollectionMapperLazy = new(
        () => BuildCollectionMapper(_directMapperLazy.Value)
    );
    
    public Mapper(Action<MappingConfig<TSource, TTarget>> configAction)
    {
        var config = new MappingConfig<TSource, TTarget>();
        configAction?.Invoke(config);
        _configuredMapper = config.Compile();
    }

    public TTarget Map(TSource source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return _configuredMapper(source);
    }

    public TTarget MapDirect(TSource source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return _directMapperLazy.Value(source);
    }

    // Mapování kolekcí s konfigurací
    public IEnumerable<TTarget> MapCollection(IEnumerable<TSource> sources)
    {
        if (sources == null) throw new ArgumentNullException(nameof(sources));
        return sources.Select(_configuredMapper);
    }

    // Automatické mapování kolekcí
    public IEnumerable<TTarget> MapDirectCollection(IEnumerable<TSource> sources)
    {
        if (sources == null) throw new ArgumentNullException(nameof(sources));
        return _directCollectionMapperLazy.Value(sources);
    }
    private static Func<TSource, TTarget> BuildDirectMapper()
    {
        var sourceParam = Expression.Parameter(typeof(TSource), "src");
        var target = Expression.Variable(typeof(TTarget), "trg");
        
        var assignments = new List<Expression>
        {
            Expression.Assign(target, Expression.New(typeof(TTarget)))
        };

        foreach (var targetProp in typeof(TTarget).GetProperties())
        {
            if (!targetProp.CanWrite) continue;

            var sourceProp = typeof(TSource).GetProperty(
                targetProp.Name, 
                BindingFlags.Public | BindingFlags.Instance
            );

            if (sourceProp == null || !sourceProp.CanRead) continue;

            var sourceValue = Expression.Property(sourceParam, sourceProp);
            var convertedValue = targetProp.PropertyType != sourceProp.PropertyType 
                ? Expression.Convert(sourceValue, targetProp.PropertyType) 
                : (Expression)sourceValue;

            assignments.Add(
                Expression.Call(target, targetProp.SetMethod!, convertedValue)
            );
        }

        var body = Expression.Block(
            [target],
            assignments
        );

        return Expression.Lambda<Func<TSource, TTarget>>(body, sourceParam).Compile();
    }
    
    private static Func<IEnumerable<TSource>, List<TTarget>> BuildCollectionMapper(Func<TSource, TTarget> itemMapper)
    {
        var sourceParam = Expression.Parameter(typeof(IEnumerable<TSource>));
        
        // Vytvoření výrazu: sources => sources.Select(itemMapper).ToList()
        var selectCall = Expression.Call(
            typeof(Enumerable),
            nameof(Enumerable.Select),
            [typeof(TSource), typeof(TTarget)],
            sourceParam,
            Expression.Constant(itemMapper)
        );
        
        var toListCall = Expression.Call(
            typeof(Enumerable),
            nameof(Enumerable.ToList),
            [typeof(TTarget)],
            selectCall
        );

        return Expression.Lambda<Func<IEnumerable<TSource>, List<TTarget>>>(
            toListCall,
            sourceParam
        ).Compile();
    }
}

public class MappingConfig<TSource, TTarget>
    where TSource : class
    where TTarget : class, new()
{
    private readonly List<Action<TSource, TTarget>> _mappings = new();

    public void Map<TPropertySource, TPropertyTarget>(
        Expression<Func<TSource, TPropertySource>> sourceProperty,
        Expression<Func<TTarget, TPropertyTarget>> targetProperty,
        Func<TPropertySource, TPropertyTarget>? converter = null)
    {
        // Compile source getter
        var sourceGetter = sourceProperty.Compile();

        // Parse target property
        if (targetProperty.Body is not MemberExpression memberExp ||
            memberExp.Member is not PropertyInfo targetProp ||
            !targetProp.CanWrite)
        {
            throw new ArgumentException("Target must be a writable property.");
        }

        // Create setter using expressions
        var targetParam = Expression.Parameter(typeof(TTarget));
        var valueParam = Expression.Parameter(typeof(TPropertyTarget));
        var setterExp = Expression.Lambda<Action<TTarget, TPropertyTarget>>(
            Expression.Assign(Expression.Property(targetParam, targetProp), valueParam),
            targetParam, valueParam
        );
        var setter = setterExp.Compile();

        // Add mapping with optional conversion
        _mappings.Add((source, target) =>
        {
            var sourceValue = sourceGetter(source);
            var targetValue = converter != null 
                ? converter(sourceValue) 
                : (TPropertyTarget)Convert.ChangeType(sourceValue, typeof(TPropertyTarget))!;
            setter(target, targetValue);
        });
    }

    public Func<TSource, TTarget> Compile()
    {
        return source =>
        {
            var target = new TTarget();
            foreach (var mapping in _mappings)
                mapping(source, target);
            return target;
        };
    }
    
    private static Func<IEnumerable<TSource>, List<TTarget>> BuildCollectionMapper(Func<TSource, TTarget> itemMapper)
    {
        var sourceParam = Expression.Parameter(typeof(IEnumerable<TSource>));
        
        // Vytvoření výrazu: sources => sources.Select(itemMapper).ToList()
        var selectCall = Expression.Call(
            typeof(Enumerable),
            nameof(Enumerable.Select),
            new[] { typeof(TSource), typeof(TTarget) },
            sourceParam,
            Expression.Constant(itemMapper)
        );
        
        var toListCall = Expression.Call(
            typeof(Enumerable),
            nameof(Enumerable.ToList),
            new[] { typeof(TTarget) },
            selectCall
        );

        return Expression.Lambda<Func<IEnumerable<TSource>, List<TTarget>>>(
            toListCall,
            sourceParam
        ).Compile();
    }
}
// Příklad použití:
// Definice tříd
// public class UserEntity
// {
//     public int Id { get; set; }
//     public string Name { get; set; }
//     public DateTime CreatedAt { get; set; }
// }
//
// public class UserDto
// {
//     public int Id { get; set; }
//     public string Name { get; set; }
//     public string CreatedAt { get; set; } // Odlišný typ!
// }
//
// // Konfigurace mapperu
// var mapper = new Mapper<UserEntity, UserDto>(cfg => 
// {
//     cfg.Map(e => e.CreatedAt, d => d.CreatedAt, 
//         date => date.ToString("yyyy-MM-dd")); // Custom konverze
// });
//
// // Použití
// var entity = new UserEntity { Id = 1, Name = "John", CreatedAt = DateTime.Now };
//
// var dto1 = mapper.Map(entity);       // Použije konfiguraci: CreatedAt → string
// var dto2 = mapper.MapDirect(entity); // Automatické mapování: CreatedAt → DateTime → chyba konverze!