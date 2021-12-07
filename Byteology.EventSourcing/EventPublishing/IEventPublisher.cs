namespace Byteology.EventSourcing.EventPublishing;

using Byteology.EventSourcing.EventStorage;

public interface IEventPublisher
{
    void PublishEvents(IEnumerable<EventRecord> events);
}
