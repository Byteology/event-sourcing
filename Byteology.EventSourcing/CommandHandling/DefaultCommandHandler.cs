namespace Byteology.EventSourcing.CommandHandling;

using Byteology.EventSourcing.Storage;

public class DefaultCommandHandler<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    private readonly IEventStore _eventStore;

    public DefaultCommandHandler(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public virtual void HandleCommand(TCommand command, CommandMetadata metadata)
    {
        IAggregateRoot aggregateRoot = BuildAggregateRoot(command.AggregateRootId, command.AggregateRootType);
        aggregateRoot.ExecuteCommand(command, metadata);

        IEnumerable<IEvent> newEvents = aggregateRoot.GetUncommitedEvents();
        if (!newEvents.Any())
            throw new InvalidOperationException("The command was not executed as the aggregate root failed to apply any new events.");

        PersistNewEvents(aggregateRoot, metadata);
    }

    protected virtual IAggregateRoot BuildAggregateRoot(Guid id, Type type)
    {
        AggregateRootFactory builder = new(_eventStore);
        IAggregateRoot root = builder.Build(id, type);
        return root;
    }

    protected virtual void PersistNewEvents(IAggregateRoot aggregateRoot, CommandMetadata commandMetadata)
    {
        IEnumerable<IEvent> newEvents = aggregateRoot.GetUncommitedEvents();

        ulong nextEventPosition = aggregateRoot.Version - (ulong)newEvents.Count() + 1;

        IEnumerable<EventRecord> eventRecords = newEvents.Select(e =>
            new EventRecord(
                e,
                new EventMetadata(
                    aggregateRoot.Id,
                    aggregateRoot.GetType(),
                    nextEventPosition++,
                    commandMetadata.Timestamp,
                    commandMetadata.Issuer,
                    Guid.NewGuid()
                )
            )
        );

        _eventStore.AddEvents(eventRecords);
        aggregateRoot.MarkAllEventsAsCommited();
    }

    private sealed record EventMetadata(Guid AggregateRootId, Type AggregateRootType, ulong EventStreamPosition, DateTimeOffset Timestamp, string? Issuer, Guid TransactionId) : IEventMetadata;
}
