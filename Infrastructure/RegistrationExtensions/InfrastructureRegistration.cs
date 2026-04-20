using Microsoft.Extensions.DependencyInjection;
using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Mongo;
using Vulpes.Perpendicularity.Infrastructure.Mongo;
using Infrastructure.Zinc;

namespace Vulpes.Perpendicularity.Infrastructure.RegistrationExtensions;

public static class InfrastructureRegistration
{
    public static IServiceCollection InjectInfrastructure(this IServiceCollection services) => services
        .AddZincClient("http://shadesmar:60000/")
        .InjectMongoServices()
        .InjectQueryProviders()
        ;

    private static IServiceCollection InjectMongoServices(this IServiceCollection services) => services
        .AddSingleton<IMongoProvider, MongoProvider>()
        .AddTransient(typeof(IModelRepository<>), typeof(MongoRepository<>))
        ;

    private static IServiceCollection InjectQueryProviders(this IServiceCollection services) => services
        .AddTransient<IQueryProvider<Core.Models.RegisteredUser>, MongoQueryProvider<Core.Models.RegisteredUser>>()
        ;

    private static IServiceCollection AddZincClient(this IServiceCollection services, string baseUrl)
    {
        services.AddHttpClient();
        services.AddTransient<IZincClient>(serviceProvider =>
        {
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient();
            return new ZincClient(baseUrl, httpClient);
        });

        return services;
    }
}