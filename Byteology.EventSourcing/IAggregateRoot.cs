namespace Byteology.EventSourcing;

using Byteology.EventSourcing.EventStorage;

public interface IAggregateRoot
{
    Guid EventStreamId { get; set; }
    ulong EventStreamPosition { get; }

    void ApplyNewEvent(IEvent @event);

    IEnumerable<IEvent> GetUncommitedEvents();
    void MarkAllEventsAsCommited();

    void ReplayEvent(EventRecord record);
}
