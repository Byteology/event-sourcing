namespace Byteology.EventSourcing.EventHandling.Publishing;

public class EventBus : IEventBus
{
    private readonly IEnumerable<IEventListener> _eventListeners;

    public EventBus(IEnumerable<IEventListener> eventListeners)
    {
        _eventListeners = eventListeners;
    }

    public void Publish(IEventContext<IEvent> eventContext)
    {
        foreach (IEventListener eventListener in _eventListeners)
        {
            try
            {
                _ = eventListener.OnEventAsync(eventContext);
            }
            catch { /* We want to notify all listeners regardless of one failing. */ }
        }
    }
}
