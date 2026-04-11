using Microsoft.Extensions.DependencyInjection;
using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Mongo;
using Vulpes.Perpendicularity.Core.Data;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Infrastructure.Mongo;

namespace Vulpes.Perpendicularity.Infrastructure.RegistrationExtensions;

public static class InfrastructureRegistration
{
    public static IServiceCollection InjectInfrastructure(this IServiceCollection services) => services
        .InjectMongoServices()
        .InjectRepositories()
        ;

    public static IServiceCollection InjectMongoServices(this IServiceCollection services) => services
        .AddSingleton<IMongoProvider, MongoProvider>()
        .AddTransient(typeof(IModelRepository<>), typeof(MongoRepository<>))
        ;

    private static IServiceCollection InjectRepositories(this IServiceCollection services) => services
        .AddTransient<IQueryProvider<RegisteredUser>, MongoQueryProvider<RegisteredUser>>()
        ;
}