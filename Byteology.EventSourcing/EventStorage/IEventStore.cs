namespace Byteology.EventSourcing.EventStorage;

public interface IEventStore
{
    IEnumerable<PersistedEventRecord> GetEventStream(Guid aggregateRootId);
    void AddEvents(IEnumerable<EventRecord> events);
}
