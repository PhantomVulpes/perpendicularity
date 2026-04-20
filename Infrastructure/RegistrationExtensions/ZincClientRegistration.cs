using Infrastructure.Zinc;
using Microsoft.Extensions.DependencyInjection;
using Vulpes.Perpendicularity.Infrastructure.Zinc.ClientFactories;

namespace Vulpes.Perpendicularity.Infrastructure.RegistrationExtensions;

public static class ZincClientRegistration
{
    public static IServiceCollection InjectZincServices(this IServiceCollection services) => services
        .AddZincClient()
        .AddSingleton<MockZincClientFactory>()
        .AddSingleton<LiveZincClientFactory>()
        .AddSingleton<IZincClientFactory, ZincClientFactory>()
        ;

    private static IServiceCollection AddZincClient(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddTransient<IZincClient>(serviceProvider =>
        {
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient();
            return new ZincClient("http://shadesmar:60000/", httpClient);
        });

        return services;
    }
}
