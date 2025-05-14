using Application.Abstraction.Mediator;

namespace Infrastructure.Mediator;

/// <summary>
/// Implementace INotificationPublisher, která publikuje notifikace všem handlerům paralelně.
/// Používá Task.WhenAll pro současné zpracování všech notifikací.
/// </summary>
public class ParallelNotificationPublisher : INotificationPublisher
{
    /// <summary>
    /// Publikuje notifikaci všem zadaným handlerům paralelně.
    /// Všechny handlery jsou spouštěny současně a metoda čeká na dokončení všech handlerů.
    /// Pokud některý z handlerů vyhodí výjimku, bude propagována zpět k volajícímu.
    /// </summary>
    /// <typeparam name="TNotification">Typ notifikace.</typeparam>
    /// <param name="handlers">Seznam handlerů, které mají notifikaci zpracovat.</param>
    /// <param name="notification">Notifikace, která má být publikována.</param>
    /// <param name="cancellationToken">Token pro zrušení operace.</param>
    /// <returns>Task reprezentující asynchronní operaci, která je dokončena, když všechny handlery dokončí zpracování.</returns>
    public Task Publish<TNotification>(
        IEnumerable<INotificationHandler<TNotification>> handlers,
        TNotification notification,
        CancellationToken cancellationToken)
        where TNotification : INotification
    {
        // Vytvoření tasků pro všechny handlery
        var tasks = handlers.Select(h => h.Handle(notification, cancellationToken));

        // Čekání na dokončení všech tasků
        return Task.WhenAll(tasks);
    }
}