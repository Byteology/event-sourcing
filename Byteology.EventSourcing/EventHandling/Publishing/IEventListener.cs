namespace Byteology.EventSourcing.EventHandling.Publishing;

public interface IEventListener
{
    Task OnEventAsync(IEventContext<IEvent> eventContext);
}

public interface IEventListener<TEvent> : IEventListener
    where TEvent : IEvent
{
    Task OnEventAsync(IEventContext<TEvent> eventContext);

    async Task IEventListener.OnEventAsync(IEventContext<IEvent> eventContext)
    {
        if (eventContext is IEventContext<TEvent> casted)
            await OnEventAsync(casted).ConfigureAwait(false);
    }
}
