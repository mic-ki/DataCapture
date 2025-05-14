using Application.Abstraction;
using Application.Abstraction.Mediator;
using Application.Exceptions;
using Application.Models;
using Application.Services;
using Application.Services.Mapper;


namespace Application.Features.Generic.Queries;

public abstract class GetEntityByIdQuery<TEntity, TDto> : IRequest<Result<TDto>>
{
    public required int Id { get; set; }
}

internal sealed class GetEntityByIdQueryHandler<TEntity, TDto>
    : GenericSingleQueryHandler<TEntity, TDto, GetEntityByIdQuery<TEntity, TDto>>
    where TEntity : class
    where TDto : class, new()
{
    public GetEntityByIdQueryHandler(IApplicationDbContext context, IMapper<TEntity, TDto> mapper)
        : base(context, mapper)
    {
    }

    protected override async Task<TDto> FetchSingleAsync(GetEntityByIdQuery<TEntity, TDto> request,
        CancellationToken cancellationToken)
    {
        var entity = await Context.Set<TEntity>()
            .FindAsync([request.Id], cancellationToken);
        if (entity is null)
            throw new NotFoundException(typeof(TEntity).Name, request.Id);

        return Mapper.MapDirect(entity);
    }
}
