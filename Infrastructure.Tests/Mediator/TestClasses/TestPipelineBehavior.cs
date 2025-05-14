using Application.Abstraction.Mediator;

namespace Infrastructure.Tests.Mediator.TestClasses;

public class TestPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly string _behaviorName;
    private readonly List<string> _executionOrder;

    public TestPipelineBehavior(string behaviorName, List<string> executionOrder)
    {
        _behaviorName = behaviorName;
        _executionOrder = executionOrder;
    }

    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Zaznamenáme, že behavior byl spuštěn před voláním handleru
        _executionOrder.Add($"Before {_behaviorName}");
        
        // Zavoláme další behavior nebo handler
        var response = await next();
        
        // Zaznamenáme, že behavior byl spuštěn po volání handleru
        _executionOrder.Add($"After {_behaviorName}");
        
        return response;
    }
}