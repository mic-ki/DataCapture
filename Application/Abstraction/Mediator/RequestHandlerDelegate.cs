namespace Application.Abstraction.Mediator;

/// <summary>
/// Delegát pro asynchronní zpracování požadavku, který vrací výsledek.
/// Používá se v pipeline behaviors pro předání řízení dalšímu behavior nebo handleru.
/// </summary>
/// <typeparam name="TResponse">Typ odpovědi, který bude vrácen po zpracování požadavku.</typeparam>
/// <returns>Task s výsledkem zpracování požadavku.</returns>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();