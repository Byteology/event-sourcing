namespace Byteology.EventSourcing.EventHandling.Publishing;

public interface IEventBus
{
    void Publish(IEventContext<IEvent> eventContext);
}
