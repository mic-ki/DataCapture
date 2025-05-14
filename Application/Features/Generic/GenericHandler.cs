using Application.Abstraction;
using Application.Abstraction.Mediator;
using Application.Models;
using Application.Services;
using Application.Services.Mapper;

namespace Application.Features.Generic;

// 1) Generický handler pro vracení kolekce TDto
public abstract class GenericCollectionQueryHandler<TEntity, TDto, TRequest>
    : IRequestHandler<TRequest, Result<List<TDto>>>
    where TRequest : IRequest<Result<List<TDto>>>
    where TEntity : class
    where TDto : class, new()
{
    protected readonly IApplicationDbContext Context;
    protected readonly IMapper<TEntity, TDto> Mapper;

    protected GenericCollectionQueryHandler(IApplicationDbContext context, IMapper<TEntity, TDto> mapper)
    {
        Context = context;
        Mapper = mapper;
    }

    public async Task<Result<List<TDto>>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var data = await FetchCollectionAsync(request, cancellationToken);
        return await Result<List<TDto>>.OkAsync(data);
    }

    protected abstract Task<List<TDto>> FetchCollectionAsync(TRequest request, CancellationToken cancellationToken);
}

// 2) Generický handler pro vracení jednoho TDto
public abstract class GenericSingleQueryHandler<TEntity, TDto, TRequest>
    : IRequestHandler<TRequest, Result<TDto>>
    where TRequest : IRequest<Result<TDto>>
    where TEntity : class
    where TDto : class, new()
{
    protected readonly IApplicationDbContext Context;
    protected readonly IMapper<TEntity, TDto> Mapper;

    protected GenericSingleQueryHandler(IApplicationDbContext context, IMapper<TEntity, TDto> mapper)
    {
        Context = context;
        Mapper = mapper;
    }

    public async Task<Result<TDto>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var item = await FetchSingleAsync(request, cancellationToken);
        return await Result<TDto>.OkAsync(item);
    }

    protected abstract Task<TDto> FetchSingleAsync(TRequest request, CancellationToken cancellationToken);
}