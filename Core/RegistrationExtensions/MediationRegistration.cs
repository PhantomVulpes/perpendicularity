using Microsoft.Extensions.DependencyInjection;
using Vulpes.Electrum.Domain.Commanding;
using Vulpes.Electrum.Domain.Mediation;
using Vulpes.Perpendicularity.Core.Commands;

namespace Vulpes.Perpendicularity.Core.RegistrationExtensions;

public static class MediationRegistration
{
    public static IServiceCollection InjectDomain(this IServiceCollection services) => services
    .InjectCommands()
    .InjectQueries()
    .InjectMediator()
    ;

    private static IServiceCollection InjectCommands(this IServiceCollection services) => services
        .AddTransient<CommandHandler<RegisterNewUserCommand>, RegisterNewUserCommandHandler>()
        ;

    private static IServiceCollection InjectQueries(this IServiceCollection services) => services;

    private static IServiceCollection InjectMediator(this IServiceCollection services)
    {
        _ = services.AddTransient<IMediator>((provider) =>
        {
            var mediator = new Mediator();

            _ = mediator
                .Register(provider.GetRequiredService<CommandHandler<RegisterNewUserCommand>>())
                ;

            _ = mediator
                // For reference when I make a query.
                // .Register(provider.GetRequiredService<QueryHandler<GetUserByLoginCredentials, ZincUser>>())
                ;

            return mediator;
        });

        return services;
    }
}
