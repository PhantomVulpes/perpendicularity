using Microsoft.Extensions.DependencyInjection;
using Vulpes.Electrum.Domain.Commanding;
using Vulpes.Electrum.Domain.Mediation;
using Vulpes.Electrum.Domain.Querying;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Commands;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Core.Queries;

namespace Vulpes.Perpendicularity.Core.RegistrationExtensions;

public static class MediationRegistration
{
    public static IServiceCollection InjectDomain(this IServiceCollection services) => services
        .AddTransient<IKnoxHasher, KnoxHasher>()
        .InjectCommands()
        .InjectQueries()
        .InjectMediator()
        ;

    private static IServiceCollection InjectCommands(this IServiceCollection services) => services
        .AddTransient<CommandHandler<RegisterNewUserCommand>, RegisterNewUserCommandHandler>()
        .AddTransient<CommandHandler<InitializeApplicationSettingsCommand>, InitializeApplicationSettingsCommandHandler>()
        .AddTransient<CommandHandler<ApproveUserCommand>, ApproveUserCommandHandler>()
        ;

    private static IServiceCollection InjectQueries(this IServiceCollection services) => services
        .AddTransient<QueryHandler<GetUserByLoginCredentialsQuery, RegisteredUser>, GetUserByLoginCredentialsQueryHandler>()
        .AddTransient<QueryHandler<GetAllUsersQuery, IQueryable<RegisteredUser>>, GetAllUsersQueryHandler>()
        ;

    private static IServiceCollection InjectMediator(this IServiceCollection services)
    {
        _ = services.AddTransient<IMediator>((provider) =>
        {
            var mediator = new Mediator();

            _ = mediator
                .Register(provider.GetRequiredService<CommandHandler<RegisterNewUserCommand>>())
                .Register(provider.GetRequiredService<CommandHandler<InitializeApplicationSettingsCommand>>())
                .Register(provider.GetRequiredService<CommandHandler<ApproveUserCommand>>())
                ;

            _ = mediator
                .Register(provider.GetRequiredService<QueryHandler<GetUserByLoginCredentialsQuery, RegisteredUser>>())
                .Register(provider.GetRequiredService<QueryHandler<GetAllUsersQuery, IQueryable<RegisteredUser>>>())
                ;

            return mediator;
        });

        return services;
    }
}
