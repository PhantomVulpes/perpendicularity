using Vulpes.Electrum.Domain.Security;

namespace Vulpes.Perpendicularity.Test.Doubles.Electrum.Domain.Security;

public class TestKnoxHasher : IKnoxHasher
{
    public List<(string ProvidedPassword, bool GrantAccess)> HashResults { get; init; } = [];
    public string MockedHashResult { get; set; } = "testing-happened";

    public bool CompareHash(string retrievedHash, string providedPassword) => HashResults.Single(hr => hr.ProvidedPassword == providedPassword).GrantAccess;

    public string HashPassword(string password) => MockedHashResult;
}