namespace Byteology.EventSourcing.EventHandling;

using Byteology.EventSourcing.EventHandling.Storage;

public sealed class EventHandlerContext<TEvent> : IEventStreamRecord
    where TEvent : IEvent
{
    private readonly Guid _aggregateRootId;

    public DateTimeOffset EventTimestamp { get; }
    public ulong EventSequence { get; }
    public TEvent Event { get; }

    /// <summary>
    /// This is here to force the aggregate roots to use the 
    /// internal event handler when executing commands.
    /// </summary>
    internal EventHandlerContext(
        Guid aggregateRootId,
        DateTimeOffset eventTimestamp,
        ulong eventSequence,
        TEvent @event)
    {
        _aggregateRootId = aggregateRootId;
        EventTimestamp = eventTimestamp;
        EventSequence = eventSequence;
        Event = @event;
    }

    IEvent IEventStreamRecord.Event => Event;
    Guid IEventStreamRecord.AggregateRootId => _aggregateRootId;
}
