namespace Byteology.EventSourcing;

using Byteology.EventSourcing.EventStorage;

public class AggregateRootFactory
{
    private readonly IEventStore _eventStore;

    public AggregateRootFactory(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public TAggregateRoot Build<TAggregateRoot>(Guid eventStreamId)
        where TAggregateRoot : IAggregateRoot, new()
    {
        TAggregateRoot root = new();
        root.EventStreamId = eventStreamId;

        IEnumerable<EventRecord> eventStream = _eventStore.GetEventStream(eventStreamId);
        foreach (EventRecord record in eventStream)
            root.ReplayEvent(record);

        return root;
    }
}
