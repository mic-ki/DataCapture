namespace Application.Abstraction.Mediator;

/// <summary>
/// Rozhraní pro handler, který zpracovává požadavky určitého typu a vrací odpovědi.
/// Každý handler implementuje logiku pro zpracování konkrétního typu požadavku.
/// </summary>
/// <typeparam name="TRequest">Typ požadavku, který handler zpracovává.</typeparam>
/// <typeparam name="TResponse">Typ odpovědi, který handler vrací.</typeparam>
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Zpracuje požadavek a vrátí odpověď.
    /// </summary>
    /// <param name="request">Požadavek, který má být zpracován.</param>
    /// <param name="cancellationToken">Token pro zrušení operace.</param>
    /// <returns>Výsledek zpracování požadavku.</returns>
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
