namespace Byteology.EventSourcing.CommandHandling;

using Byteology.EventSourcing.EventHandling.Publishing;
using Byteology.EventSourcing.EventHandling.Storage;

public class DefaultCommandDispatcher : CommandDispatcherBase
{
    private readonly IEventStore _eventStore;
    private readonly IEventBus _eventBus;

    public DefaultCommandDispatcher(
        IEventStore eventStore,
        IEventBus eventBus,
        Func<DateTimeOffset> dateTimeProvider) : base(dateTimeProvider)
    {
        _eventStore = eventStore;
        _eventBus = eventBus;
    }

    protected override ICommandHandler<TCommand> GetCommandHandler<TCommand>(TCommand command)
        => new DefaultCommandHandler<TCommand>(_eventStore, _eventBus);
}
