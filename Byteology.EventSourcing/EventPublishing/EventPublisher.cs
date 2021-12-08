namespace Byteology.EventSourcing.EventPublishing;

using Byteology.EventSourcing.EventStorage;

public class EventPublisher : IEventPublisher
{
    private readonly List<IEventSubscriber> _syncEventSubscribers = new();
    private readonly List<IEventSubscriber> _asyncEventSubscribers = new();

    public void PublishEvents(IEnumerable<EventRecord> events)
    {
        asyncPublish(events);
        syncPublish(events);
    }

    public void RegisterEventSubsriber(IEventSubscriber subscriber, bool @async = true)
    { 
        if (async)
            _asyncEventSubscribers.Add(subscriber);
        else
            _syncEventSubscribers.Remove(subscriber);
    }

    private void asyncPublish(IEnumerable<EventRecord> events)
    {
        foreach (IEventSubscriber subscriber in _asyncEventSubscribers)
            foreach (EventRecord record in events)
                _ = subscriber.OnEventAsync(record.Event, record.Metadata);
    }

    private void syncPublish(IEnumerable<EventRecord> events)
    {
        List<Task> tasks = new();

        foreach (IEventSubscriber subscriber in _syncEventSubscribers)
            tasks.Add(syncSubscriberPublish(subscriber, events));

        Task.WhenAll(tasks).Wait();
    }

    private static async Task syncSubscriberPublish(IEventSubscriber subscriber, IEnumerable<EventRecord> events)
    {
        foreach (EventRecord record in events)
            await subscriber.OnEventAsync(record.Event, record.Metadata).ConfigureAwait(false);
    }
}
