using Application.Abstraction.Mediator;
using Infrastructure.Mediator;
using Infrastructure.Tests.Mediator.TestClasses;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Infrastructure.Tests.Mediator;

public class MediatorTests
{
    [Fact]
    public async Task Send_ShouldCallCorrectHandler()
    {
        // Arrange
        var serviceProvider = new ServiceCollection()
            .AddTransient<IRequestHandler<TestRequest, TestResponse>, TestRequestHandler>()
            .BuildServiceProvider();

        var notificationPublisher = new Mock<INotificationPublisher>();
        var mediator = new Infrastructure.Mediator.Mediator(serviceProvider, notificationPublisher.Object);
        
        var request = new TestRequest { Data = "Test Data" };
        
        // Act
        var response = await mediator.Send(request);
        
        // Assert
        Assert.NotNull(response);
        Assert.Equal("Handled: Test Data", response.Result);
    }
    
    [Fact]
    public async Task Send_WithPipelineBehaviors_ShouldExecuteInCorrectOrder()
    {
        // Arrange
        var executionOrder = new List<string>();
        
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<IRequestHandler<TestRequest, TestResponse>, TestRequestHandler>();
        
        // Přidáme dva behaviors
        serviceCollection.AddTransient<IPipelineBehavior<TestRequest, TestResponse>>(
            _ => new TestPipelineBehavior<TestRequest, TestResponse>("Behavior1", executionOrder));
        serviceCollection.AddTransient<IPipelineBehavior<TestRequest, TestResponse>>(
            _ => new TestPipelineBehavior<TestRequest, TestResponse>("Behavior2", executionOrder));
        
        var serviceProvider = serviceCollection.BuildServiceProvider();
        
        var notificationPublisher = new Mock<INotificationPublisher>();
        var mediator = new Infrastructure.Mediator.Mediator(serviceProvider, notificationPublisher.Object);
        
        var request = new TestRequest { Data = "Test Data" };
        
        // Act
        var response = await mediator.Send(request);
        
        // Assert
        Assert.NotNull(response);
        Assert.Equal("Handled: Test Data", response.Result);
        
        // Ověříme pořadí volání behaviors
        // Behaviors jsou aplikovány v pořadí, v jakém byly registrovány, ale v opačném pořadí se vrací
        Assert.Equal(4, executionOrder.Count);
        Assert.Equal("Before Behavior1", executionOrder[0]);
        Assert.Equal("Before Behavior2", executionOrder[1]);
        Assert.Equal("After Behavior2", executionOrder[2]);
        Assert.Equal("After Behavior1", executionOrder[3]);
    }
    
    [Fact]
    public async Task Publish_ShouldCallAllHandlers()
    {
        // Arrange
        var handler1 = new Mock<INotificationHandler<TestNotification>>();
        handler1.Setup(h => h.Handle(It.IsAny<TestNotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var handler2 = new Mock<INotificationHandler<TestNotification>>();
        handler2.Setup(h => h.Handle(It.IsAny<TestNotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<INotificationHandler<TestNotification>>(_ => handler1.Object);
        serviceCollection.AddTransient<INotificationHandler<TestNotification>>(_ => handler2.Object);
        
        var serviceProvider = serviceCollection.BuildServiceProvider();
        
        var notificationPublisher = new Mock<INotificationPublisher>();
        notificationPublisher
            .Setup(p => p.Publish(
                It.IsAny<IEnumerable<INotificationHandler<TestNotification>>>(),
                It.IsAny<TestNotification>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var mediator = new Infrastructure.Mediator.Mediator(serviceProvider, notificationPublisher.Object);
        
        var notification = new TestNotification { Message = "Test Message" };
        
        // Act
        await mediator.Publish(notification);
        
        // Assert
        notificationPublisher.Verify(
            p => p.Publish(
                It.Is<IEnumerable<INotificationHandler<TestNotification>>>(handlers => handlers.Count() == 2),
                It.Is<TestNotification>(n => n.Message == "Test Message"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task Publish_WithNoHandlers_ShouldNotCallPublisher()
    {
        // Arrange
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        
        var notificationPublisher = new Mock<INotificationPublisher>();
        var mediator = new Infrastructure.Mediator.Mediator(serviceProvider, notificationPublisher.Object);
        
        var notification = new TestNotification { Message = "Test Message" };
        
        // Act
        await mediator.Publish(notification);
        
        // Assert
        notificationPublisher.Verify(
            p => p.Publish(
                It.IsAny<IEnumerable<INotificationHandler<TestNotification>>>(),
                It.IsAny<TestNotification>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
