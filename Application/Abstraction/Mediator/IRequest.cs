namespace Application.Abstraction.Mediator;

/// <summary>
/// Rozhraní pro požadavek, který je zpracován mediátorem a vrací výsledek.
/// Slouží jako marker interface pro identifikaci tříd, které představují požadavky.
/// </summary>
/// <typeparam name="TResponse">Typ odpovědi, který bude vrácen po zpracování požadavku.</typeparam>
public interface IRequest<TResponse> { }