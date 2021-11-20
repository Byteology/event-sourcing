namespace Byteology.EventSourcing.CommandHandling;

using Byteology.EventSourcing.EventHandling;
using Byteology.EventSourcing.Storage;

public class DefaultCommandHandler<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    private readonly IEventStore _eventStore;

    public DefaultCommandHandler(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public virtual IEnumerable<IEventContext> HandleCommand(TCommand command, DateTimeOffset timestamp)
    {
        using IEventStoreContext eventStoreContext = _eventStore.CreateContext();

        AggregateRootBuilder rootBuilder = new(eventStoreContext);
        IAggregateRoot root = rootBuilder.Build(command.AggregateRootId, command.AggregateType);

        root.ExecuteCommand(command, timestamp);

        IEnumerable<IEventContext> eventStream = root.GetUncommitedEvents();
        eventStoreContext.AddEvents(eventStream);
        root.MarkAllEventsAsCommited();

        return eventStream;
    }
}
