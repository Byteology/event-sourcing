namespace Byteology.EventSourcing.EntityFramework.EventStorage;

using Byteology.EventSourcing.EventHandling;
using Byteology.EventSourcing.EventHandling.Storage;

internal class EventStreamRecord : IEventStreamRecord
{
    public Guid AggregateRootId { get; set; }
    public IEvent Event { get; set; }
    public DateTimeOffset EventTimestamp { get; set; }
    public ulong EventSequence { get; set; }

    public EventStreamRecord(Guid aggregateRootId, IEvent @event, DateTimeOffset timestamp, ulong sequence)
    {
        AggregateRootId = aggregateRootId;
        Event = @event;
        EventTimestamp = timestamp;
        EventSequence = sequence;
    }
}
