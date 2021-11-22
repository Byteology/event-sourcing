namespace Byteology.EventSourcing;

using Byteology.EventSourcing.CommandHandling;
using Byteology.EventSourcing.EventHandling;
using Byteology.EventSourcing.EventHandling.Storage;
using System.Reflection;

public abstract class AggregateRoot : IAggregateRoot
{
    private readonly IList<IEventStreamRecord> _newEvents = new List<IEventStreamRecord>();

    public Guid Id { get; private set; }
    public ulong Version { get; private set; }

    protected void ApplyNewEvent<TEvent, TCommand>(TEvent @event, CommandContext<TCommand> commandContext)
        where TEvent : IEvent
        where TCommand : ICommand
    {
        applyEvent(commandContext.CommandTimestamp, Version + 1, @event, true);
    }

    private void applyEvent(DateTimeOffset timestamp, ulong sequence, IEvent @event, bool isNew)
    {
        Type eventType = @event.GetType();
        Type aggregateType = this.GetType();
        Type contextType = typeof(EventHandlerContext<>).MakeGenericType(eventType);
        Type handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
        MethodInfo handlerMethod = handlerType.GetMethod(nameof(IEventHandler<IEvent>.HandleEvent))!;

        object context = Activator.CreateInstance(
            contextType,
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new object[] { timestamp, sequence, @event },
            null)!;

        if (aggregateType.IsAssignableTo(handlerType))
            handlerMethod.Invoke(this, new object[] { context });
        else 
            throw new InvalidOperationException($"The aggregate root can't handle the specified event as it does not implement the {handlerType} interface.");

        Version = sequence;

        if (isNew)
            _newEvents.Add((context as IEventStreamRecord)!);
    }

    Guid IAggregateRoot.Id { get => Id; set => Id = value; }

    void IAggregateRoot.ReplayEvent(IEventStreamRecord @event) =>
        applyEvent(@event.EventTimestamp, @event.EventSequence, @event.Event, false);

    IEnumerable<IEventStreamRecord> IAggregateRoot.GetNewEvents() => _newEvents;
}

