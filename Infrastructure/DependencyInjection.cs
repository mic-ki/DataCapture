using Application.Abstraction.Mediator;
using Infrastructure.Mediator;
using Infrastructure.Persistence;
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
        //options.UseSqlServer(connectionString));
        //services.AddDatabaseDeveloperPageExceptionFilter();
        
        services.AddSingleton<IMediator, Mediator.Mediator>(); 
        services.AddSingleton<INotificationPublisher, ParallelNotificationPublisher>();
        //services.AddTransient<IRequestHandler<TRequest, TResponse>, THandler>();
        //services.AddTransient<INotificationHandler<TNotification>, TNotificationHandler>();
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(SomePipelineBehavior<,>));
        
        return services;
    }
}