namespace Application.Abstraction.Mediator;

/// <summary>
/// Rozhraní pro handler, který zpracovává notifikace určitého typu.
/// Každý handler implementuje logiku pro zpracování konkrétního typu notifikace.
/// Na rozdíl od IRequestHandler může být pro jeden typ notifikace více handlerů.
/// </summary>
/// <typeparam name="TNotification">Typ notifikace, kterou handler zpracovává.</typeparam>
public interface INotificationHandler<TNotification>
{
    /// <summary>
    /// Zpracuje notifikaci.
    /// </summary>
    /// <param name="notification">Notifikace, která má být zpracována.</param>
    /// <param name="cancellationToken">Token pro zrušení operace.</param>
    /// <returns>Task reprezentující asynchronní operaci.</returns>
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}