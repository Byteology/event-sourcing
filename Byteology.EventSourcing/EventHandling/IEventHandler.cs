namespace Byteology.EventSourcing.EventHandling;

public interface IEventHandler<TEvent>
    where TEvent : IEvent
{
    void HandleEvent(EventHandlerContext<TEvent> context);
}
