using Microsoft.Extensions.DependencyInjection;
using Vulpes.Perpendicularity.Core.RegistrationExtensions.Models;

namespace Vulpes.Perpendicularity.Core.RegistrationExtensions;

public static class CommonRegistration
{
    public static IServiceCollection InjectLazy(this IServiceCollection services) => services.AddTransient(typeof(Lazy<>), typeof(Lazier<>));
}
