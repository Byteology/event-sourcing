namespace Byteology.EventSourcing.Storage;

using Byteology.EventSourcing.EventHandling;

public interface IEventStore
{
    IEnumerable<IEventContext> GetEventStream(Guid aggregateRootId);
    void AddEvents(IEnumerable<IEventContext> eventStream);
    IEnumerable<IEventContext> Commit();
}
