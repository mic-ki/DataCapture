namespace Application.Abstraction.Mediator;

/// <summary>
/// Rozhraní pro mediátor, který implementuje návrhový vzor Mediator.
/// Slouží jako centrální bod pro zpracování požadavků a publikování notifikací v aplikaci.
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Odešle požadavek příslušnému handleru a vrátí výsledek.
    /// </summary>
    /// <typeparam name="TResponse">Typ odpovědi, který handler vrátí.</typeparam>
    /// <param name="request">Požadavek, který má být zpracován.</param>
    /// <param name="cancellationToken">Token pro zrušení operace.</param>
    /// <returns>Výsledek zpracování požadavku.</returns>
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publikuje notifikaci všem registrovaným handlerům.
    /// </summary>
    /// <typeparam name="TNotification">Typ notifikace.</typeparam>
    /// <param name="notification">Notifikace, která má být publikována.</param>
    /// <param name="cancellationToken">Token pro zrušení operace.</param>
    /// <returns>Task reprezentující asynchronní operaci.</returns>
    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;
}