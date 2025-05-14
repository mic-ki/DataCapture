using Application.Abstraction.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Mediator;

/// <summary>
/// Implementace rozhraní IMediator, která zprostředkovává komunikaci mezi různými částmi aplikace.
/// Implementuje návrhový vzor Mediator, který umožňuje snížit propojení mezi komponentami.
/// </summary>
public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly INotificationPublisher _notificationPublisher;

    /// <summary>
    /// Inicializuje novou instanci třídy <see cref="Mediator"/>.
    /// </summary>
    /// <param name="serviceProvider">Service provider pro získání handlerů a behaviors.</param>
    /// <param name="notificationPublisher">Publisher pro distribuci notifikací.</param>
    public Mediator(IServiceProvider serviceProvider, INotificationPublisher notificationPublisher)
    {
        _serviceProvider = serviceProvider;
        _notificationPublisher = notificationPublisher;
    }

    /// <summary>
    /// Odešle požadavek příslušnému handleru a vrátí výsledek.
    /// Vytvoří pipeline behaviors, které jsou aplikovány před a po zpracování požadavku.
    /// </summary>
    /// <typeparam name="TResponse">Typ odpovědi, který handler vrátí.</typeparam>
    /// <param name="request">Požadavek, který má být zpracován.</param>
    /// <param name="cancellationToken">Token pro zrušení operace.</param>
    /// <returns>Výsledek zpracování požadavku.</returns>
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        // Získání typu handleru pro daný typ požadavku a odpovědi
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        dynamic handler = _serviceProvider.GetRequiredService(handlerType);

        // Získání všech registrovaných pipeline behaviors pro daný typ požadavku a odpovědi
        var behaviorsType = typeof(IEnumerable<>).MakeGenericType(typeof(IPipelineBehavior<,>)
            .MakeGenericType(request.GetType(), typeof(TResponse)));
        var behaviors = (IEnumerable<dynamic>)_serviceProvider.GetService(behaviorsType) ?? Enumerable.Empty<dynamic>();

        // Vytvoření delegátu pro volání handleru
        RequestHandlerDelegate<TResponse> handlerDelegate = () => handler.Handle((dynamic)request, cancellationToken);

        // Vytvoření pipeline behaviors v opačném pořadí (od posledního k prvnímu)
        // Takže první registrovaný behavior bude první v pipeline
        foreach (var behavior in behaviors.Reverse())
        {
            var next = handlerDelegate;
            // Vytvoření nového delegátu, který volá behavior a předává mu původní delegát
            handlerDelegate = () => behavior.Handle((dynamic)request, next, cancellationToken);
        }

        // Spuštění pipeline
        return await handlerDelegate();
    }

    /// <summary>
    /// Publikuje notifikaci všem registrovaným handlerům.
    /// Používá INotificationPublisher pro distribuci notifikací handlerům.
    /// </summary>
    /// <typeparam name="TNotification">Typ notifikace.</typeparam>
    /// <param name="notification">Notifikace, která má být publikována.</param>
    /// <param name="cancellationToken">Token pro zrušení operace.</param>
    /// <returns>Task reprezentující asynchronní operaci.</returns>
    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        // Získání všech registrovaných handlerů pro daný typ notifikace
        IEnumerable<INotificationHandler<TNotification>> handlers = _serviceProvider.GetServices<INotificationHandler<TNotification>>();

        // Pokud nejsou žádné handlery, není potřeba nic dělat
        if (!handlers.Any()) return;

        // Použití publisheru pro distribuci notifikace všem handlerům
        await _notificationPublisher.Publish(handlers, notification, cancellationToken);
    }
}
