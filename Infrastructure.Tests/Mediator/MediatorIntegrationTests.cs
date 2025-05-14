using Application.Abstraction.Mediator;
using Application.Services.Events;
using Domain.Core;
using Infrastructure.Mediator;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Infrastructure.Tests.Mediator;

public class MediatorIntegrationTests
{
    private readonly ITestOutputHelper _output;

    public MediatorIntegrationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    // Testovací doménová událost
    private class TestDomainEvent : DomainEvent
    {
        public string Data { get; set; } = string.Empty;
    }

    // Handler pro doménovou událost
    private class TestDomainEventHandler : INotificationHandler<DomainEventNotification<TestDomainEvent>>
    {
        private readonly List<string> _handledEvents;
        private readonly ITestOutputHelper _output;

        public TestDomainEventHandler(List<string> handledEvents, ITestOutputHelper output)
        {
            _handledEvents = handledEvents;
            _output = output;
        }

        public Task Handle(DomainEventNotification<TestDomainEvent> notification, CancellationToken cancellationToken)
        {
            _output.WriteLine($"Handler called with data: {notification.DomainEvent.Data}");
            _handledEvents.Add(notification.DomainEvent.Data);
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task DomainEventPublisher_ShouldPublishEventsViaMediator()
    {
        // Arrange
        var handledEvents = new List<string>();

        var services = new ServiceCollection();

        // Registrujeme Mediator a NotificationPublisher
        services.AddSingleton<IMediator, Infrastructure.Mediator.Mediator>();
        services.AddSingleton<INotificationPublisher, ParallelNotificationPublisher>();

        // Registrujeme DomainEventPublisher
        services.AddScoped<DomainEventPublisher>();

        // Registrujeme handler pro doménovou událost
        services.AddTransient<INotificationHandler<DomainEventNotification<TestDomainEvent>>>(
            _ => new TestDomainEventHandler(handledEvents, _output));

        var serviceProvider = services.BuildServiceProvider();

        // Získáme instanci DomainEventPublisher
        var domainEventPublisher = serviceProvider.GetRequiredService<DomainEventPublisher>();

        // Získáme instanci Mediatoru pro debugging
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        // Ověříme, že handler je správně registrován
        var handlers = serviceProvider.GetServices<INotificationHandler<DomainEventNotification<TestDomainEvent>>>().ToList();
        _output.WriteLine($"Found {handlers.Count} handlers for DomainEventNotification<TestDomainEvent>");
        Assert.Single(handlers); // Ověříme, že máme jeden handler

        // Vytvoříme doménovou událost
        var domainEvent = new TestDomainEvent { Data = "Test Domain Event" };

        // Act
        // Vytvoříme notifikaci přímo a publikujeme ji přes mediator pro ověření
        _output.WriteLine("Publishing notification directly via mediator");
        var notification = new DomainEventNotification<TestDomainEvent>(domainEvent);
        await mediator.Publish(notification);

        // Ověříme, že notifikace byla zpracována
        Assert.Single(handledEvents);
        Assert.Equal("Test Domain Event", handledEvents[0]);
        handledEvents.Clear();

        // Nyní zkusíme publikovat přes DomainEventPublisher
        _output.WriteLine("Publishing event via DomainEventPublisher");
        await domainEventPublisher.Publish(domainEvent);

        // Assert
        Assert.Single(handledEvents);
        Assert.Equal("Test Domain Event", handledEvents[0]);
        Assert.True(domainEvent.IsPublished);
    }

    [Fact]
    public async Task DomainEventPublisher_ShouldPublishMultipleEvents()
    {
        // Arrange
        var handledEvents = new List<string>();

        var services = new ServiceCollection();

        // Registrujeme Mediator a NotificationPublisher
        services.AddSingleton<IMediator, Infrastructure.Mediator.Mediator>();
        services.AddSingleton<INotificationPublisher, ParallelNotificationPublisher>();

        // Registrujeme DomainEventPublisher
        services.AddScoped<DomainEventPublisher>();

        // Registrujeme handler pro doménovou událost
        services.AddTransient<INotificationHandler<DomainEventNotification<TestDomainEvent>>>(
            _ => new TestDomainEventHandler(handledEvents, _output));

        var serviceProvider = services.BuildServiceProvider();

        // Získáme instanci DomainEventPublisher
        var domainEventPublisher = serviceProvider.GetRequiredService<DomainEventPublisher>();

        // Získáme instanci Mediatoru pro debugging
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        // Ověříme, že handler je správně registrován
        var handlers = serviceProvider.GetServices<INotificationHandler<DomainEventNotification<TestDomainEvent>>>().ToList();
        _output.WriteLine($"Found {handlers.Count} handlers for DomainEventNotification<TestDomainEvent>");

        // Vytvoříme doménové události
        var domainEvent1 = new TestDomainEvent { Data = "Event 1" };
        var domainEvent2 = new TestDomainEvent { Data = "Event 2" };
        var domainEvent3 = new TestDomainEvent { Data = "Event 3" };

        var events = new List<DomainEvent> { domainEvent1, domainEvent2, domainEvent3 };

        // Act
        await domainEventPublisher.Publish(events);

        // Assert
        Assert.Equal(3, handledEvents.Count);
        Assert.Contains("Event 1", handledEvents);
        Assert.Contains("Event 2", handledEvents);
        Assert.Contains("Event 3", handledEvents);

        Assert.True(domainEvent1.IsPublished);
        Assert.True(domainEvent2.IsPublished);
        Assert.True(domainEvent3.IsPublished);
    }
}
