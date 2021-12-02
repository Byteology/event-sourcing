﻿namespace Byteology.EventSourcing.CommandHandling;

using Byteology.EventSourcing.EventStorage;

public abstract class CommandHandler<TCommand, TAggregateRoot> : ICommandHandler<TCommand, TAggregateRoot>
    where TCommand : ICommand<TAggregateRoot>
    where TAggregateRoot : IAggregateRoot, new()
{
    private readonly IEventStore _eventStore;

    protected CommandHandler(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    protected abstract void ExecuteCommand(TCommand command, CommandMetadata metadata, TAggregateRoot aggregateRoot);

    protected virtual TAggregateRoot BuildAggregateRoot(Guid eventStreamId)
    {
        AggregateRootFactory builder = new(_eventStore);
        TAggregateRoot root = builder.Build<TAggregateRoot>(eventStreamId);
        return root;
    }

    protected virtual void PersistNewEvents(IAggregateRoot aggregateRoot, CommandMetadata commandMetadata)
    {
        Guid transactionId = Guid.NewGuid();

        IEnumerable<IEvent> newEvents = aggregateRoot.GetUncommitedEvents();

        ulong nextEventPosition = aggregateRoot.EventStreamPosition - (ulong)newEvents.Count() + 1;

        IEnumerable<EventRecord> eventRecords = newEvents.Select(e =>
            new EventRecord(
                e,
                new EventMetadata(
                    aggregateRoot.EventStreamId,
                    aggregateRoot.GetType(),
                    nextEventPosition++,
                    commandMetadata.Timestamp,
                    commandMetadata.Issuer,
                    transactionId
                )
            )
        );

        _eventStore.AddEvents(eventRecords);
        aggregateRoot.MarkAllEventsAsCommited();
    }

    void ICommandHandler<TCommand, TAggregateRoot>.HandleCommand(TCommand command, CommandMetadata metadata)
    {
        TAggregateRoot aggregateRoot = BuildAggregateRoot(command.EventStreamId);

        ExecuteCommand(command, metadata, aggregateRoot);

        PersistNewEvents(aggregateRoot, metadata);
    }
}
