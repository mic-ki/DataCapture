using Application.Abstraction.Mediator;

namespace Infrastructure.Tests.Mediator.TestClasses;

public class TestNotification : INotification
{
    public string Message { get; set; } = string.Empty;
}

public class TestNotificationHandler1 : INotificationHandler<TestNotification>
{
    public Task Handle(TestNotification notification, CancellationToken cancellationToken)
    {
        // V reálném testu bychom zde mohli zaznamenat, že handler byl volán
        return Task.CompletedTask;
    }
}

public class TestNotificationHandler2 : INotificationHandler<TestNotification>
{
    public Task Handle(TestNotification notification, CancellationToken cancellationToken)
    {
        // V reálném testu bychom zde mohli zaznamenat, že handler byl volán
        return Task.CompletedTask;
    }
}
