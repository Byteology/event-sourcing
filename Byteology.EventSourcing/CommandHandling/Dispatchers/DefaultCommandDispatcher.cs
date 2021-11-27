namespace Byteology.EventSourcing.CommandHandling.Dispatchers;

using Byteology.EventSourcing.Storage;

public class DefaultCommandDispatcher : CommandDispatcherBase
{
    private readonly IEventStore _eventStore;
    private readonly string? _issuer;

    protected DefaultCommandDispatcher(IEventStore eventStore, string issuer)
    {
        _eventStore = eventStore;
        _issuer = issuer;
    }

    protected override CommandMetadata CreateCommandMetadata()
        => new (_issuer, DateTimeOffset.Now);

    protected override ICommandHandler<TCommand> GetCommandHandler<TCommand>(TCommand command)
        => new DefaultCommandHandler<TCommand>(_eventStore);
}
