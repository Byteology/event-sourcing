namespace Byteology.EventSourcing.EventHandling;

public sealed class EventContext<TEvent> : IEventContext
    where TEvent : IEvent
{
    public Guid AggregateId { get; }
    public Type AggregateType { get; }
    public DateTimeOffset Timestamp { get; }
    public ulong EventSequence { get; }
    public TEvent Event { get; }

    internal EventContext(
        Guid aggregateId, 
        Type aggregateType, 
        DateTimeOffset eventTimestamp,
        ulong eventSequence,
        TEvent @event)
    {
        AggregateId = aggregateId;
        AggregateType = aggregateType;
        Timestamp = eventTimestamp;
        EventSequence = eventSequence;
        Event = @event;
    }

    IEvent IEventContext.Event => Event;
}
