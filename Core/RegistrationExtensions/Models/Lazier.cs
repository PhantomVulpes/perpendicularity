using Microsoft.Extensions.DependencyInjection;

namespace Vulpes.Perpendicularity.Core.RegistrationExtensions.Models;

public class Lazier<T> : Lazy<T> where T : class
{
    public Lazier(IServiceProvider provider) : base(provider.GetRequiredService<T>) { }
}
