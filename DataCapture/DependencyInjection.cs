using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataCapture;

public static class DependencyInjection
{
    public static IServiceCollection AddServerUI(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddHubOptions(options=> options.MaximumReceiveMessageSize = 64 * 1024);
        services.AddCascadingAuthenticationState();

        services.AddControllers();

        services.AddProblemDetails();
        services.AddHealthChecks();

        return services;
    }
}