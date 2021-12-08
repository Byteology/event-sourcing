namespace Byteology.EventSourcing.EventPublishing;

public interface IEventSubscriber
{
    Task OnEventAsync(IEvent @event, EventMetadata metadata);
}

public interface IEventSubscriber<in TEvent> : IEventSubscriber
    where TEvent : IEvent
{
    Task OnEventAsync(TEvent @event, EventMetadata metadata);

    async Task IEventSubscriber.OnEventAsync(IEvent @event, EventMetadata metadata)
    {
        Type eventType = @event.GetType();
        Type genericSubscriberType = typeof(IEventSubscriber<>).MakeGenericType(eventType);

        if (this.GetType().IsAssignableTo(genericSubscriberType))
        {
            Task task = 
                (genericSubscriberType.GetMethod(nameof(OnEventAsync))!.Invoke(this, new object[] { @event, metadata }) as Task)!;

            await task.ConfigureAwait(false);
        }

    }
}
