namespace Byteology.EventSourcing.EventPublishing;

public interface IEventSubscriber
{
    Task OnEventAsync(IEvent @event, EventMetadata metadata);
}
