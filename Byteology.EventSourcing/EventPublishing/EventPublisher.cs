namespace Byteology.EventSourcing.EventPublishing;

using Byteology.EventSourcing.EventStorage;

public class EventPublisher : IEventPublisher
{
    private readonly List<IEventSubscriber> _syncEventSubscribers = new();
    private readonly List<IEventSubscriber> _asyncEventSubscribers = new();

    public void PublishEvents(IEnumerable<EventRecord> events)
    {
        foreach (IEventSubscriber subscriber in _asyncEventSubscribers)
            foreach (EventRecord record in events)
                _ = subscriber.OnEventAsync(record.Event, record.Metadata);

        synchronousPublish(events);
    }

    public void RegisterAsynchronousEventSubsriber(IEventSubscriber subscriber)
        => _asyncEventSubscribers.Add(subscriber);

    public void RegisterSynchronousEventSubsriber(IEventSubscriber subscriber)
        => _syncEventSubscribers.Add(subscriber);

    private void synchronousPublish(IEnumerable<EventRecord> events)
    {
        List<Task> tasks = new();

        foreach (IEventSubscriber subscriber in _asyncEventSubscribers)
            foreach (EventRecord record in events)
                tasks.Add(subscriber.OnEventAsync(record.Event, record.Metadata));

        Task.WhenAll(tasks).Wait();
    }
}
