using Application.Abstraction.Mediator;
using Application.Models;
using Application.Services;
using Application.Services.Mapper;

namespace Application.Features.Generic;

public abstract class GenericQueryHandler<TEntity, TDto, TRequest> 
: IRequestHandler<TRequest, Result<object>> where TRequest : IRequest<Result<object>> where TEntity : class where TDto : class, new()
{
    protected readonly IApplicationDbContext Context;
    protected readonly IMapper<TEntity, TDto> Mapper;

    protected GenericQueryHandler(IApplicationDbContext context, IMapper<TEntity, TDto> mapper)
    {
        Context = context;
        Mapper = mapper;
    }

    public async Task<Result<object>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var result = await FetchDataAsync(request, cancellationToken);
        return await Result<object>.SuccessAsync(result);
    }
    protected abstract Task<Result<object>> FetchDataAsync(TRequest request,
        CancellationToken cancellationToken);
}