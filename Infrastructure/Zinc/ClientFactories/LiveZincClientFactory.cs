using Infrastructure.Zinc;

namespace Vulpes.Perpendicularity.Infrastructure.Zinc.ClientFactories;

public class LiveZincClientFactory : IZincClientFactory
{
    private readonly IZincClient zincClient;

    public LiveZincClientFactory(IZincClient zincClient)
    {
        this.zincClient = zincClient;
    }

    public IZincClient BuildClient() => zincClient;
}
