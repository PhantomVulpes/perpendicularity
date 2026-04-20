using Microsoft.Extensions.DependencyInjection;

namespace Vulpes.Perpendicularity.Infrastructure.RegistrationExtensions;

public static class InfrastructureRegistration
{
    public static IServiceCollection InjectInfrastructure(this IServiceCollection services) => services
        .InjectMongoServices()
        .InjectQueryProviders()
        ;


}