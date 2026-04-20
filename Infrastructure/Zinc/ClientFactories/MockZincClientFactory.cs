using Infrastructure.Zinc;

namespace Vulpes.Perpendicularity.Infrastructure.Zinc.ClientFactories;

public class MockZincClientFactory : IZincClientFactory
{
    public IZincClient BuildClient() => throw new NotImplementedException();
}
