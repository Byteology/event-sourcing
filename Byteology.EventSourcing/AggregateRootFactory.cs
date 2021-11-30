namespace Byteology.EventSourcing;

using Byteology.EventSourcing.EventStorage;

public class AggregateRootFactory
{
    private readonly IEventStore _eventStore;

    public AggregateRootFactory(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public TAggregateRoot Build<TAggregateRoot>(Guid id)
        where TAggregateRoot : IAggregateRoot, new()
    {
        TAggregateRoot root = new();
        root.Id = id;

        IEnumerable<PersistedEventRecord> eventStream = _eventStore.GetEventStream(id);
        foreach (PersistedEventRecord record in eventStream)
            root.ReplayEvent(record);

        return root;
    }
}
