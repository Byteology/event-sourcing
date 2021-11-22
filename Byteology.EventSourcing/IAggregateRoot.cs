namespace Byteology.EventSourcing;

using Byteology.EventSourcing.EventHandling.Storage;

public interface IAggregateRoot
{
    Guid Id { get; set; }

    void ReplayEvent(IEventStreamRecord @event);

    IEnumerable<IEventStreamRecord> GetNewEvents();
}
