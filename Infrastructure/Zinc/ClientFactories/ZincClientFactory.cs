using Infrastructure.Zinc;
using Vulpes.Perpendicularity.Core.Configuration;

namespace Vulpes.Perpendicularity.Infrastructure.Zinc.ClientFactories;

public class ZincClientFactory : IZincClientFactory
{
    private readonly Lazy<LiveZincClientFactory> liveZincClientFactoryLazy;
    private readonly Lazy<MockZincClientFactory> mockZincClientFactoryLazy;

    public ZincClientFactory(Lazy<LiveZincClientFactory> liveZincClientFactoryLazy, Lazy<MockZincClientFactory> mockZincClientFactoryLazy)
    {
        this.liveZincClientFactoryLazy = liveZincClientFactoryLazy;
        this.mockZincClientFactoryLazy = mockZincClientFactoryLazy;
    }

    public IZincClient BuildClient()
    {
        if (ApplicationConfiguration.IsRelease)
        {
            return liveZincClientFactoryLazy.Value.BuildClient();
        }

        return mockZincClientFactoryLazy.Value.BuildClient();
    }
}