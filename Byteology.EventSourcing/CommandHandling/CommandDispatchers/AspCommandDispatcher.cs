namespace Byteology.EventSourcing.CommandHandling.CommandDispatchers;

using Byteology.EventSourcing.CommandHandling.CommandHandlerLocators;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

internal class AspCommandDispatcher : CommandDispatcherBase
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AspCommandDispatcher(
        ICommandHandlerLocator commandHandlerLocator,
        IHttpContextAccessor httpContextAccessor) 
            : base(commandHandlerLocator)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override string? GetIssuer() =>
        _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}
