namespace Byteology.EventSourcing.Storage;

using Byteology.EventSourcing.EventHandling;

public interface IEventStoreContext : IDisposable
{
    IEnumerable<IEventContext> GetEventStream(Guid aggregateRootId);
    void AddEvents(IEnumerable<IEventContext> eventStream);
}
