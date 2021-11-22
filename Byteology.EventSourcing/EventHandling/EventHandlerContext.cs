namespace Byteology.EventSourcing.EventHandling;

using Byteology.EventSourcing.EventHandling.Storage;

public sealed class EventHandlerContext<TEvent> : IEventStreamRecord
    where TEvent : IEvent
{
    public DateTimeOffset EventTimestamp { get; }
    public ulong EventSequence { get; }
    public TEvent Event { get; }

    /// <summary>
    /// This is here to force the aggregate roots to use the 
    /// internal event handler when executing commands.
    /// </summary>
    internal EventHandlerContext(
        DateTimeOffset eventTimestamp,
        ulong eventSequence,
        TEvent @event)
    {
        EventTimestamp = eventTimestamp;
        EventSequence = eventSequence;
        Event = @event;
    }

    IEvent IEventStreamRecord.Event => Event;
}
