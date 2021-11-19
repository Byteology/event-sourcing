namespace Byteology.EventSourcing.EventHandling;

public interface IEventListener
{
    Task OnEventAsync(IEventContext eventContext);
}

public interface IEventListener<TEvent> : IEventListener
    where TEvent : IEvent
{
    Task OnEventAsync(EventContext<TEvent> eventContext);

    async Task IEventListener.OnEventAsync(IEventContext eventContext)
    {
        if (eventContext.Event is TEvent casted)
            await OnEventAsync(
                new EventContext<TEvent>(
                    eventContext.AggregateId, 
                    eventContext.AggregateType, 
                    eventContext.Timestamp, 
                    eventContext.EventSequence, 
                    casted))
                .ConfigureAwait(false);
    }
}
