using Application.Abstraction.Mediator;

namespace Infrastructure.Tests.Mediator.TestClasses;

public class TestRequest : IRequest<TestResponse>
{
    public string Data { get; set; } = string.Empty;
}

public class TestResponse
{
    public string Result { get; set; } = string.Empty;
}

public class TestRequestHandler : IRequestHandler<TestRequest, TestResponse>
{
    public Task<TestResponse> Handle(TestRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new TestResponse { Result = $"Handled: {request.Data}" });
    }
}
