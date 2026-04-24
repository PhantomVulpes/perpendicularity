using Microsoft.Extensions.DependencyInjection;
using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Mongo;
using Vulpes.Perpendicularity.Infrastructure.Mongo;

namespace Vulpes.Perpendicularity.Infrastructure.RegistrationExtensions;

public static class MongoRegistration
{
    public static IServiceCollection InjectMongoServices(this IServiceCollection services) => services
        .AddSingleton<IMongoProvider, MongoProvider>()
        .AddTransient(typeof(IModelRepository<>), typeof(MongoRepository<>))
        ;

    public static IServiceCollection InjectQueryProviders(this IServiceCollection services) => services
        .AddTransient<IQueryProvider<Core.Models.RegisteredUser>, MongoQueryProvider<Core.Models.RegisteredUser>>()
        .AddTransient<IQueryProvider<Core.Models.ExternalProject>, MongoQueryProvider<Core.Models.ExternalProject>>()
        ;
}
