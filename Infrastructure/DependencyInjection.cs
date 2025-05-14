﻿﻿﻿using Application.Abstraction;
using Application.Abstraction.Mediator;
using Application.Services;
using Application.Services.Events;
using Infrastructure.Mediator;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Infrastructure.Services.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));

        // TODO tohle proveřit v GetPagedSamplesQueryHandler
        // Registruje ApplicationDbContext jako IApplicationDbContext
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IMediator, Mediator.Mediator>();
        services.AddScoped<INotificationPublisher, ParallelNotificationPublisher>();

        // Registrace služeb pro entity
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
        services.AddScoped<DomainEventPublisher>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddMemoryCache();
        services.AddScoped<ICacheService, MemoryCacheService>();


        return services;
    }
}