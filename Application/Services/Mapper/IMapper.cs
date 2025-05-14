namespace Application.Services.Mapper;

public interface IMapper<in TSource, out TTarget>
    where TSource : class
    where TTarget : class, new()
{
    TTarget Map(TSource source);
    TTarget MapDirect(TSource source);
    IEnumerable<TTarget> MapCollection(IEnumerable<TSource> sources);
    IEnumerable<TTarget> MapDirectCollection(IEnumerable<TSource> sources);
}