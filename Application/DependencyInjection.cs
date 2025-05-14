﻿using Application.Abstraction;
 using Application.Abstraction.Mediator;
 using Application.Features.Generic.Queries;
 using Application.Features.Sample;
using Application.Features.Sample.Queries;
using Application.Models;
using Application.Pipeline;
using Application.Services;
using Application.Services.Mapper;
using Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ISessionInfoService, SessionInfoService>();
        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        services.AddSingleton<IMapper<SampleEntity, SampleDto>>(sp =>
            new Mapper<SampleEntity, SampleDto>(cfg =>
            {
                // Konfigurace mapperu pro SampleEntity -> SampleDto
                // Například:
                // cfg.Map(e => e.Created, d => d.CreatedDate, date => date?.ToString("yyyy-MM-dd"));
            }));

        // TODO: tady bude třeba vymyslet šikovnou registraci
        services.AddScoped<IRequestHandler<GetAllSamplesQuery, Result<List<SampleDto>>>, GetAllEntitiesQueryHandler<SampleEntity,SampleDto>>();
        services.AddTransient<IRequestHandler<GetPagedSamplesQuery, Result<PagedList<SampleDto>>>, GetPagedSamplesQueryHandler>();
        return services;
    }
}