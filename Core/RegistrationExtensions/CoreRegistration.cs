using Microsoft.Extensions.DependencyInjection;
using Vulpes.Electrum.Domain.Security;

namespace Vulpes.Perpendicularity.Core.RegistrationExtensions;

public static class CoreRegistration
{
    public static IServiceCollection InjectCore(this IServiceCollection services) => services
        .AddTransient<IKnoxHasher, KnoxHasher>()
        .InjectLazy()
        .InjectCommands()
        .InjectQueries()
        .InjectMediator()
        ;
}