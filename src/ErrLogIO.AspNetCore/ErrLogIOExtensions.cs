using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace ErrLogIO.AspNetCore;

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
        services.Configure(configure);

        services.AddHttpClient<ErrLogIO>(options =>
        {
            options.BaseAddress = new Uri("https://relay.errlog.io");
            options.Timeout = TimeSpan.FromSeconds(5);
        });

        services.TryAddSingleton<ErrLogIO>();

        return services;
    }

    public static IApplicationBuilder UseErrLogIO(this IApplicationBuilder app)
    {
        var service = app.ApplicationServices.GetService<ErrLogIO>();

        if (service is null)
        {
            throw new InvalidOperationException(
                $"Please call {nameof(ErrLogIOExtensions.AddErrLogIO)} " +
                $"in ConfigureServices before adding the middleware.");
        }

        return app.UseMiddleware<ErrLogIOMiddleware>();
    }
}
