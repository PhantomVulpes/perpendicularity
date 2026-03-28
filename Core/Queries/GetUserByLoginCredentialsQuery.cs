using System.Security.Authentication;
using Vulpes.Electrum.Domain.Querying;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Perpendicularity.Core.Data;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Core.Queries;

public record GetUserByLoginCredentialsQuery(string FirstName, string LastName, string PasswordRaw) : Query;
public class GetUserByLoginCredentialsQueryHandler : QueryHandler<GetUserByLoginCredentialsQuery, RegisteredUser>
{
    private readonly IQueryProvider<RegisteredUser> queryProvider;
    private readonly IKnoxHasher knoxHasher;

    private readonly static string failureMessage = "Provided name or password was incorrect. Try again.";

    public GetUserByLoginCredentialsQueryHandler(IQueryProvider<RegisteredUser> queryProvider, IKnoxHasher knoxHasher)
    {
        this.queryProvider = queryProvider;
        this.knoxHasher = knoxHasher;
    }

    protected override async Task<RegisteredUser> InternalRequestAsync(GetUserByLoginCredentialsQuery query)
    {
        var user = (await queryProvider.BeginQueryAsync())
            .SingleOrDefault(user =>
                user.FirstName.Equals(query.FirstName, StringComparison.CurrentCultureIgnoreCase)
                && user.LastName.Equals(query.LastName, StringComparison.CurrentCultureIgnoreCase))
                ?? throw new InvalidCredentialException(failureMessage)
            ;

        var passwordMatch = knoxHasher.CompareHash(user.PasswordHash, query.PasswordRaw);

        if (!passwordMatch)
        {
            throw new InvalidCredentialException(failureMessage);
        }

        return user;
    }
}