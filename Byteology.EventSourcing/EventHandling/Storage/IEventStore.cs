namespace Byteology.EventSourcing.EventHandling.Storage;

public interface IEventStore
{
    IEnumerable<IEventStreamRecord> GetEventStream(Guid aggregateRootId);
    void AddEvents(IEnumerable<IEventStreamRecord> eventStream);
}
