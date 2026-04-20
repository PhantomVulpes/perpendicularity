using Infrastructure.Zinc;

namespace Vulpes.Perpendicularity.Infrastructure.Zinc.ClientFactories;

public class MockZincClient : IZincClient
{
    public Task AddCommentAsync(AddCommentToTicketRequest body) => throw new NotImplementedException();
    public Task AddCommentAsync(AddCommentToTicketRequest body, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<string> CreateAsync(CreateNewProjectRequest body) => throw new NotImplementedException();
    public Task<string> CreateAsync(CreateNewProjectRequest body, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<Project> CreateTicketAsync(string projectShorthand, CreateTicketRequest body) => throw new NotImplementedException();
    public Task<Project> CreateTicketAsync(string projectShorthand, CreateTicketRequest body, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task EditAsync(EditTicketRequest body) => throw new NotImplementedException();
    public Task EditAsync(EditTicketRequest body, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<LoginResponse> LoginAsync(LoginRequest body) => throw new NotImplementedException();
    public Task<LoginResponse> LoginAsync(LoginRequest body, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<ICollection<Project>> ProjectsAllAsync() => throw new NotImplementedException();
    public Task<ICollection<Project>> ProjectsAllAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<Project> ProjectsAsync(string projectShorthand) => throw new NotImplementedException();
    public Task<Project> ProjectsAsync(string projectShorthand, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<string> RegisterAsync(RegisterNewUserRequest body) => throw new NotImplementedException();
    public Task<string> RegisterAsync(RegisterNewUserRequest body, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<RegisteredUser> UserAsync(Guid userKey) => throw new NotImplementedException();
    public Task<RegisteredUser> UserAsync(Guid userKey, CancellationToken cancellationToken) => throw new NotImplementedException();
}
