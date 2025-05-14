using Application.Abstraction;
using Application.Abstraction.Mediator;
using Application.Features.Generic;

namespace Application.Pipeline;

public class CacheBehavior<TRequest, TResponse>(ICacheService cacheService) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachableQuery<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Kontrola cache
        var cachedResponse = await cacheService.GetAsync<TResponse>(request.CacheKey, cancellationToken);
        if (cachedResponse != null)
        {
            return cachedResponse;
        }

        // Pokračování k dalšímu handleru, pokud data nejsou v cache
        var response = await next();

        // Uložení výsledku do cache
        await cacheService.SetAsync(request.CacheKey, response, TimeSpan.FromMinutes(10), cancellationToken);

        return response;
    }
}
