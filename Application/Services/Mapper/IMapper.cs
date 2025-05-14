namespace Application.Services.Mapper;

/// <summary>
/// Rozhraní pro mapování mezi zdrojovými a cílovými typy.
/// </summary>
/// <typeparam name="TSource">Zdrojový typ, ze kterého se mapuje.</typeparam>
/// <typeparam name="TTarget">Cílový typ, na který se mapuje.</typeparam>
public interface IMapper<in TSource, out TTarget>
    where TSource : class
    where TTarget : class, new()
{
    /// <summary>
    /// Mapuje zdrojový objekt na cílový objekt pomocí nakonfigurovaných pravidel mapování.
    /// </summary>
    /// <param name="source">Zdrojový objekt, ze kterého se mapuje.</param>
    /// <returns>Nová instance cílového typu s vlastnostmi namapovanými ze zdroje.</returns>
    TTarget Map(TSource source);

    /// <summary>
    /// Mapuje zdrojový objekt na cílový objekt pomocí automatického párování názvů vlastností.
    /// </summary>
    /// <param name="source">Zdrojový objekt, ze kterého se mapuje.</param>
    /// <returns>Nová instance cílového typu s vlastnostmi automaticky namapovanými ze zdroje.</returns>
    TTarget MapDirect(TSource source);

    /// <summary>
    /// Mapuje kolekci zdrojových objektů na cílové objekty pomocí nakonfigurovaných pravidel mapování.
    /// </summary>
    /// <param name="sources">Kolekce zdrojových objektů, ze kterých se mapuje.</param>
    /// <returns>Kolekce cílových objektů namapovaných ze zdrojové kolekce.</returns>
    IEnumerable<TTarget> MapCollection(IEnumerable<TSource> sources);

    /// <summary>
    /// Mapuje kolekci zdrojových objektů na cílové objekty pomocí automatického párování názvů vlastností.
    /// </summary>
    /// <param name="sources">Kolekce zdrojových objektů, ze kterých se mapuje.</param>
    /// <returns>Kolekce cílových objektů automaticky namapovaných ze zdrojové kolekce.</returns>
    IEnumerable<TTarget> MapDirectCollection(IEnumerable<TSource> sources);
}