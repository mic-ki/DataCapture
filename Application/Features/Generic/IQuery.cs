
using Application.Abstraction.Mediator;

namespace Application.Features.Generic;

public interface IQuery<TResult> : IRequest<TResult>;

public interface ICachableQuery<TResult> : IQuery<TResult>
{
    string CacheKey { get; }
    IEnumerable<string>? Tags { get; }
}
