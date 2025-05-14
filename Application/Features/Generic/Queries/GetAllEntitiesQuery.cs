using Application.Abstraction;
using Application.Abstraction.Mediator;
using Application.Exceptions;
using Application.Models;
using Application.Services;
using Application.Services.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Generic.Queries;

public abstract class GetAllEntitiesQuery<TEntity, TDto> : IRequest<Result<List<TDto>>>;

public class GetAllEntitiesQueryHandler<TEntity, TDto>
    : GenericCollectionQueryHandler<TEntity, TDto, GetAllEntitiesQuery<TEntity, TDto>>
    where TEntity : class
    where TDto : class, new()
{
    public GetAllEntitiesQueryHandler(IApplicationDbContext context, IMapper<TEntity, TDto> mapper)
        : base(context, mapper)
    {
    }

    protected override async Task<List<TDto>> FetchCollectionAsync(GetAllEntitiesQuery<TEntity, TDto> request,
        CancellationToken cancellationToken)
    {
        var data = await Context.Set<TEntity>().ToListAsync(cancellationToken);
        if (data is null)
            throw new NotFoundException(typeof(TEntity).Name, typeof(TDto).Name);

        // Vracíme přímo kolekci TDto
        return Mapper.MapDirectCollection(data).ToList();
    }
}