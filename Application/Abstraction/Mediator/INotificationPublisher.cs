namespace Application.Abstraction.Mediator;

/// <summary>
/// Rozhraní pro publisher notifikací, který je zodpovědný za distribuci notifikací všem registrovaným handlerům.
/// Umožňuje různé strategie publikování notifikací (např. sériově, paralelně).
/// </summary>
public interface INotificationPublisher
{
    /// <summary>
    /// Publikuje notifikaci všem zadaným handlerům.
    /// </summary>
    /// <typeparam name="TNotification">Typ notifikace.</typeparam>
    /// <param name="handlers">Seznam handlerů, které mají notifikaci zpracovat.</param>
    /// <param name="notification">Notifikace, která má být publikována.</param>
    /// <param name="cancellationToken">Token pro zrušení operace.</param>
    /// <returns>Task reprezentující asynchronní operaci.</returns>
    Task Publish<TNotification>(
        IEnumerable<INotificationHandler<TNotification>> handlers,
        TNotification notification,
        CancellationToken cancellationToken)
        where TNotification : INotification;
}