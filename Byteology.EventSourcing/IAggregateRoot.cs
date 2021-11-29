namespace Byteology.EventSourcing;

using Byteology.EventSourcing.Storage;

public interface IAggregateRoot
{
    Guid Id { get; set; }
    ulong Version { get; }

    IEnumerable<IEvent> GetUncommitedEvents();
    void MarkAllEventsAsCommited();

    void ReplayEvent(PersistedEventRecord record);
}
