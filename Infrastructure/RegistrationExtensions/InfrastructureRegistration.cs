using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Zinc;

namespace Vulpes.Perpendicularity.Infrastructure.RegistrationExtensions;

public static class InfrastructureRegistration
{
    public static IServiceCollection InjectInfrastructure(this IServiceCollection services) => services
        .InjectZincServices()
        .InjectMongoServices()
        .InjectQueryProviders()
        ;


}