using System.Reflection;

namespace Application.Services.Mapper;

using System.Linq.Expressions;

/// <summary>
/// Generická implementace mapperu, která poskytuje jak konfigurované, tak automatické mapování mezi zdrojovými a cílovými typy.
/// </summary>
/// <typeparam name="TSource">Zdrojový typ, ze kterého se mapuje.</typeparam>
/// <typeparam name="TTarget">Cílový typ, na který se mapuje.</typeparam>
public class Mapper<TSource, TTarget> : IMapper<TSource, TTarget>
    where TSource : class
    where TTarget : class, new()
{
    /// <summary>
    /// Nakonfigurovaná mapovací funkce založená na uživatelem definovaných pravidlech mapování.
    /// </summary>
    private readonly Func<TSource, TTarget> _configuredMapper;

    /// <summary>
    /// Líně načítaná přímá mapovací funkce, která automaticky mapuje vlastnosti se shodnými názvy.
    /// </summary>
    private static readonly Lazy<Func<TSource, TTarget>> _directMapperLazy = new(
        BuildDirectMapper,
        LazyThreadSafetyMode.ExecutionAndPublication
    );

    /// <summary>
    /// Líně načítaná funkce pro mapování kolekcí, která používá přímý mapper pro mapování kolekcí.
    /// </summary>
    private static readonly Lazy<Func<IEnumerable<TSource>, List<TTarget>>> _directCollectionMapperLazy = new(
        () => BuildCollectionMapper(_directMapperLazy.Value)
    );

    /// <summary>
    /// Inicializuje novou instanci třídy Mapper s vlastní konfigurací mapování.
    /// </summary>
    /// <param name="configAction">Akce, která konfiguruje mapování mezi zdrojovými a cílovými typy.</param>
    public Mapper(Action<MappingConfig<TSource, TTarget>> configAction)
    {
        var config = new MappingConfig<TSource, TTarget>();
        configAction?.Invoke(config);
        _configuredMapper = config.Compile();
    }

    /// <summary>
    /// Mapuje zdrojový objekt na cílový objekt pomocí nakonfigurovaných pravidel mapování.
    /// </summary>
    /// <param name="source">Zdrojový objekt, ze kterého se mapuje.</param>
    /// <returns>Nová instance cílového typu s vlastnostmi namapovanými ze zdroje.</returns>
    /// <exception cref="ArgumentNullException">Vyvoláno, když je zdroj null.</exception>
    public TTarget Map(TSource source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return _configuredMapper(source);
    }

    /// <summary>
    /// Mapuje zdrojový objekt na cílový objekt pomocí automatického párování názvů vlastností.
    /// </summary>
    /// <param name="source">Zdrojový objekt, ze kterého se mapuje.</param>
    /// <returns>Nová instance cílového typu s vlastnostmi automaticky namapovanými ze zdroje.</returns>
    /// <exception cref="ArgumentNullException">Vyvoláno, když je zdroj null.</exception>
    public TTarget MapDirect(TSource source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return _directMapperLazy.Value(source);
    }

    /// <summary>
    /// Mapuje kolekci zdrojových objektů na cílové objekty pomocí nakonfigurovaných pravidel mapování.
    /// </summary>
    /// <param name="sources">Kolekce zdrojových objektů, ze kterých se mapuje.</param>
    /// <returns>Kolekce cílových objektů namapovaných ze zdrojové kolekce.</returns>
    /// <exception cref="ArgumentNullException">Vyvoláno, když je kolekce zdrojů null.</exception>
    public IEnumerable<TTarget> MapCollection(IEnumerable<TSource> sources)
    {
        if (sources == null) throw new ArgumentNullException(nameof(sources));
        return sources.Select(_configuredMapper);
    }

    /// <summary>
    /// Mapuje kolekci zdrojových objektů na cílové objekty pomocí automatického párování názvů vlastností.
    /// </summary>
    /// <param name="sources">Kolekce zdrojových objektů, ze kterých se mapuje.</param>
    /// <returns>Kolekce cílových objektů automaticky namapovaných ze zdrojové kolekce.</returns>
    /// <exception cref="ArgumentNullException">Vyvoláno, když je kolekce zdrojů null.</exception>
    public IEnumerable<TTarget> MapDirectCollection(IEnumerable<TSource> sources)
    {
        if (sources == null) throw new ArgumentNullException(nameof(sources));
        return _directCollectionMapperLazy.Value(sources);
    }
    /// <summary>
    /// Vytváří přímou mapovací funkci, která automaticky mapuje vlastnosti se shodnými názvy.
    /// </summary>
    /// <returns>Funkce, která mapuje zdrojový objekt na cílový objekt.</returns>
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

            var assignExpr = Expression.Call(target, targetProp.SetMethod!, convertedValue);
            assignments.Add(assignExpr);
        }

        // 💡 Přidáme návrat hodnoty na konec Expression.Block
        assignments.Add(target);

        var body = Expression.Block(
            new[] { target },
            assignments
        );

        return Expression.Lambda<Func<TSource, TTarget>>(body, sourceParam).Compile();
    }


    /// <summary>
    /// Vytváří funkci pro mapování kolekcí, která mapuje kolekci zdrojových objektů na cílové objekty.
    /// </summary>
    /// <param name="itemMapper">Funkce, která mapuje jednotlivé položky.</param>
    /// <returns>Funkce, která mapuje kolekci zdrojových objektů na seznam cílových objektů.</returns>
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

/// <summary>
/// Konfigurační třída pro definování vlastních pravidel mapování mezi zdrojovými a cílovými typy.
/// </summary>
/// <typeparam name="TSource">Zdrojový typ, ze kterého se mapuje.</typeparam>
/// <typeparam name="TTarget">Cílový typ, na který se mapuje.</typeparam>
public class MappingConfig<TSource, TTarget>
    where TSource : class
    where TTarget : class, new()
{
    /// <summary>
    /// Seznam mapovacích akcí, které budou aplikovány během mapování.
    /// </summary>
    private readonly List<Action<TSource, TTarget>> _mappings = new();

    /// <summary>
    /// Konfiguruje mapování mezi zdrojovou vlastností a cílovou vlastností.
    /// </summary>
    /// <typeparam name="TPropertySource">Typ zdrojové vlastnosti.</typeparam>
    /// <typeparam name="TPropertyTarget">Typ cílové vlastnosti.</typeparam>
    /// <param name="sourceProperty">Výraz, který vybírá zdrojovou vlastnost.</param>
    /// <param name="targetProperty">Výraz, který vybírá cílovou vlastnost.</param>
    /// <param name="converter">Volitelná konverzní funkce pro převod mezi typy vlastností.</param>
    /// <exception cref="ArgumentException">Vyvoláno, když cílová vlastnost není zapisovatelná.</exception>
    public void Map<TPropertySource, TPropertyTarget>(
        Expression<Func<TSource, TPropertySource>> sourceProperty,
        Expression<Func<TTarget, TPropertyTarget>> targetProperty,
        Func<TPropertySource, TPropertyTarget>? converter = null)
    {
        var sourceGetter = sourceProperty.Compile();

        // Parsuje cílovou vlastnost
        if (targetProperty.Body is not MemberExpression memberExp ||
            memberExp.Member is not PropertyInfo targetProp ||
            !targetProp.CanWrite)
        {
            throw new ArgumentException("Target must be a writable property.");
        }

        var targetParam = Expression.Parameter(typeof(TTarget));
        var valueParam = Expression.Parameter(typeof(TPropertyTarget));
        var setterExp = Expression.Lambda<Action<TTarget, TPropertyTarget>>(
            Expression.Assign(Expression.Property(targetParam, targetProp), valueParam),
            targetParam, valueParam
        );
        var setter = setterExp.Compile();

        // Přidává  mapování s volitelnou konverzí (příklad je dole)
        _mappings.Add((source, target) =>
        {
            var sourceValue = sourceGetter(source);
            var targetValue = converter != null
                ? converter(sourceValue)
                : (TPropertyTarget)Convert.ChangeType(sourceValue, typeof(TPropertyTarget))!;
            setter(target, targetValue);
        });
    }

    /// <summary>
    /// Kompiluje nakonfigurovaná mapování do mapovací funkce.
    /// </summary>
    /// <returns>Funkce, která mapuje zdrojový objekt na cílový objekt pomocí nakonfigurovaných mapování.</returns>
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

    /// <summary>
    /// Vytváří funkci pro mapování kolekcí, která mapuje kolekci zdrojových objektů na cílové objekty.
    /// </summary>
    /// <param name="itemMapper">Funkce, která mapuje jednotlivé položky.</param>
    /// <returns>Funkce, která mapuje kolekci zdrojových objektů na seznam cílových objektů.</returns>
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
// var dto1 = mapper.Map(entity);       // Použijeme konfiguraci: CreatedAt → string
// var dto2 = mapper.MapDirect(entity); // Automatické mapování: CreatedAt → DateTime → chyba konverze!