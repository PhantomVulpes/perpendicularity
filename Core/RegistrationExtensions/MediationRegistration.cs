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
        .AddTransient<CommandHandler<LogInCommand>, LogInCommandHandler>()
        .AddTransient<CommandHandler<RegisterNewUserCommand>, RegisterNewUserCommandHandler>()
        .AddTransient<CommandHandler<InitializeApplicationSettingsCommand>, InitializeApplicationSettingsCommandHandler>()
        .AddTransient<CommandHandler<ApproveUserCommand>, ApproveUserCommandHandler>()
        .AddTransient<CommandHandler<EditApplicationSettingsCommand>, EditApplicationSettingsCommandHandler>()
        ;

    private static IServiceCollection InjectQueries(this IServiceCollection services) => services
        .AddTransient<QueryHandler<GetUserByLoginCredentialsQuery, RegisteredUser>, GetUserByLoginCredentialsQueryHandler>()
        .AddTransient<QueryHandler<GetAllUsersQuery, IQueryable<RegisteredUser>>, GetAllUsersQueryHandler>()
        .AddTransient<QueryHandler<GetApplicationSettingsQuery, ApplicationSettings>, GetApplicationSettingsQueryHandler>()
        ;

    private static IServiceCollection InjectMediator(this IServiceCollection services)
    {
        _ = services.AddTransient<IMediator>((provider) =>
        {
            var mediator = new Mediator();

            _ = mediator
                .Register(provider.GetRequiredService<CommandHandler<LogInCommand>>())
                .Register(provider.GetRequiredService<CommandHandler<RegisterNewUserCommand>>())
                .Register(provider.GetRequiredService<CommandHandler<InitializeApplicationSettingsCommand>>())
                .Register(provider.GetRequiredService<CommandHandler<ApproveUserCommand>>())
                .Register(provider.GetRequiredService<CommandHandler<EditApplicationSettingsCommand>>())
                ;

            _ = mediator
                .Register(provider.GetRequiredService<QueryHandler<GetUserByLoginCredentialsQuery, RegisteredUser>>())
                .Register(provider.GetRequiredService<QueryHandler<GetAllUsersQuery, IQueryable<RegisteredUser>>>())
                .Register(provider.GetRequiredService<QueryHandler<GetApplicationSettingsQuery, ApplicationSettings>>())
                ;

            return mediator;
        });

        return services;
    }
}
