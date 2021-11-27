namespace Byteology.EventSourcing.Storage;

public interface IEventStore
{
    IEnumerable<PersistedEventRecord> GetEventStream(Guid aggregateRootId);
    void AddEvents(IEnumerable<EventRecord> events);
}
