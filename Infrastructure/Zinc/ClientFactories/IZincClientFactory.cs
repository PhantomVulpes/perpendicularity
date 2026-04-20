using Infrastructure.Zinc;

namespace Vulpes.Perpendicularity.Infrastructure.Zinc.ClientFactories;

public interface IZincClientFactory
{
    IZincClient BuildClient();
}
