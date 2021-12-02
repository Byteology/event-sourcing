namespace Byteology.EventSourcing.EventStorage;

public interface IEventStore
{
    IEnumerable<PersistedEventRecord> GetEventStream(Guid id);
    void AddEvents(IEnumerable<EventRecord> events);
}
