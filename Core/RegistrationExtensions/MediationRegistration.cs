using Microsoft.Extensions.DependencyInjection;
using Vulpes.Electrum.Domain.Commanding;
using Vulpes.Electrum.Domain.Mediation;
using Vulpes.Electrum.Domain.Querying;
using Vulpes.Perpendicularity.Core.Commands;
using Vulpes.Perpendicularity.Core.Models;
using Vulpes.Perpendicularity.Core.QueriedModels;
using Vulpes.Perpendicularity.Core.Queries;

namespace Vulpes.Perpendicularity.Core.RegistrationExtensions;

public static class MediationRegistration
{
    public static IServiceCollection InjectCommands(this IServiceCollection services) => services
        .AddTransient<CommandHandler<LogInCommand>, LogInCommandHandler>()
        .AddTransient<CommandHandler<RegisterNewUserCommand>, RegisterNewUserCommandHandler>()
        .AddTransient<CommandHandler<InitializeApplicationSettingsCommand>, InitializeApplicationSettingsCommandHandler>()
        .AddTransient<CommandHandler<ApproveUserCommand>, ApproveUserCommandHandler>()
        .AddTransient<CommandHandler<RejectUserCommand>, RejectUserCommandHandler>()
        .AddTransient<CommandHandler<EditApplicationSettingsCommand>, EditApplicationSettingsCommandHandler>()
        .AddTransient<CommandHandler<AddExternalProjectCommand>, AddExternalProjectCommandHandler>()
        .AddTransient<CommandHandler<DeleteExternalProjectCommand>, DeleteExternalProjectCommandHandler>()
        .AddTransient<CommandHandler<EditUserCommand>, EditUserCommandHandler>()
        .AddTransient<CommandHandler<AddUserDownloadCommand>, AddUserDownloadCommandHandler>()
        ;

    public static IServiceCollection InjectQueries(this IServiceCollection services) => services
        .AddTransient<QueryHandler<GetUserByLoginCredentialsQuery, RegisteredUser>, GetUserByLoginCredentialsQueryHandler>()
        .AddTransient<QueryHandler<GetAllUsersQuery, IQueryable<RegisteredUser>>, GetAllUsersQueryHandler>()
        .AddTransient<QueryHandler<GetUserByKeyQuery, RegisteredUser>, GetUserByKeyQueryHandler>()
        .AddTransient<QueryHandler<GetApplicationSettingsQuery, ApplicationSettings>, GetApplicationSettingsQueryHandler>()
        .AddTransient<QueryHandler<GetDirectoryContentsQuery, DirectoryContents>, GetDirectoryContentsQueryHandler>()
        .AddTransient<QueryHandler<GetDirectoryConfigurationsQuery, IEnumerable<DirectoryConfiguration>>, GetDirectoryConfigurationsQueryHandler>()
        .AddTransient<QueryHandler<GetFileForDownloadQuery, FileForDownload>, GetFileForDownloadQueryHandler>()
        .AddTransient<QueryHandler<GetFilesAsZipQuery, ZipFileForDownload>, GetFilesAsZipQueryHandler>()
        .AddTransient<QueryHandler<GetAllExternalProjectsQuery, IQueryable<ExternalProject>>, GetAllExternalProjectsQueryHandler>()
        ;

    public static IServiceCollection InjectMediator(this IServiceCollection services)
    {
        _ = services.AddTransient<IMediator>((provider) =>
        {
            var mediator = new Mediator();

            _ = mediator
                .Register(provider.GetRequiredService<CommandHandler<LogInCommand>>())
                .Register(provider.GetRequiredService<CommandHandler<RegisterNewUserCommand>>())
                .Register(provider.GetRequiredService<CommandHandler<InitializeApplicationSettingsCommand>>())
                .Register(provider.GetRequiredService<CommandHandler<ApproveUserCommand>>())
                .Register(provider.GetRequiredService<CommandHandler<RejectUserCommand>>())
                .Register(provider.GetRequiredService<CommandHandler<EditApplicationSettingsCommand>>())
                .Register(provider.GetRequiredService<CommandHandler<AddExternalProjectCommand>>())
                .Register(provider.GetRequiredService<CommandHandler<DeleteExternalProjectCommand>>())
                .Register(provider.GetRequiredService<CommandHandler<EditUserCommand>>())
                .Register(provider.GetRequiredService<CommandHandler<AddUserDownloadCommand>>())
                ;

            _ = mediator
                .Register(provider.GetRequiredService<QueryHandler<GetUserByLoginCredentialsQuery, RegisteredUser>>())
                .Register(provider.GetRequiredService<QueryHandler<GetAllUsersQuery, IQueryable<RegisteredUser>>>())
                .Register(provider.GetRequiredService<QueryHandler<GetUserByKeyQuery, RegisteredUser>>())
                .Register(provider.GetRequiredService<QueryHandler<GetApplicationSettingsQuery, ApplicationSettings>>())
                .Register(provider.GetRequiredService<QueryHandler<GetDirectoryContentsQuery, DirectoryContents>>())
                .Register(provider.GetRequiredService<QueryHandler<GetDirectoryConfigurationsQuery, IEnumerable<DirectoryConfiguration>>>())
                .Register(provider.GetRequiredService<QueryHandler<GetFileForDownloadQuery, FileForDownload>>())
                .Register(provider.GetRequiredService<QueryHandler<GetFilesAsZipQuery, ZipFileForDownload>>())
                .Register(provider.GetRequiredService<QueryHandler<GetAllExternalProjectsQuery, IQueryable<ExternalProject>>>())
                ;

            return mediator;
        });

        return services;
    }
}
