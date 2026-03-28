using Microsoft.Extensions.DependencyInjection;
using Vulpes.Perpendicularity.Core.Data;
using Vulpes.Perpendicularity.Infrastructure.Mongo;

namespace Vulpes.Perpendicularity.Infrastructure.RegistrationExtensions;

public static class InfrastructureRegistration
{
    public static IServiceCollection InjectInfrastructure(this IServiceCollection services) => services
        .AddSingleton<IMongoProvider, MongoProvider>()
        .AddTransient(typeof(IModelRepository<>), typeof(MongoRepository<>))
        ;
}