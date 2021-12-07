namespace Byteology.EventSourcing.EventPublishing;

using Byteology.EventSourcing.EventStorage;

public interface IEventPublisher
{
    void PublishEvents(IEnumerable<EventRecord> events);

    void RegisterSynchronousEventSubsriber(IEventSubscriber subscriber);
    void RegisterAsynchronousEventSubsriber(IEventSubscriber subscriber);
}
