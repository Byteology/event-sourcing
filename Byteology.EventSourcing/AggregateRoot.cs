namespace Byteology.EventSourcing;

using Byteology.EventSourcing.CommandHandling;
using Byteology.EventSourcing.EventHandling;
using Byteology.GuardClauses;
using System.Reflection;

public abstract class AggregateRoot : IAggregateRoot
{
    private readonly IList<IEventContext> _uncommitedEvents = new List<IEventContext>();

    public Guid Id { get; private set; }
    public ulong Version { get; private set; }

    protected void ApplyNewEvent<TEvent>(TEvent @event, DateTimeOffset commandTimestamp)
        where TEvent : class
    {
        applyEvent(commandTimestamp, Version + 1, @event, true);
    }

    private void applyEvent(DateTimeOffset timestamp, ulong sequence, object @event, bool isNew)
    {
        Type eventType = @event.GetType();
        Type aggregateType = this.GetType();
        Type contextType = typeof(EventContext<>).MakeGenericType(eventType);
        Type handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
        MethodInfo handlerMethod = handlerType.GetMethod(nameof(IEventHandler<IEvent>.HandleEvent))!;

        object context = Activator.CreateInstance(
            contextType,
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new object[] { Id, aggregateType, timestamp, sequence, @event },
            null)!;

        if (aggregateType.IsAssignableTo(handlerType))
            handlerMethod.Invoke(this, new object[] { context });
        else 
            throw new InvalidOperationException($"The aggregate root can't handle events of type {eventType.Name} as it does not implement the proper {typeof(IEventHandler<>).Name} interface.");

        Version = sequence;

        if (isNew)
            _uncommitedEvents.Add((context as IEventContext)!);
    }

    Guid IAggregateRoot.Id { get => Id; set => Id = value; }

    void IAggregateRoot.ExecuteCommand<TCommand>(TCommand command, DateTimeOffset timestamp)
    {
        #pragma warning disable S3060 // "is" should not be used with "this" 
        // This is fine since we are testing derived classes in external code.
            if (this is ICommandable<TCommand> commandable)
                commandable.Execute(command, timestamp);
            else
                throw new InvalidOperationException($"The aggregate root can't execute the specified command as it does not inherit the {typeof(ICommandable<TCommand>).Name} interface.");
        #pragma warning restore S3060 // "is" should not be used with "this"
    }

    void IAggregateRoot.ReplayEvent(IEventContext eventContext)
    {
        Guard.Argument(eventContext, nameof(eventContext))
            .Satisfies(x => x.AggregateId == Id && x.AggregateType == this.GetType(),
                "Event should apply be for this aggregate root.")
            .Satisfies(x => x.EventSequence > Version, "The events sequence should be higher than the aggregate root version.");

        applyEvent(eventContext.Timestamp, eventContext.EventSequence, eventContext.Event, false);
    }

    IEnumerable<IEventContext> IAggregateRoot.GetUncommitedEvents() => _uncommitedEvents;

    void IAggregateRoot.MarkAllEventsAsCommited() => _uncommitedEvents.Clear();
}

