namespace Byteology.EventSourcing.CommandHandling.CommandMetadataFactories;

using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public class AspCommandMetadataFactory : ICommandMetadataFactory
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AspCommandMetadataFactory(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public CommandMetadata CreateCommandMetadata()
        => new (_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, DateTimeOffset.Now);
}
