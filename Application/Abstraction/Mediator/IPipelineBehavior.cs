namespace Application.Abstraction.Mediator;

/// <summary>
/// Rozhraní pro pipeline behavior, který umožňuje přidat dodatečnou logiku před a po zpracování požadavku.
/// Implementuje návrhový vzor Decorator, který obaluje zpracování požadavku.
/// Behaviors jsou řetězeny za sebou a tvoří pipeline pro zpracování požadavku.
/// </summary>
/// <typeparam name="TRequest">Typ požadavku, který behavior zpracovává.</typeparam>
/// <typeparam name="TResponse">Typ odpovědi, který behavior vrací.</typeparam>
public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Zpracuje požadavek, provede vlastní logiku a předá řízení dalšímu behavior nebo handleru.
    /// </summary>
    /// <param name="request">Požadavek, který má být zpracován.</param>
    /// <param name="next">Delegát pro volání dalšího behavior nebo handleru v pipeline.</param>
    /// <param name="cancellationToken">Token pro zrušení operace.</param>
    /// <returns>Výsledek zpracování požadavku.</returns>
    Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}