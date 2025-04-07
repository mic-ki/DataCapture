using Application.Abstraction.Mediator;
using Application.Exceptions;
using Application.Models;
using Application.Services;
using Application.Services.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Generic.Queries;

public abstract class GetAllEntitiesQuery<TEntity, TDto> : IRequest<Result<object>>;

public class GetAllEntitiesQueryHandler<TEntity, TDto> :
    GenericQueryHandler<TEntity, TDto, GetAllEntitiesQuery<TEntity, TDto>> 
    where TEntity : class where TDto : class, new() 
{
    public GetAllEntitiesQueryHandler(IApplicationDbContext context, IMapper<TEntity, TDto> mapper) 
        : base(context, mapper)
    {
    }

    protected override async Task<Result<object>> FetchDataAsync(GetAllEntitiesQuery<TEntity, TDto> request,
        CancellationToken cancellationToken)
    {
        var data = await Context.Set<TEntity>().ToListAsync(cancellationToken);
 

        if (data is null)
            throw new NotFoundException(typeof(TEntity).Name, typeof(TDto).Name);

        return await Result<object>.SuccessAsync(Mapper.MapDirectCollection(data));
    }
}