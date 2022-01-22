using ErrLogIO.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection;

public static class ErrLogIOExtensions
{
    public static IServiceCollection AddErrLogIO(
        this IServiceCollection services, string apiKey)
    {
        return services.AddErrLogIO(options
            => options.ApiKey = apiKey);
    }

    public static IServiceCollection AddErrLogIO(
        this IServiceCollection services, Action<ErrLogIOOptions> configure)
    {
        services.AddErrLogIO();
        services.Configure(configure);
        return services;
    }

    public static IServiceCollection AddErrLogIO(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services.AddHttpClient<ErrLogIOService>(options =>
        {
            options.BaseAddress = new Uri("https://relay.errlog.io");
            options.Timeout = TimeSpan.FromSeconds(5);
        });

        services.TryAddSingleton<ErrLogIOService>();

        return services;
    }

    public static IApplicationBuilder UseErrLogIO(this IApplicationBuilder app)
    {
        if (app == null)
            throw new ArgumentNullException(nameof(app));

        var service = app.ApplicationServices.GetService<ErrLogIOService>();

        if (service is null)
        {
            throw new InvalidOperationException(
                $"Please call {nameof(ErrLogIOExtensions.AddErrLogIO)} " +
                $"in ConfigureServices before adding the middleware.");
        }

        return app.UseMiddleware<ErrLogIOMiddleware>();
    }
}
