namespace DataCapture;

public static class DependencyInjection
{
    public static IServiceCollection AddServerUI(this IServiceCollection services, IConfiguration config)
    {
        services.AddRazorComponents().AddInteractiveServerComponents().AddHubOptions(options=> options.MaximumReceiveMessageSize = 64 * 1024);
        services.AddCascadingAuthenticationState();
        //services.AddScoped<IdentityUserAccessor>();
        //services.AddScoped<IdentityRedirectManager>();
        

        // services.AddScoped<LocalizationCookiesMiddleware>()
        //     .Configure<RequestLocalizationOptions>(options =>
        //     {
        //         options.AddSupportedUICultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray());
        //         options.AddSupportedCultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray());
        //         options.FallBackToParentUICultures = true;
        //     })
        //     .AddLocalization(options => options.ResourcesPath = LocalizationConstants.ResourcesPath);

        // services.AddHangfire(configuration => configuration
        //         .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        //         .UseSimpleAssemblyNameTypeSerializer()
        //         .UseRecommendedSerializerSettings()
        //         .UseInMemoryStorage())
        //     .AddHangfireServer()
        //     .AddMvc();

        services.AddControllers();

        // services.AddScoped<IApplicationHubWrapper, ServerHubWrapper>()
        //     .AddSignalR(options=>options.MaximumReceiveMessageSize=64*1024);
        //services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddHealthChecks();
        
        // services.AddScoped<LocalTimeOffset>();
        // services.AddScoped<HubClient>();
        // services
        //     .AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>()
        //     .AddScoped<LayoutService>()
        //     .AddScoped<DialogServiceHelper>()
        //     .AddScoped<PermissionHelper>()
        //     .AddBlazorDownloadFile()
        //     .AddScoped<IUserPreferencesService, UserPreferencesService>()
        //     .AddScoped<IMenuService, MenuService>()
        //     .AddScoped<InMemoryNotificationService>()
        //     .AddScoped<INotificationService>(sp =>
        //     {
        //         var service = sp.GetRequiredService<InMemoryNotificationService>();
        //         service.Preload();
        //         return service;
        //     });
        
        return services;
    }
}