namespace Byteology.EventSourcing.EventStorage;

public interface IEventStore
{
    IEnumerable<EventRecord> GetEventStream(Guid id);
    IEnumerable<EventRecord> AddEvents(IEnumerable<EventRecord> events);
}
