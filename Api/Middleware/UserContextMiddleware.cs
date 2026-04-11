using System.Security.Claims;
using Vulpes.Electrum.Domain.Data;
using Vulpes.Perpendicularity.Core.Models;

namespace Vulpes.Perpendicularity.Api.Middleware;

public class UserContextMiddleware : IMiddleware
{
    public static string ContextKey => $"{nameof(RegisteredUser)}.{nameof(RegisteredUser.Key)}".ToLower();

    private readonly IModelRepository<RegisteredUser> userRepository;

    public UserContextMiddleware(IModelRepository<RegisteredUser> userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            context.Items[ContextKey] = RegisteredUser.Empty;
            await next(context);
        }
        else
        {
            var key = Guid.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? context.User.FindFirst("UserId")?.Value!);
            var user = await userRepository.GetAsync(key);

            context.Items[ContextKey] = user;
            await next(context);
        }
    }
}