namespace Byteology.EventSourcing.EventStorage;

public interface IEventStore
{
    IEnumerable<EventRecord> GetEventStream(Guid id);
    void AddEvents(IEnumerable<EventRecord> events);
}
