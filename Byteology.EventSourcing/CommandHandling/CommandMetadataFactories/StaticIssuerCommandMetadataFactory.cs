namespace Byteology.EventSourcing.CommandHandling.CommandMetadataFactories;

public class StaticIssuerCommandMetadataFactory : ICommandMetadataFactory
{
    private readonly string? _issuer;

    public StaticIssuerCommandMetadataFactory(string? issuer)
    {
        _issuer = issuer;
    }

    public CommandMetadata CreateCommandMetadata()
        => new(_issuer, DateTimeOffset.Now);
}
