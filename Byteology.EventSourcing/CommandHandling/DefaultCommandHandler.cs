namespace Byteology.EventSourcing.CommandHandling;

using Byteology.EventSourcing.EventHandling;
using Byteology.EventSourcing.EventHandling.Publishing;
using Byteology.EventSourcing.EventHandling.Storage;

public class DefaultCommandHandler<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    private readonly IEventStore _eventStore;
    private readonly IEventBus _eventBus;

    public DefaultCommandHandler(IEventStore eventStore, IEventBus eventBus)
    {
        _eventStore = eventStore;
        _eventBus = eventBus;
    }

    public void HandleCommand(CommandContext<TCommand> context)
    {
        OnBeforeHandling(context);

        AggregateRootBuilder rootBuilder = new(_eventStore);
        IAggregateRoot aggregateRoot = rootBuilder.Build(context.Command.AggregateRootId, context.Command.AggregateType);
        OnAggregateRootBuilt(context, aggregateRoot);

        if (aggregateRoot is ICommandHandler<TCommand> rootAsHandler)
        {
            rootAsHandler.HandleCommand(context);
            OnAggregateRootHandledCommand(context, aggregateRoot);
        }
        else
            throw new InvalidOperationException($"The aggregate root can't execute the specified command as it does not inherit the {typeof(ICommandHandler<TCommand>)} interface.");

        IEnumerable<IEventStreamRecord> eventStream = aggregateRoot.GetNewEvents();
        _eventStore.AddEvents(eventStream);
        OnEventsPersisted(context, eventStream);

        foreach (IEventStreamRecord eventRecord in eventStream)
        {
            IEventContext<IEvent> eventContext =
                EventContextFactory.Create(eventRecord, context.Command.AggregateRootId, context.Command.AggregateType);
            _eventBus.Publish(eventContext);
        }

        OnEventsPublished(context, eventStream);
    }

    protected virtual void OnBeforeHandling(
        CommandContext<TCommand> context) { }
    protected virtual void OnAggregateRootBuilt(
        CommandContext<TCommand> context, 
        IAggregateRoot aggregateRoot) { }
    protected virtual void OnAggregateRootHandledCommand(
        CommandContext<TCommand> context,
        IAggregateRoot aggregateRoot) { }
    protected virtual void OnEventsPersisted(
        CommandContext<TCommand> context, 
        IEnumerable<IEventStreamRecord> eventStream) { }
    protected virtual void OnEventsPublished(
        CommandContext<TCommand> context, 
        IEnumerable<IEventStreamRecord> eventStream) { }

}
