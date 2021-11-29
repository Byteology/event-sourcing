namespace Byteology.EventSourcing;

public interface IAggregateRoot
{
    Guid Id { get; set; }
    ulong Version { get; }

    IEnumerable<IEvent> GetUncommitedEvents();
    void MarkAllEventsAsCommited();

    void ReplayEvent(PersistedEventRecord record);
}
