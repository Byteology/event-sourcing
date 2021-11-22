namespace Byteology.EventSourcing.EventHandling.Storage;

public interface IEventStoreContext : IDisposable
{
    IEnumerable<IEventStreamRecord> GetEventStream(Guid aggregateRootId);
    void AddEvents(IEnumerable<IEventStreamRecord> eventStream);
}
