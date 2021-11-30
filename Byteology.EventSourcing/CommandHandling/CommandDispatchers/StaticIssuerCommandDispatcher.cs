namespace Byteology.EventSourcing.CommandHandling.CommandDispatchers;

using Byteology.EventSourcing.CommandHandling.CommandHandlerLocators;

public class StaticIssuerCommandDispatcher : CommandDispatcherBase
{
    private readonly string? _issuer;

    public StaticIssuerCommandDispatcher(
        ICommandHandlerLocator commandHandlerLocator,
        string? issuer)
            : base(commandHandlerLocator)
    {
        _issuer = issuer;
    }

    protected override string? GetIssuer() => _issuer;
}
