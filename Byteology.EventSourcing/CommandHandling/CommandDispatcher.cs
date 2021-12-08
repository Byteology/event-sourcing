namespace Byteology.EventSourcing.CommandHandling;

using Byteology.EventSourcing.CommandHandling.CommandHandlerLocators;
using Byteology.EventSourcing.CommandHandling.CommandMetadataFactories;
using Byteology.EventSourcing.EventPublishing;
using Byteology.EventSourcing.EventStorage;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IEventStore _eventStore;
    private readonly IEventPublisher _eventPublisher;
    private readonly ICommandMetadataFactory _commandMetadataFactory;
    private readonly ICommandHandlerLocator _commandHandlerLocator;

    protected CommandDispatcher(
        IEventStore eventStore,
        IEventPublisher eventPublisher,
        ICommandMetadataFactory commandMetadataFactory,
        ICommandHandlerLocator commandHandlerLocator)
    {
        _eventStore = eventStore;
        _eventPublisher = eventPublisher;
        _commandMetadataFactory = commandMetadataFactory;
        _commandHandlerLocator = commandHandlerLocator;
    }

    public void Dispatch<TAggregateRoot>(ICommand<TAggregateRoot> command) 
        where TAggregateRoot : IAggregateRoot, new()
    {
        Type commandType = command.GetType();
        Type aggregateRootType = typeof(TAggregateRoot);

        object handler = getEventHandler(commandType, aggregateRootType);

        execute((dynamic)handler, (dynamic)command);
    }

    private object getEventHandler(Type commandType, Type aggregateRootType)
    {
        object handler = typeof(ICommandHandlerLocator)
            .GetMethod(nameof(ICommandHandlerLocator.Locate))!
            .MakeGenericMethod(commandType, aggregateRootType)!
            .Invoke(_commandHandlerLocator, null)!;

        return handler;
    }

    private void execute<TCommand, TAggregateRoot>(ICommandHandler<TCommand, TAggregateRoot> handler, TCommand command)
        where TCommand : ICommand<TAggregateRoot>
        where TAggregateRoot : IAggregateRoot, new()
    {
        CommandMetadata metadata = _commandMetadataFactory.CreateCommandMetadata();
        TAggregateRoot aggregateRoot = buildAggregateRoot<TAggregateRoot>(command.EventStreamId);
        handler.HandleCommand(command, metadata, aggregateRoot);
        IEnumerable<EventRecord> eventRecords = persistNewEvents(aggregateRoot, metadata);
        publishEvents(eventRecords);
    }

    private TAggregateRoot buildAggregateRoot<TAggregateRoot>(Guid eventStreamId)
        where TAggregateRoot : IAggregateRoot, new()
    {
        AggregateRootFactory builder = new(_eventStore);
        TAggregateRoot root = builder.Build<TAggregateRoot>(eventStreamId);
        return root;
    }

    private IEnumerable<EventRecord> persistNewEvents(IAggregateRoot aggregateRoot, CommandMetadata commandMetadata)
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

        eventRecords = _eventStore.AddEvents(eventRecords);
        aggregateRoot.MarkAllEventsAsCommited();

        return eventRecords;
    }

    private void publishEvents(IEnumerable<EventRecord> eventRecords)
    {
        _eventPublisher.PublishEvents(eventRecords);
    }
}
