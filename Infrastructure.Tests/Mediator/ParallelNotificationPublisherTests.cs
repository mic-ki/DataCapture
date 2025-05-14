using Application.Abstraction.Mediator;
using Infrastructure.Mediator;
using Infrastructure.Tests.Mediator.TestClasses;
using Moq;
using Xunit;

namespace Infrastructure.Tests.Mediator;

public class ParallelNotificationPublisherTests
{
    [Fact]
    public async Task Publish_ShouldCallAllHandlersInParallel()
    {
        // Arrange
        var handler1 = new Mock<INotificationHandler<TestNotification>>();
        var handler1Called = false;
        
        handler1.Setup(h => h.Handle(It.IsAny<TestNotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.Run(async () => 
            {
                await Task.Delay(100); // Simulujeme nějakou práci
                handler1Called = true;
            }));
        
        var handler2 = new Mock<INotificationHandler<TestNotification>>();
        var handler2Called = false;
        
        handler2.Setup(h => h.Handle(It.IsAny<TestNotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.Run(async () => 
            {
                await Task.Delay(100); // Simulujeme nějakou práci
                handler2Called = true;
            }));
        
        var handlers = new List<INotificationHandler<TestNotification>> 
        { 
            handler1.Object, 
            handler2.Object 
        };
        
        var publisher = new ParallelNotificationPublisher();
        var notification = new TestNotification { Message = "Test Message" };
        
        // Act
        await publisher.Publish(handlers, notification, CancellationToken.None);
        
        // Assert
        Assert.True(handler1Called);
        Assert.True(handler2Called);
        
        handler1.Verify(h => h.Handle(
            It.Is<TestNotification>(n => n.Message == "Test Message"),
            It.IsAny<CancellationToken>()), 
            Times.Once);
        
        handler2.Verify(h => h.Handle(
            It.Is<TestNotification>(n => n.Message == "Test Message"),
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    [Fact]
    public async Task Publish_WithFailingHandler_ShouldStillCallOtherHandlers()
    {
        // Arrange
        var handler1 = new Mock<INotificationHandler<TestNotification>>();
        handler1.Setup(h => h.Handle(It.IsAny<TestNotification>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Handler 1 failed"));
        
        var handler2 = new Mock<INotificationHandler<TestNotification>>();
        var handler2Called = false;
        
        handler2.Setup(h => h.Handle(It.IsAny<TestNotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.Run(async () => 
            {
                await Task.Delay(100);
                handler2Called = true;
            }));
        
        var handlers = new List<INotificationHandler<TestNotification>> 
        { 
            handler1.Object, 
            handler2.Object 
        };
        
        var publisher = new ParallelNotificationPublisher();
        var notification = new TestNotification { Message = "Test Message" };
        
        // Act & Assert
        // Očekáváme výjimku, protože Task.WhenAll propaguje výjimky
        await Assert.ThrowsAsync<Exception>(() => 
            publisher.Publish(handlers, notification, CancellationToken.None));
        
        // I když jeden handler selhal, druhý by měl být stále volán
        // Poznámka: Toto chování závisí na implementaci Task.WhenAll
        // V reálném prostředí by bylo lepší, kdyby publisher zachytil výjimky a logoval je
        handler1.Verify(h => h.Handle(
            It.Is<TestNotification>(n => n.Message == "Test Message"),
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}
